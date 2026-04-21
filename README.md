# MomirDinA4
MTG Paper Momir with a DINA4 printer

I have seen lots of Momir projects online or with a thermo-printer.

This is an application to print a DinA4 page full of creatures with the same CMC.  
You can choose the basic-print button to download a page for every CMC and Momir emblems.  
What has been printed is saved on the client side, so simply reload the site to reset the card pool.
The default address of the web-server started by the application is http://localhost:8001

This downloads the scryfall database and saves it (about 550MB).  
To redownload the database, simply delete scryfallDefaultCards.json  

Values in config.json (will be created at first start):
- SkipScryfallUpdate: Set to false to redownload the database at every startup.  
- Host and Port: Change where the webserver is reachable.  
- CubeFilename: Filename to pull cards from to include, every entry is a new line. These can also include tokens or other card types than creatures, but must be present in card database from scryfall. If empty, use hte default filter to use all creatures which are not tokens, not double-sided and no augments.   
- CubeCardsAreScryfallIdsInsteadOfNames: Determines if the entries from CubeFilename should be english card names or scryfall ids.  
- DefaultFilterIncludesDigitalCards: Set to true if you do not provide CubeFilename but want also digital cards included.  
- DefaultFilterIncludesFunnyCards: Set to true if you do not provide CubeFilename but want also cards from funny sets included (for example Un-sets).  
- MomirAvatarMtgoId: MTGO id for Momir emblem. Should be always be 23965.  