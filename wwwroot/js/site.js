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