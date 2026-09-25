(() => {
    'use strict';

    const sidebar = document.getElementById('adminSidebar');
    const overlay = document.getElementById('adminSidebarOverlay');
    const openButton = document.getElementById('adminSidebarOpen');
    const closeButton = document.getElementById('adminSidebarClose');

    const setSidebar = (open) => {
        if (!sidebar || !overlay) return;
        sidebar.classList.toggle('open', open);
        overlay.classList.toggle('open', open);
        document.body.classList.toggle('admin-menu-open', open);
    };

    openButton?.addEventListener('click', () => setSidebar(true));
    closeButton?.addEventListener('click', () => setSidebar(false));
    overlay?.addEventListener('click', () => setSidebar(false));
    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') setSidebar(false);
    });

    document.querySelectorAll('.admin-nav-item').forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth <= 900) setSidebar(false);
        });
    });

    document.querySelectorAll('[data-confirm]').forEach(element => {
        element.addEventListener('click', (event) => {
            const message = element.getAttribute('data-confirm') || 'Are you sure you want to continue?';
            if (!window.confirm(message)) event.preventDefault();
        });
    });

    document.querySelectorAll('form[data-confirm]').forEach(form => {
        form.addEventListener('submit', (event) => {
            const message = form.getAttribute('data-confirm') || 'Are you sure you want to continue?';
            if (!window.confirm(message)) event.preventDefault();
        });
    });

    document.querySelectorAll('.admin-search input').forEach(input => {
        input.addEventListener('keydown', (event) => {
            if (event.key === 'Escape') {
                input.value = '';
                input.focus();
            }
        });
    });

    document.querySelectorAll('.admin-table tbody tr').forEach(row => {
        row.addEventListener('click', (event) => {
            if (event.target.closest('a, button, form, input, select')) return;
            row.classList.toggle('selected');
        });
    });
})();
