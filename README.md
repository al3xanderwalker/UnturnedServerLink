# Unturned Server Link

Having closed my vanilla servers I have decided to release the system I made to manage it, Whether it is any good is for you to decide.

## Features

Useful admin commands such as god/vanish  
Stat tracking  
Banning accross all linked servers  
Discord chat and link and live status  
Death logs including information + spy of the killer  
Vote checker/tracker  
Autosave / Auto announcer  
Ability to increase queue slots  
Combattracking (Logs in console if someone combat logs)  

## Setting Up

### Server

1. Run `npm install` in the Server folder to install the required packages;
2. Run `tsc` to compile the the typescript files into javascript
3. Change the settings in `config.yml`

| Setting       | Description                                                                                                  |
| ------------- | ------------------------------------------------------------------------------------------------------------ |
| botToken      | Your discord bot application [token](https://discordjs.guide/preparations/setting-up-a-bot-application.html) |
| color         | Color used for embed messages                                                                                |
| iconUrl       | Sets the icon used for some messages ?                                                                       |
| authToken     | Token used by servers to authenticate, make sure its long and random                                         |
| ownerId       | Owners discord account id, used for allowing the configuration of admins                                     |
| autoRoleId    | Id of the role to be assigned on joining the discord, set to `''` to disable                                 |
| modulePort    | Port used for the loader to download the module from, ensure its not blocked by the firewall                 |
| websocketPort | Port used to establish the connection between the servers, ensure its not blocked by the firewall            |

4. Run `node server.js` to start the server.

### Loader

1. Build the USLLoader and copy its output into a new folder inside Modules, e.g. `Unturned/Modules/USLLoader`
2. Copy the `UnturnedServerLink.module` file from the Client directory into the folder created in step 1
3. Start the server, wait for it to generate the loader config file then stop the server
4. Open the `loader_config.json` file and modify the settings to use your Webservers IP and the `modulePort`

### Module

1. Having configured the loader, start the server and allow the config file to generate then stop the server.
2. Change the settings in `config.json`

| Setting            | Description                                                                                       |
| ------------------ | ------------------------------------------------------------------------------------------------- |
| WelcomeMessage     | Message sent to the player on join                                                                |
| MessageIconUrl     | Icon for messages sent by the server                                                              |
| ChatAnnouncement   | Message sent in chat on interval configured below                                                 |
| AuthorizationToken | Token configured when setting up the Server                                                       |
| ServerId           | An identifier used by the server e.g. EU1 (Should always be unique)                               |
| ChatChannel        | ID of the discord channel to be used for bidirectional chat                                       |
| DeathLogChannel    | ID of the discord channel for the deathlogs to be sent to                                         |
| ServerListID       | ID of the server on unturned-servers.net                                                          |
| ServerListAPIKey   | API key of the server on unturned-servers.net                                                     |
| QueueLimit         | Maximum amount of people in the servers queue at once                                             |
| SocketTimeout      | How long before attempting to reestablish the connection (should not be below 1000)               |
| ServerIp           | IP of the webserver to connect to                                                                 |
| ServerPort         | Port of the webserver to connect to, used `websocketPort`                                         |
| SaveInterval       | Interval between auto saves (Measured in milliseconds)                                            |
| DiscordLink        | Link used for /discord command and the browser request on join.                                   |
| MessageInterval    | Time between ChatAnnouncements (Measured in milliseconds)                                         |
| CombatExpiration   | How long it takes from the last time a player was hit for them to be out of combat (milliseconds) |

3. Start the server

## Commands

### In Game

Globalban/Unban commands can only be used on online players

| Command                     | Description                                                                 | Admin Only         |
| --------------------------- | --------------------------------------------------------------------------- | ------------------ |
| CheckOwner                  | Tells you the owners id of a barricade/structure/vehicle you are looking at | :heavy_check_mark: |
| ClearInventory              | Clears your inventory                                                       | :heavy_check_mark: |
| Discord                     | Opens a browser request with your discord link                              | :x:                |
| GlobalBan <Steam Id/Name>   | Bans a player on all servers                                                | :heavy_check_mark: |
| GlobalUnban <Steam Id/Name> | Unbans a player on all servers                                              | :heavy_check_mark: |
| God                         | Makes you invincible to player (and zombie?) damage                         | :heavy_check_mark: |
| Link                        | Generates a code which can be used to link your discord account             | :x:                |
| ToggleCheats                | Toggles the whether cheats are enabled on the server                        | :heavy_check_mark: |
| Vanish                      | Toggles whether you are invivisble to players                               | :heavy_check_mark: |

### Discord

Case sensetive

| Command                    | Description                                                                                      | Admin Only             |
| -------------------------- | ------------------------------------------------------------------------------------------------ | ---------------------- |
| globalban <ID/Name/HWID>   | Bans all players by both Steam ID and HWID on each server                                        | :heavy_check_mark:     |
| globalunban <ID/Name/HWID> | Unbans all players by both Steam ID and HWID on each server                                      | :heavy_check_mark:     |
| stats <ID/Name>            | Returns a players kills, deaths, kdr, experience, reputation and playtime                        | :x:                    |
| info <ID/Name/HWID>        | Returns all information about a player excludes their ip                                         | :heavy_check_mark:     |
| search [key] [value]       | Returns the top 10 players in the database with the same Value for the Key                       | :heavy_check_mark:     |
| leaderboard                | Creates a message and uses it as the leaderboard. **will overwrite previous leaderboard**        | :heavy_check_mark:     |
| setinfo                    | Creates a message and uses it as the servers status. **will overwrite previous server status**   | :heavy_check_mark:     |
| setlog                     | Sets the channel to be used for ban logs. **will overwrite previous channel**                    | :heavy_check_mark:     |
| prefix [New Prefix]        | If no prefix is specified it will display the current one, else it will change it to the new one | :x:/:heavy_check_mark: |
| link [Code]                | Used to link your discord account to your steam account, you get your code using /link in game   | :x:                    |
| announce <Message/Command> | Sends the specified message/command to all servers (Command signified by /)                      | :heavy_check_mark:     |
| votes                      | Used to check/claim your votes for the past 24 hours                                             | :x:                    |
