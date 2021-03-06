﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

document.getElementById("sendButton").disabled = true;

connection.on("DataUpdated", function () {
    updateMessages();
});

connection.on("DataUpdated2", function () {
    updateMessage2s();
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

updateMessages();
updateMessage2s();

function updateMessages() {
    fetch('/messages')
        .then(res => res.json())
        .then(data => {
            var ul = document.getElementById("messagesList");
            ul.innerHTML = '';
            data.forEach(function (e) {
                var msg = e.value && e.value.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                var li = document.createElement("li");
                li.textContent = msg;
                ul.appendChild(li);
            });
        });
}

function updateMessage2s() {
    fetch('/message2s')
        .then(res => res.json())
        .then(data => {
            var ul = document.getElementById("messagesList2");
            ul.innerHTML = '';
            data.forEach(function (e) {
                var msg = e.value && e.value.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                var li = document.createElement("li");
                li.textContent = msg;
                ul.appendChild(li);
            });
        });
}