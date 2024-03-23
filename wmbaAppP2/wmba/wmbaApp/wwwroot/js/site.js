
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Done By: Emmanuel James
// Last Modified: 2024/01/23
// Write your JavaScript code.




document.addEventListener("DOMContentLoaded", function () {
    const links = document.querySelectorAll("nav a");
    const phoneNavLinks = document.querySelectorAll(".phoneNav a");
    const scoreKeepingNavLinks = document.querySelectorAll(".scoreKeepingNav a");


    // Function to set the active link based on the current URL
    function setActiveLink() {
        const currentUrl = window.location.href;

        links.forEach(function (link) {
            const linkUrl = link.getAttribute("href");

            // Check if the current URL ends with the link's href
            if (linkUrl && currentUrl.endsWith(linkUrl)) {
                link.classList.add("active");
            } else {
                link.classList.remove("active");
            }
        });
    }

    $('#clickGame').on('click', function () {
        // Submit the form when the user confirms the save action

    });


    // Define the hideNavBar function
    function hideNavBar() {
        const currentUrl = window.location.href;
        const hideBar = document.getElementById("navBar");
        const phoneBack = document.getElementById("phoneBack");

        if (hideBar && phoneBack) {
            const mustHide = currentUrl === "https://localhost:7297/ScoreKeeping?GameID=1&HomeTeamName=Navy%20Mustangs&AwayTeamName=Green%20Jr%20Jackfish&HomeTeamScore=0&AwayTeamScore=0&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&LineUp=wmbaApp.ViewModels.PlayerScoreKeepingVM&CurrentInning=0&Score=0";
            hideBar.classList.toggle("hidden-bar", mustHide);

            // Toggle the "absolute-top" class based on the condition
            phoneBack.classList.toggle("absolute-top", mustHide);
        }
    }

    // Call the hideNavBar function when the page loads
    document.addEventListener("DOMContentLoaded", function () {
        hideNavBar();
    });

    // Call the hideNavBar function when the "START GAME" button is clicked
    $('#clickGame').on('click', function () {
        hideNavBar();
    });


    hideNavBar();
    document.addEventListener('DOMContentLoaded', function () {
        hideNavBar();

        const hideBar = document.querySelector(".navbar a");
        if (hideBar) {
            hideBar.addEventListener("click", function () {
                game();
            });
        }
    });


    function setBack() {
        const currentUrl = window.location.href;
        const phoneBack = document.getElementById("phoneBack");

        if (phoneBack) {
            // Check if it is the homepage and adjust the display property
            const shouldHide = currentUrl === "https://localhost:7297/";
            phoneBack.style.display = shouldHide ? "none" : "block";
        }
    }

    setBack();




    document.addEventListener('DOMContentLoaded', function () {
        setBack();
        showMenu();

        const phoneBack = document.querySelector(".phoneBack a");
        if (phoneBack) {
            phoneBack.addEventListener("click", function () {
                back();
            });
        }
    });

    // Set the active link on page load
    setActiveLink();
    setActiveLink(phoneNavLinks);
    setActiveLink(scoreKeepingNavLinks);

    links.forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault(); // Prevent default link behavior

            // Remove the 'active' class from all links
            links.forEach(function (otherLink) {
                otherLink.classList.remove("active");
            });

            // Add the 'active' class to the clicked link
            link.classList.add("active");

            // Manually navigate to the specified URL
            const href = link.getAttribute("href");
            if (href) {
                setTimeout(function () {
                    window.location.href = href;
                }, 100); // You can adjust the timeout if needed

                // Set the active link on the redirected page
                setActiveLink();
            }
        });
    });
});

function back() {
    window.history.back();
}

function openModal() {
    $('#customModal').show();
    $('#saveCheck').show();
    $('#required').hide();
    $('#txtSave').hide();
    $('#btnView').show();
    $('#error').hide();
    $('#hideBtn').hide();
    $('#question').show();
    $('#bck').show();
}

function closeModal() {
    $('#customModal').hide();
    $('#btnSubmit').hide();

    // Enable the dropdown
    $('#disable').prop('disabled', false);
}



$(document).ready(function () {
    // Handle the click event for the "Save" button outside the modal
    $('#btnView').on('click', function (event) {
        // Trigger form validation
        var isValid = $('#editform').valid();

        if (!isValid) {
            // If validation errors, show the error message and display the modal
            $('#customModal').show();
            $('#saveCheck').hide();
            $('#error').show();
            $('#question').hide();
            $('#txtSave').hide();
            $('#required').show();
            $('#hideBtn').hide();
            $('#btnView').show();
            $('#bck').show();

            // Disable the teamName input field after a short delay to ensure the error message is shown
            setTimeout(function () {
                $('#disable').prop('disabled', true);
            }, 100);
        } else {
            // Prevent the default form submission
            event.preventDefault();

            // Perform actions when saved and close the modal
            $('#hideBtn').show();
            $('#btnView').show();
            $('#saveCheck').show();
            $('#customModal').show();
            $('#error').hide();
            $('#required').hide();
            $('#question').show();
            $('#txtSave').show();
            $('#bck').hide();
        }
    });

    // Handle the click event for the "Back" button
    $('#hideBtn').on('click', function () {
        // Submit the form when the user confirms the save action
        $('#editform').unbind('submit').submit();
    });

    $('#clicked').on('click', function (event) {
        // Submit the form when the user confirms the save action
        event.preventDefault();
        $('#editButton').prop('disabled', true);
    });


    //$('#btnView').on('click', function () {
    //    // Submit the form when the user confirms the save action
    //    $('#bck').show();
    //});

    // Handle the click event for the "Back" button
    $('#tmsBck').on('click', function () {
        openModal();
    });

    // Handle the click event for the close button
    $(document).on('click', '#close', function () {
        closeModal();
    });
    //


    // Use jQuery to handle the click event
    $('#editButton').on('click', function (event) {
        if (gameStarted) {
            event.preventDefault();
            $('#editButton')
        }
    });





});

function showMenu() {
    if (window.location.href.includes("ScoreKeeping")) {

        document.getElementById("scoreBar").style.display = "block";
    } else {
        // Hides the navigation bar on all other pages
        document.getElementById("scoreBar").style.display = "none";
    }
}

showMenu();

$(document).ready(function () {
    $('#scoreLink').click(function () {
        // Show or hide score related content here
        // For example:
        $('#inning-info-container').toggle();
    });

    $('#teamLink').click(function () {
        // Show or hide team related content here
        // For example:
        $('#team-related-content').toggle();
    });


    $('#playsLink').click(function () {
        // Show or hide team related content here
        // For example:
        $('#inning-tracker-container').toggle();
    });
    // Similarly handle click events for other icons
});
