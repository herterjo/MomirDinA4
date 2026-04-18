# MomirDinA4
MTG Paper Momir with a DINA4 printer

I have seen lots of Momir projects online or with a thermo-printer.

This is an application to print a DinA4 page full of creatures with the same CMC.
You can choose the basic-print button to download a page for every CMC and Momir emblems.
What has been printed is saved on the client side, so simply reload the site to reset the card pool.

This downloads the scryfall database and saves it (about 200MB).
To redownload the database, simply delete scryfallCards.json
To redownload the database at every startup, change SkipScryfallUpdate to true in config.json (will be created at first start).
In config.json you can also change Host and Port.
