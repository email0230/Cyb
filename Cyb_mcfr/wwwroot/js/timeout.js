// Constants and Variables
const n = 1; // Replace n with your desired timeout in minutes.
const timeoutInMinutes = n;
const timeoutInSeconds = n * 60;
const timeout = timeoutInSeconds * 1000; // Convert seconds to milliseconds
let logoutTimer;

// Logout-related functions
function ForceLogout() {
    window.location.href = '/Identity/Account/Logout';
}

function DebugButtonLogout() { //candidate for getting trashed soon
    console.log("Button pressed: Forcing logout by timeout...");
    ForceLogout();
}

// Timer display function
function ticker() {
    const fullTime = timeoutInSeconds;
    let counter = 0;

    setInterval(function () {
        const element = document.getElementById("tickerObject");

        if (element) {
            const timeLeft = fullTime - counter;
            element.textContent = timeLeft;
            counter++;
        }
    }, 1000);
}

logoutTimer = setTimeout(ForceLogout, timeout);
ticker();