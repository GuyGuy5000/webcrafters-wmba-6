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

    //Moving a player up a division
    $(document).ready(function () {
        $("#divisionDropdown").change(function () {
            var divisionId = $(this).val();
            if (divisionId) {
                $.ajax({
                    url: "/Players/GetTeamsByDivision",
                    type: "GET",
                    data: { divisionId: divisionId },
                    success: function (data) {
                        $("#teamDropdown").html("");
                        $("#teamDropdown").append('<option value="">Assign Team to Player</option>');
                        $.each(data, function (i, item) {
                            $("#teamDropdown").append('<option value="' + item.value + '">' + item.text + '</option>');
                        });
                    }
                });
            } else {
                $("#teamDropdown").html("");
                $("#teamDropdown").append('<option value="">Assign Team to Player</option>');
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


//User Roles
$(document).ready(function () {
    if ($('input[name="selectedRoles"][value="Convenor"]').prop('checked')) {
        $('#additionalRoles').show();
    }
    $('input[name="selectedRoles"][value="Convenor"]').change(function () {
        if ($(this).is(':checked')) {
            $('#additionalRoles').show();
        } else {
            $('input[name="selectedRoles"][value*="Convenor"]').each(function (index, value) {
                value.checked = false;
                console.log(value.checked);
            });
            $('#additionalRoles').hide();

        }
    });
});



