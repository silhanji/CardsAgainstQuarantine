# CardsAgainstQuarantine
Cards Against Humanity clone, which was created during 2020 Coronavirus quarantine. Aim of this is to be able to play Cards 
Against Humanity online with friends, when it is not possible to play them in person. This project doesn't aim to include
everything available in original game and it's expansion packs, nor to fully replace it. **Support the original game
developers by buing the physical copy of the game if you enjoy it.**

*Most cards used in the repository are taken from the PDF, downloadable at the Cards Against Humanity page (distributed under
Creative Commons license). Some were however removed and some were added to accommodate my personal humor.*

## Deployment
This project uses ASP.NET Core with Razor pages and SignalR. It should be ready as is to be deployed on any website running 
.NET Core runtime, successfully tested on Azure.

You will probably want to change the cards content, as the ones present on the Github are modified to fit my personal taste. 
Each white card is defined in `CardsAgainstQuarantine/WhiteCards.txt` as a single line. Each black card is defined in 
`CardsAgainstQuarantine/BlackCards.txt` as a single line. Each not interrupted sequence of underscored (`_`) is a mark that 
white card is requested at that position. If no underscore is present in black card definition that it is suppossed that 
exactly one white card is requested. In any doubt see already defined cards for reference.

*Cards like `Make a Haiku` are currently not supported.*
