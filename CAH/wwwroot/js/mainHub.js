"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/mainHub").build();

// Disable buttons until connection is established
document.getElementById("join-submit").disabled = true;
document.getElementById("create-submit").disabled = true;

connection.on("JoinGameFail", function() {
    document.getElementById("main-error").style.display = "block";
});

connection.on("JoinGame", function(playerId, gameId) {
    document.cookie = "player-id=" + playerId;
    document.cookie = "game-id=" + gameId;
    window.location.href = "/GameLobby";
});

connection.start().then(function() {
   document.getElementById("join-submit").disabled = false;
   document.getElementById("create-submit").disabled = false;
}).catch(function(err) {
    return console.error(err.toString());
});

document.getElementById("join-submit").addEventListener("click", function(event) {
    event.preventDefault();
    var username = document.getElementById("join-player-name").value;
    var gameId = document.getElementById("join-game-id").value;
    console.log("Join clicked");
    connection.invoke("JoinGame", username, gameId).catch(function(err) {
       return console.error(err.toString());
    })
});

document.getElementById("create-submit").addEventListener("click", function(event) {
    event.preventDefault();
    var username = document.getElementById("create-player-name").value;
    console.log("Create clicked");
    connection.invoke("CreateGame", username).catch(function(err) {
        return console.error(err.toString());
    })
});
