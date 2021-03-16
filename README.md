# CircusGroupsBotCSharp

This project makes use of a few frameworks:
EntityFramework: https://docs.microsoft.com/en-us/ef/
Pomelo.EntityFrameworkCore.MySql (MariaDB): https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
Discord.net: https://docs.stillu.cc/guides/introduction/intro.html

Some level of knowledge of these frameworks will be required.


To setup your local development environment, follow these steps:

Create a discord bot:
https://docs.stillu.cc/guides/getting_started/first-bot.html

Copy or write down your bots token, you will need it later

Install MariaDB:
https://mariadb.com/downloads/

Windows:
• Clone the project
• Open cmd.exe
• run the following commands:
	• setx circusBotToken PasteBotTokenHere
	• setx dbpass ChooseAPasswordHere
• Open HeidiSQL (installed with MariaDB)
• Create a new database locally, make sure you use the same password as above
• Add your discord bot to a server
• Run the project
• You can try typing "$testcommand" in your discord server to see if the bot is alive and well.

Linux:
TODO

Mac:
• Go to store
• Buy a different computer
• See "Windows" or "Linux"