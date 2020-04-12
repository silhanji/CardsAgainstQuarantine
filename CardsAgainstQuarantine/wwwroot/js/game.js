"use strict";

function getSelectedCardsNumber() {
    var playerHand = document.getElementById("player-hand");
    var selected = playerHand.getElementsByClassName("selected");
    return selected.length;
}

function getPossibleSelections() {
    var playerHand = document.getElementById("player-hand");
    return playerHand.selectionsPossible;
}

function setPossibleSelections(number) {
    var playerHand = document.getElementById("player-hand");
    playerHand.selectionsPossible = number;
}

function setPlayerHand(ids, texts, selectable) {
    console.log("SET PLAYER HAND");
    var hand = document.getElementById("player-hand");
    var cards = hand.getElementsByClassName("card");
    for(var i = 0; i < cards.length; i++) {
        cards[i].innerHTML = texts[i];
        cards[i].cardId = ids[i];
        if(cards[i].classList.contains("empty"))
            cards[i].classList.remove("empty");
    }
    
    if(selectable)
        setSelectable();
    else
        setNonSelectable();
}

function selectCardOnTable(card) {
    // Deselect anything else
    var table = document.getElementById("table-white");
    var wrappers = table.getElementsByClassName("table-card-wrapper");
    for(var i = 0; i < wrappers.length; i++) {
        if(wrappers[i].classList.contains("selected"))
            wrappers[i].classList.remove("selected");
    }
    
    card.parentElement.classList.add("selected");
}

function setCardsOnTableSelectable(connection) {
    var table = document.getElementById("table-white");
    var wrappers = table.getElementsByClassName("table-card-wrapper");
    for(var i = 0; i < wrappers.length; i++) {
        var cards = wrappers[i].getElementsByClassName("card");
        for(var j = 0; j < cards.length; j++) {
            cards[j].addEventListener("click", function(event) {

                var owner = event.target.parentNode.ownerId + ""; // String needed

                connection.invoke("SelectCardOnTheTable", getCookie("game-id"),
                    getCookie("player-id"), owner).catch(function(err) {
                    console.error(err.toString());
                });
            });
        }
    }
}

function setPlayers(names, scores) {
    var playerArea = document.getElementById("players");
    // Remove any old players
    playerArea.innerHTML = "";
    
    for(var i = 0; i < names.length; i++) {
        var player = document.createElement("div");
        var name = document.createElement("span");
        name.classList.add("name");
        name.innerHTML = names[i];
        var score = document.createElement("span");
        score.classList.add("points");
        score.innerHTML = scores[i];
        player.appendChild(name);
        player.appendChild(score);
        playerArea.appendChild(player);
    }
}

function updatePlayButton() {
    var playButton = document.getElementById("player-action-play-card");
    playButton.disabled = getSelectedCardsNumber() !== getPossibleSelections();
}

function selectCard(card) {
    card.order = getSelectedCardsNumber() + 1;
    card.classList.add("selected");
    
    if(getPossibleSelections() > 1) {
        var footnote = document.createElement("span");
        footnote.textContent = card.order;
        footnote.classList.add("footnote");
        card.append(footnote);
    }
}

function deselectCard(card) {
    var oldOrder = card.order;
    var selectedCards = document.getElementById("player-hand").getElementsByClassName("selected");
    for(var i = 0; i < selectedCards.length; i++) {
        if(selectedCards[i].order > oldOrder) {
            selectedCards[i].order--;
            selectedCards[i].getElementsByClassName("footnote")[0].textContent = selectedCards[i].order;
        }
    }
    
    card.order = 0;
    var footnote = card.getElementsByClassName("footnote");
    if(footnote.length > 0) footnote[0].remove();
    card.classList.remove("selected");
    updatePlayButton();
}

function deselectAllCards() {
    var cards = docuemnt.getElementById("player-hand").getElementsByClassName("card");
    for(var i = 0; i < cards.length; i++) {
        if(cards[i].classList.contains("selected"))
            deselectCard(cards[i]);
    }
}

/**
 * Sets all cards in player hand not available for selection
 */
function setNonSelectable() {
    var cards = document.getElementById("player-hand").getElementsByClassName("card");
    for(var i = 0; i < cards.length; i++) {
        cards[i].classList.add("nonselectable");
        if(cards[i].classList.contains("selected"))
            deselectCard(cards[i]);
        
        // Remove event listeners
        var newCard = cards[i].cloneNode(true);
        newCard.cardId = cards[i].cardId;
        cards[i].parentNode.replaceChild(newCard, cards[i]);
    }
}

/**
 * Sets all cards in player hand available for selection
 */
function setSelectable() {
    var cards = document.getElementById("player-hand").getElementsByClassName("card");
    for(var i = 0; i < cards.length; i++) {
        cards[i].classList.remove("nonselectable");
        cards[i].addEventListener("click", function(event) {
            event.preventDefault();
            if(event.target.classList.contains("selected")) {
                deselectCard(event.target);
                updatePlayButton();
            } else {
                var selectedCount = getSelectedCardsNumber();
                var possibleSelection = getPossibleSelections();
                while(selectedCount >= possibleSelection) {
                    deselectCard(document.getElementsByClassName("selected")[0]);
                    selectedCount--;
                }
                selectCard(event.target);
                updatePlayButton();
            }
        });
    }
}

function revealCards(card) {
    var wrapper = card.parentElement;
    var cards = wrapper.getElementsByClassName("card");
    for(var i = 0; i < cards.length; i++) {
        var clone = cards[i].cloneNode(true);
        clone.innerHTML = cards[i].cardText;
        if(clone.classList.contains("reverse"))
            clone.classList.remove("reverse");
        wrapper.replaceChild(clone, cards[i]);
    }
}

function addCardOnTableContent(ids, texts) {
    var table = document.getElementById("table-white");
    var wrappers = table.getElementsByClassName("table-card-wrapper");
    for(var i = 0; i < ids.length; i++) {
        var cards = wrappers[i].getElementsByClassName("card");
        wrappers[i].ownerId = ids[i];
        for(var j = 0; j < cards.length; j++) {
            cards[j].cardText = texts[i][j];
        }
    }
}

function clearCardsOnTable() {
    var table = document.getElementById("table-white");
    table.innerHTML = "";
}

function addCardsOnTable(count) {
    var table = document.getElementById("table-white");
    var wrapper = document.createElement("div");
    wrapper.classList.add("table-card-wrapper");
    for(var i = 0; i < count; i++) {
        var card = document.createElement("div");
        //card.cardText = arguments[i];
        card.classList.add("card", "white", "reverse");
        card.textContent = "Cards Against Quarantine";
        card.style.zIndex = (count - i);
        if(i > 0) {
            card.style.position = "absolute";
            card.style.top = (32 * i) + "px";
        } else {
            // Add margin to allow other cards to fit
            card.style.marginBottom = ((count -1) * 32) + "px"; 
        }
        
        wrapper.appendChild(card);
    }
    table.appendChild(wrapper);
}

function addRevealListener(connection) {
    var table = document.getElementById("table-white");
    var wrappers = table.getElementsByClassName("table-card-wrapper");
    for(var i = 0; i < wrappers.length; i++) {
        var cards = wrappers[i].getElementsByClassName("card");
        for(var j = 0; j < cards.length; j++) {
            cards[j].addEventListener("click", function(event) {
                // Remove listener
                var owner = event.target.parentNode.ownerId + ""; // String needed

                connection.invoke("TurnCardOnTable", getCookie("game-id"), 
                    getCookie("player-id"), owner).catch(function(err) {
                    console.error(err.toString());
                });
            });
        }
    }
}