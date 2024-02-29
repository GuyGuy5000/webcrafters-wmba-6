    $(document).ready(function () {
        $('#DivisionID').change(function () {
            var selectedDivisionId = $(this).val();

            //Get the teams from the division
            $.ajax({
                url: '/Games/GetTeamsByDivision',
                method: 'GET',
                data: { divisionId: selectedDivisionId },
                success: function (data) {
                    // Update HomeTeam dropdown
                    var homeTeamDropdown = $('#HomeTeamID');
                    homeTeamDropdown.empty();
                    homeTeamDropdown.append('<option value="">--Select Home Team--</option>');
                    $.each(data.teams, function (index, team) {
                        homeTeamDropdown.append('<option value="' + team.id + '">' + team.name + '</option>');
                    });

                    // Update AwayTeam dropdown
                    var awayTeamDropdown = $('#AwayTeamID');
                    awayTeamDropdown.empty();
                    awayTeamDropdown.append('<option value="">--Select Away Team--</option>');
                    $.each(data.teams, function (index, team) {
                        awayTeamDropdown.append('<option value="' + team.id + '">' + team.name + '</option>');
                    });
                },
                error: function () {
                    console.log('Error fetching teams for the selected division.');
                }
            });
        });

    //Location modal
    $('#toggleLocationModal').click(function () {
        $('#modalNewGameLocation').val('');
            });

    $('#saveLocation').click(function () {
                var newLocation = $('#modalNewGameLocation').val().trim();

    if (newLocation !== "") {
        $("#GameLocation").append("<option value='" + newLocation + "'>" + newLocation + "</option>");
    $('#addLocationModal').modal('hide');
                } else {
        alert("Please enter a location.");
                }
            });
        });
