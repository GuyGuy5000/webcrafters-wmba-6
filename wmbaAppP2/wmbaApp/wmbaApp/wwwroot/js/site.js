// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Done By: Emmanuel James
// Last Modified: 2024/01/23
// Write your JavaScript code.

  


document.addEventListener("DOMContentLoaded", function () {
    const links = document.querySelectorAll("nav a");

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


    // Set the active link on page load
    setActiveLink();

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



