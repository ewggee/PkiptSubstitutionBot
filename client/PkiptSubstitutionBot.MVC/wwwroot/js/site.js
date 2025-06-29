// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    const usernameInput = document.getElementById('loggedInUsername');
    const username = usernameInput ? usernameInput.value : null;
    const greetingSpan = document.getElementById('greeting-message');

    if (username && greetingSpan) {
        const hour = new Date().getHours();
        let greetingText;

        if (hour >= 5 && hour < 12) {
            greetingText = 'Доброе утро';
        } else if (hour >= 12 && hour < 18) {
            greetingText = 'Добрый день';
        } else if (hour >= 18 && hour < 23) {
            greetingText = 'Добрый вечер';
        } else {
            greetingText = 'Доброй ночи';
        }

        greetingSpan.textContent = `${greetingText}, ${username}!`;
    }
});
