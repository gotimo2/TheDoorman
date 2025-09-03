# TheDoorman

> "Please, Excuse the mess. The Baroness is currently undergoing renovation."

TheDoorman is a Discord bot that allows you to enable people in a discord server to add themselves (or other people) to the whitelist of a Minecraft server via RCON.

you can run it via docker-compose pretty easily if you have a discord bot set up and a server to run it in:
```yaml
services:
  doorman:
    image: ghcr.io/gotimo2/the-doorman:main
    environment:
      MINECRAFT__HOSTIPADDRESS: 127.0.0.1 # <- must be an IP, sorry
      MINECRAFT__RCONPORT: 25575
      MINECRAFT__RCONPASSWORD: YOUR_RCON_PASSWORD
      DISCORD__TOKEN: YOUR_BOT_TOKEN
      DISCORD__GUILDID: YOUR_GUILD_ID
```

After which you'll have a bot with a `/check-in` command that will add the specified username to your whitelist over RCON.
