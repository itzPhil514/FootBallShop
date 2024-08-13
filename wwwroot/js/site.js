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
