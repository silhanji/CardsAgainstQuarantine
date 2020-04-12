"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

connection.on("ChangeBlack", function(text, whiteNeeded) {
    var black_card = document.getElementById("table-black").getElementsByClassName("card")[0];
    black_card.innerHTML = text;
    setPossibleSelections(whiteNeeded);
});

connection.on("SetHand", function(ids, texts, selectable) {
    setPlayerHand(ids, texts, selectable);
});

connection.on("NewRound", function(czarId) {
    var scoreBtn = document.getElementById("player-action-give-points");
    scoreBtn.style.display = "none";
    clearCardsOnTable();
    if(getCookie("player-id") == czarId) {
        setNonSelectable();
    } else {
        setSelectable();
   } 
});

connection.on("AddCardOnTable", function(cardsCount) {
    addCardsOnTable(cardsCount);
});

connection.on("AddingSuccessful", function() {
   setNonSelectable();
});

connection.on("AddInfoToCardsOnTable", function(czarId, ids, texts) {
    var myId = getCookie("player-id");
    addCardOnTableContent(ids, texts);
    if(myId == czarId) {
        addRevealListener(connection);
    }
});

connection.on("RevealCard", function(ownerId) {
    var wrappers = document.getElementById("table-white").getElementsByClassName("table-card-wrapper");
    for(var i = 0; i < wrappers.length; i++) {
        if(wrappers[i].ownerId == ownerId)
            revealCards(wrappers[i].getElementsByClassName("card")[0]);
    }
});

connection.on("StartScoring", function(czarId) {
    if(czarId == getCookie("player-id")) {
        var scoreBtn = document.getElementById("player-action-give-points");
        scoreBtn.style.display = "block";
        setCardsOnTableSelectable(connection);
    }
});

connection.on("HighlightCard", function (ownerId) {
    var wrappers = document.getElementById("table-white").getElementsByClassName("table-card-wrapper");
    for(var i = 0; i < wrappers.length; i++) {
        if(wrappers[i].ownerId == ownerId) {
            var card = wrappers[i].getElementsByClassName("card")[0];
            selectCardOnTable(card);
        }
    }
});

connection.on("SetPlayers", function(names, scores) {
   setPlayers(names, scores); 
});

connection.on("UpdateScore", function(position, value) {
    
});

connection.start().then(function() {
    console.log("CONNECTION STARTED");
    connection.invoke("SetupGame", getCookie("game-id"), getCookie("player-id")).catch(function(err){
        console.error(err.toString());
    });
}).catch(function(err) {
    console.error(err.toString());
});

document.getElementById("player-action-play-card").addEventListener("click", function(event) {
    console.log("Button clicked");
    var playerHand = document.getElementById("player-hand");
    var selected = playerHand.getElementsByClassName("selected");
    var ids = [];
    for(var i = 0; i < selected.length; i++) {
        ids[selected[i].order-1] = selected[i].cardId;
    }
    connection.invoke("PlayWhiteCards", getCookie("game-id"), getCookie("player-id"), ids).catch(function(err) {
        console.error(err.toString());
    });
});

document.getElementById("player-action-give-points").addEventListener("click", function(event) {
   var selected = document.getElementById("table-white").getElementsByClassName("selected")[0];
   if(selected == null)
       return;
   var owner = selected.ownerId + "";
    connection.invoke("AddPointsToPlayer", getCookie("game-id"), getCookie("player-id"), owner).catch(function(err) {
        console.error(err.toString());
    });
});

document.getElementById("player-action-play-card").disabled = true;
