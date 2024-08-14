$(document).ready(function () {
    $('#leagueSelect').change(function () {
        var leagueId = $(this).val();
        $.ajax({
            url: '/Jerseys/GetTeamsByLeague',
            type: 'GET',
            data: { leagueId: leagueId },
            success: function (data) {
                $('#teamSelect').empty().append('<option value="">Select a Club</option>');
                $.each(data, function (index, team) {
                    $('#teamSelect').append('<option value="' + team.teamId + '">' + team.teamName + '</option>');
                });
            }
        });
    });
});
$(document).ready(function () {
    function updateCartQuantity() {
        $.getJSON('/Cart/TotalQuantity', function (data) {
            if (data > 0) {
                $('#cart-quantity').text(data).show();
            } else {
                $('#cart-quantity').hide();
            }
        });
    }
    updateCartQuantity();
});
$(document).ready(function () {
    function updateWishlistQuantity() {
        $.getJSON('/Wishlist/TotalItems', function (data) {
            if (data > 0) {
                $('#wishlist-quantity').text(data).show();
            } else {
                $('#wishlist-quantity').hide();
            }
        });
    }
    updateWishlistQuantity();
});
$(document).ready(function () {
    function updateDropdowns(isInter) {
        let url = isInter ? '/Jerseys/GetInternationalLeaguesAndNations' : '/Jerseys/GetRegularLeaguesAndClubs';

        $.getJSON(url, function (data) {
            console.log("Received data:", data);

            // Clear and update the league dropdown
            $('#leagueSelect').empty().append('<option value="">Select a League</option>');
            if (isInter) {
                $.each(data.interLeagues, function (i, item) {
                    $('#leagueSelect').append($('<option>').val(item.value).text(item.text));
                });
            } else {
                $.each(data.leagues, function (i, item) {
                    $('#leagueSelect').append($('<option>').val(item.value).text(item.text));
                });
            }

            // Clear and update the club/nation dropdown
            $('#teamSelect').empty().append('<option value="">Select a Club</option>');
            $('#nationSelect').empty().append('<option value="">Select a Nation</option>');
            if (isInter) {
                $('#teamSelect').hide(); // Hide club dropdown
                $('#nationSelect').show(); // Show nation dropdown
                $.each(data.nations, function (i, item) {
                    $('#nationSelect').append($('<option>').val(item.value).text(item.text));
                });
            } else {
                $('#teamSelect').show(); // Show club dropdown
                $('#nationSelect').hide(); // Hide nation dropdown
                $.each(data.clubs, function (i, item) {
                    $('#teamSelect').append($('<option>').val(item.value).text(item.text));
                });
            }
        }).fail(function () {
            console.error('Error fetching data.');
        });
    }

    // Bind change event to radio buttons
    $('input[name="IsInter"]').change(function () {
        let isInter = $(this).val() === 'true';
        console.log("Selected type:", isInter);
        updateDropdowns(isInter);
    });

    // Trigger change event on page load to set initial values
    $('input[name="IsInter"]:checked').trigger('change');
});
