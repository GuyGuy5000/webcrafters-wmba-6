
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



    // Define the hideNavBar function



    // Function to hide the navigation bar based on the current URL
    function hideNavBar() {
        const currentUrl = window.location.href;
        const phoneBack = document.getElementById("phoneBack");

        // Check if the current URL indicates a scorekeeping page or homepage
        if (phoneBack) {
            const shouldHide = currentUrl.includes("ScoreKeeping") || currentUrl === "https://localhost:7297/";
            phoneBack.style.display = shouldHide ? "none" : "block"; // Hide or show based on condition
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
  
    // Event listener for clicking links in the navigation bar
    links.forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault(); // Prevent default link behavior

            // Removes the 'active' class from all links
            links.forEach(function (otherLink) {
                otherLink.classList.remove("active");
            });

            // This will add the 'active' class to the clicked link
            link.classList.add("active");

            // Manually navigates to the specified URL after a short delay
            const href = link.getAttribute("href");
            if (href) {
                setTimeout(function () {
                    window.location.href = href;
                }, 100);

                // Set the active link on the redirected page
                setActiveLink();
            }
        });
    });

    // Set the active link on page load for different navigation sections
    setActiveLink();
    setActiveLink(phoneNavLinks);
    setActiveLink(scoreKeepingNavLinks);
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
            $('notify').hide();
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
            $('notify').show();
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
        document.querySelector("nav[class=phoneNav]").style.display = "none";
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

$(document).ready(function () {
    // Function to fade out the success message after 3 seconds (adjust as needed)
    setTimeout(function () {
        $('#successMessage').fadeOut('slow');
    }, 3000); // 3000 milliseconds = 3 seconds

    // Optional: You can also close the success message when clicking on it
    $('#successMessage').click(function () {
        $(this).fadeOut('slow');
    });
});
document.addEventListener("DOMContentLoaded", function () {
    const currentUrl = window.location.href;
    const logo = document.getElementById("img");
    const homeLogo = document.getElementById("imgHome");

    // Check if the current URL indicates a scorekeeping page or homepage
    if (logo && homeLogo) {
        const isScoreKeepingPage = currentUrl.includes("ScoreKeeping");
        if (isScoreKeepingPage) {
            logo.style.display = "block"; // Show the statsLogo
            homeLogo.style.display = "none"; // Hide the homeLogo
        } else {
            logo.style.display = "none"; // Hide the statsLogo
            homeLogo.style.display = "block"; // Show the homeLogo on other pages
        }
    }
});


