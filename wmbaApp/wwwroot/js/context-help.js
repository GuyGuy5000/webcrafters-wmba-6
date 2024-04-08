window.onload = () => {
    RemoveUnusedHelpIcon();

    function RemoveUnusedHelpIcon() {
        body = document.body;
        //if GenerateContextHelp was not called, it means the help box is empty and won't display
        if (!(body.innerHTML.includes('GenerateContextHelp(['))) {
            helpIcon = document.getElementsByClassName("help-icon-container"); //gets the help icon
            helpIcon[0].remove(); //removes help icon
        }
    }
};

function GenerateContextHelp(explanations) {

    let helpIcons = document.querySelectorAll('.help-icon-container');
    let helpBox = document.getElementById('help-box');
    let nextButton = document.getElementById('nextBtn');
    let skipButton = document.getElementById('skipBtn');
    let pageNum = document.getElementById("pageNum");
    let helpContent = document.querySelector('.help-content');
    let explanationIndex = 0;
    firstTimeHelp();

    helpIcons.forEach(helpIcon => {
        helpIcon.addEventListener('keydown', (event) => {
            if (event.code == "Enter") {
                explanationIndex = 0;
                helpBox.style.display = 'block';
                displayExplanation(explanationIndex);
            }

        });

        helpIcon.addEventListener('click', () => {
            explanationIndex = 0;
            helpBox.style.display = 'block';
            displayExplanation(explanationIndex);
        });
    });

    nextButton.addEventListener('click', () => {
        explanationIndex++;
        if (explanationIndex < explanations.length) {
            displayExplanation(explanationIndex);
        } else {
            helpBox.style.display = 'none';
        }
    });

    skipButton.addEventListener('click', () => {
        helpBox.style.display = 'none';
    });

    function displayExplanation(index) {
        helpContent.innerHTML = explanations[index];
        pageNum.innerText = `${index + 1}/${explanations.length}`;
        if (index == explanations.length - 1)
            nextButton.textContent = "close";
        else
            nextButton.textContent = "next";
    }

    async function firstTimeHelp() {
        function sleep(ms) {
            return new Promise(resolve => setTimeout(resolve, ms));
        }
        await sleep(1000); // Waits 1 second (1000 milliseconds)
        // Check if it's a first-time visit to the site (will reset when the session ends)
        if (sessionStorage["firstTimeHelpFlag"] == undefined) {
            sessionStorage.setItem("firstTimeHelpFlag", "false"); // Set flag to indicate the homepage was visited
            // Display help box
            helpBox.style.display = 'block';
            displayExplanation(0);
        }
    }
}
