"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/lobbyHub").build();

connection.on("PlayerJoined", function(playerName) {
    var li = document.createElement("li");
    var text = document.createTextNode(playerName);
    li.appendChild(text);
    document.getElementById("lobby-players").append(li);
});

connection.on("StartGame", function() {
    window.location.href = "/GamePage";
});

connection.start().then(function() {
   connection.invoke("PlayerJoined", getCookie("game-id"), getCookie("player-id")).catch(function(err){
       console.error(err.toString());
   });
}).catch(function(err) {
    console.error(err.toString());
});

var startButton = document.getElementById("lobby-start-game");
if(startButton !== null) {
    startButton.addEventListener("click", function(event) {
        event.preventDefault();
        connection.invoke("StartGame", getCookie("game-id"), getCookie("player-id")).catch(function(err) {
            console.error(err.toString());
        });
    });
}
