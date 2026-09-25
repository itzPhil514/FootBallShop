/* ============================================================
   FootBallShop — site.js
   UI interactions, theme, search, badges and product forms.
   ============================================================ */

(function ($) {
    'use strict';

    const FBS = {
        themeKey: 'fbs-theme',
        searchDelay: 280,
        searchRequest: null,

        init() {
            this.initTheme();
            this.initToast();
            this.initSearch();
            this.initBadges();
            this.initJerseyForm();
            this.initQuantity();
        },

        // -------------------------------------------------------
        // Theme
        // -------------------------------------------------------
        initTheme() {
            const html = document.documentElement;
            const button = document.getElementById('theme-toggle');
            const icon = document.getElementById('theme-icon');
            const label = document.getElementById('theme-label');
            const media = window.matchMedia('(prefers-color-scheme: dark)');

            const getSystemTheme = () => media.matches ? 'dark' : 'light';

            const updateUI = (theme) => {
                html.setAttribute('data-theme', theme);

                if (icon) {
                    icon.className = theme === 'dark'
                        ? 'fas fa-sun'
                        : 'fas fa-moon';
                }

                if (label) {
                    label.textContent = theme === 'dark' ? 'Light' : 'Dark';
                }

                if (button) {
                    button.setAttribute(
                        'aria-label',
                        theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'
                    );
                    button.setAttribute('aria-pressed', theme === 'dark' ? 'true' : 'false');
                }
            };

            const applyTheme = (theme, persist = true) => {
                updateUI(theme);
                if (persist) {
                    localStorage.setItem(this.themeKey, theme);
                }
            };

            const saved = localStorage.getItem(this.themeKey);
            updateUI(saved || getSystemTheme());

            if (button) {
                button.addEventListener('click', () => {
                    const current = html.getAttribute('data-theme') || 'light';
                    applyTheme(current === 'dark' ? 'light' : 'dark');
                });
            }

            media.addEventListener?.('change', () => {
                if (!localStorage.getItem(this.themeKey)) {
                    updateUI(getSystemTheme());
                }
            });
        },

        // -------------------------------------------------------
        // Toasts
        // -------------------------------------------------------
        initToast() {
            document.querySelectorAll('#fbs-toast').forEach((toast) => {
                window.setTimeout(() => {
                    toast.style.transition = 'opacity .2s ease, transform .2s ease';
                    toast.style.opacity = '0';
                    toast.style.transform = 'translateY(-8px)';

                    window.setTimeout(() => {
                        toast.closest('.fbs-toast-container')?.remove();
                    }, 220);
                }, 4000);
            });
        },

        // -------------------------------------------------------
        // Search  (jerseys · clubs · nations tabs)
        // -------------------------------------------------------
        initSearch() {
            const input = $('#search-query');
            const results = $('#search-results');
            const list = $('#search-results-list');
            const container = $('#search-container');

            if (!input.length || !results.length) return;

            let timer = null;
            let activeType = 'all';

            const escapeHtml = (v) => $('<div>').text(v ?? '').html();

            const hideResults = () => {
                results.stop(true, true).fadeOut(100);
                input.attr('aria-expanded', 'false');
            };

            const showResults = () => {
                results.stop(true, true).fadeIn(120);
                input.attr('aria-expanded', 'true');
            };

            // Icon per result type
            const typeIcon = { jersey: 'fa-shirt', club: 'fa-shield-halved', nation: 'fa-flag' };
            const typeLabel = { jersey: 'Jersey', club: 'Club', nation: 'Nation' };

            const renderResults = (items) => {
                list.empty();

                if (!Array.isArray(items) || items.length === 0) {
                    list.html(
                        '<div class="fbs-search-empty">' +
                        '<i class="fas fa-search me-2"></i>No results found</div>'
                    );
                    showResults();
                    return;
                }

                items.slice(0, 10).forEach((item) => {
                    const url = escapeHtml(item.url || '#');
                    const name = escapeHtml(item.name || '');
                    const subtitle = escapeHtml(item.subtitle || '');
                    const img = escapeHtml(item.img || '');
                    const icon = typeIcon[item.type] || 'fa-shirt';
                    const badge = typeLabel[item.type] || '';

                    // img path: jerseys use plain filename; clubs/nations already prefixed
                    const imgSrc = item.type === 'jersey'
                        ? `/img/jerseys/${img}`
                        : `/img/${img}`;

                    list.append(`
                        <a href="${url}" class="fbs-search-item">
                            <img src="${imgSrc}" alt="" loading="lazy" class="fbs-search-thumb">
                            <div class="fbs-search-copy">
                                <strong>${name}</strong>
                                <span>${subtitle}</span>
                            </div>
                            <span class="fbs-search-badge">
                                <i class="fas ${icon}"></i> ${badge}
                            </span>
                        </a>
                    `);
                });

                showResults();
            };

            const search = (query, type) => {
                if (this.searchRequest) {
                    this.searchRequest.abort();
                    this.searchRequest = null;
                }

                if (query.length < 2) { hideResults(); return; }

                list.html(
                    '<div class="fbs-search-empty">' +
                    '<i class="fas fa-spinner fa-spin me-2"></i>Searching…</div>'
                );
                showResults();

                this.searchRequest = $.ajax({
                    url: '/Home/SearchAjax',
                    method: 'GET',
                    data: { query, type },
                    dataType: 'json',
                    timeout: 7000
                })
                    .done(renderResults)
                    .fail((xhr, status) => {
                        if (status === 'abort') return;
                        list.html(
                            '<div class="fbs-search-empty">' +
                            '<i class="fas fa-exclamation-circle me-2"></i>' +
                            'Search unavailable</div>'
                        );
                        showResults();
                    })
                    .always(() => { this.searchRequest = null; });
            };

            // Tab clicks
            $(document).on('click', '.fbs-search-tab', function () {
                $('.fbs-search-tab').removeClass('active');
                $(this).addClass('active');
                activeType = $(this).data('type') || 'all';

                const query = String(input.val() || '').trim();
                if (query.length >= 2) search(query, activeType);
            });

            input
                .attr('aria-expanded', 'false')
                .on('input', function () {
                    const query = String($(this).val() || '').trim();
                    clearTimeout(timer);
                    timer = window.setTimeout(() => search(query, activeType), FBS.searchDelay);
                })
                .on('keydown', function (e) {
                    if (e.key === 'Escape') { hideResults(); $(this).trigger('blur'); }
                });

            $(document).on('click', function (e) {
                if (!container.is(e.target) && container.has(e.target).length === 0) {
                    hideResults();
                }
            });
        },

        // -------------------------------------------------------
        // Header badges
        // -------------------------------------------------------
        initBadges() {
            const updateBadge = (url, selector) => {
                $.getJSON(url)
                    .done((data) => {
                        const quantity = Number(data) || 0;
                        const badge = $(selector);

                        if (!badge.length) return;

                        if (quantity > 0) {
                            badge.text(quantity > 99 ? '99+' : quantity).show();
                        } else {
                            badge.hide();
                        }
                    })
                    .fail(() => {
                        // Keep the UI quiet if the endpoint is unavailable.
                    });
            };

            updateBadge('/Cart/TotalQuantity', '#cart-quantity');
            updateBadge('/Wishlist/TotalItems', '#wishlist-quantity');
        },

        // -------------------------------------------------------
        // Jersey create/edit form
        // -------------------------------------------------------
        initJerseyForm() {
            const internationalToggle = $('#IsInterTrue');

            if (!internationalToggle.length) return;

            const $regular = $('#regular-jersey-fields');
            const $international = $('#international-jersey-fields');

            const fillDropdown = (selector, items, placeholder = 'Select…') => {
                const dropdown = $(selector);
                if (!dropdown.length) return;

                dropdown.empty().append(
                    $('<option>', { value: '', text: placeholder })
                );

                (Array.isArray(items) ? items : []).forEach((item) => {
                    dropdown.append(
                        $('<option>', {
                            value: item.value ?? '',
                            text: item.text ?? ''
                        })
                    );
                });
            };

            const loadJson = (url, data, onSuccess) => {
                return $.getJSON(url, data)
                    .done(onSuccess)
                    .fail(() => {
                        // Don't break the form when an optional lookup fails.
                    });
            };

            const loadLeagues = () => {
                loadJson('/Jerseys/GetRegularLeaguesAndClubs', null, (data) => {
                    fillDropdown('#LeagueDropdown', data.leagues);
                    fillDropdown('#ClubDropdown', []);
                });
            };

            const loadInternationalLeagues = () => {
                loadJson('/Jerseys/GetInternationalLeaguesAndNations', null, (data) => {
                    fillDropdown('#InterLeagueDropdown', data.interLeagues);
                    fillDropdown('#NationDropdown', []);
                });
            };

            const toggleFields = () => {
                const isInternational = internationalToggle.is(':checked');

                $regular.toggleClass('d-none', isInternational);
                $international.toggleClass('d-none', !isInternational);

                if (isInternational) {
                    loadInternationalLeagues();
                } else {
                    loadLeagues();
                }
            };

            $('input[name="IsInter"]').on('change', toggleFields);

            $('#LeagueDropdown').on('change', function () {
                const leagueId = $(this).val();

                if (!leagueId) {
                    fillDropdown('#ClubDropdown', []);
                    return;
                }

                loadJson('/Jerseys/GetClubsByLeague', { leagueId }, (data) => {
                    fillDropdown('#ClubDropdown', data.clubs);

                    if (Array.isArray(data.clubs) && data.clubs.length) {
                        $('#ClubDropdown').val(data.clubs[0].value);
                    }
                });
            });

            $('#InterLeagueDropdown').on('change', function () {
                const interLeagueId = $(this).val();

                if (!interLeagueId) {
                    fillDropdown('#NationDropdown', []);
                    return;
                }

                loadJson(
                    '/Jerseys/GetNationsByInterLeague',
                    { interLeagueId },
                    (data) => {
                        fillDropdown('#NationDropdown', data.nations);

                        if (Array.isArray(data.nations) && data.nations.length) {
                            $('#NationDropdown').val(data.nations[0].value);
                        }
                    }
                );
            });

            toggleFields();
        },

        // -------------------------------------------------------
        // Quantity
        // -------------------------------------------------------
        initQuantity() {
            const input = document.getElementById('quantity');
            if (!input) return;

            input.addEventListener('change', () => {
                let value = parseInt(input.value, 10);
                if (!Number.isFinite(value) || value < 1) value = 1;

                const max = parseInt(input.max, 10);
                if (Number.isFinite(max) && max > 0) {
                    value = Math.min(value, max);
                }

                input.value = value;
            });
        }
    };

    $(function () {
        FBS.init();
    });

    // Kept global because existing Razor views use onclick="changeQuantity(...)"
    window.changeQuantity = function (change) {
        const input = document.getElementById('quantity');
        if (!input) return;

        let value = parseInt(input.value, 10) || 1;
        value = Math.max(1, value + Number(change || 0));

        const max = parseInt(input.max, 10);
        if (Number.isFinite(max) && max > 0) {
            value = Math.min(value, max);
        }

        input.value = value;
        input.dispatchEvent(new Event('change', { bubbles: true }));
    };

})(jQuery);