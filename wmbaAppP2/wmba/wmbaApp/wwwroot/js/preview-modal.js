// Function to update the modal content based on selected checkboxes
function updatePreviewModal() {
    var selectedHomePlayers = [];
    var selectedAwayPlayers = [];

    // going through all checkboxes with the name 'SelectedPlayers'
    var checkboxes = document.getElementsByName('SelectedPlayers');
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {
            // Determine the team of the selected player
            var team = checkboxes[i].getAttribute('data-team');
            // Add the label text (player name) to the appropriate array
            if (team === 'Home') {
                selectedHomePlayers.push(checkboxes[i].nextSibling.textContent.trim());
            } else if (team === 'Away') {
                selectedAwayPlayers.push(checkboxes[i].nextSibling.textContent.trim());
            }
        }
    }

    // Update the modal content for HomeTeam
    var homeTeamContent = generateTableContent(selectedHomePlayers);
    document.getElementById('homeTeamTableBody').innerHTML = homeTeamContent;

    // Update the modal content for AwayTeam
    var awayTeamContent = generateTableContent(selectedAwayPlayers);
    document.getElementById('awayTeamTableBody').innerHTML = awayTeamContent;
}

// Function to generate the table content for a specific team based on selected checkboxes
function generateTableContent(selectedPlayers) {
    var tableContent = '';

    // Check if there are selected players for the specified team
    if (selectedPlayers.length > 0) {
        // Generate table rows for each selected player
        for (var i = 0; i < selectedPlayers.length; i++) {
            tableContent += '<tr><td class="font-italic font-weight-bold">' + selectedPlayers[i] + '</td></tr>';
        }
    } else {
        // If no players selected, display a message
        tableContent = '<tr><td>No players selected for this team</td></tr>';
    }

    return tableContent;
}

// Attach the updatePreviewModal function to the change event of checkboxes
document.addEventListener('DOMContentLoaded', function () {
    var checkboxes = document.getElementsByName('SelectedPlayers');
    for (var i = 0; i < checkboxes.length; i++) {
        checkboxes[i].addEventListener('change', updatePreviewModal);
    }
});


//function to make Select/DeSelect all players at once to work with preview modal also

    $(document).ready(function () {
        $("#selectAllToggle").change(function () {
            $("input[name='SelectedPlayers']").prop('checked', $(this).prop("checked"));
            updatePreviewModal();
        });

    $("#previewLineupButton").click(function () {
        updatePreviewModal();
            });
        });


