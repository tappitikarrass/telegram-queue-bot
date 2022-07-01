# telegram-queue-bot

![](https://img.shields.io/github/license/tappitikarrass/telegram-queue-bot)

## Synopsis
Yes another simple telegram bot that can manage multiple queues of users.

## Table of contents
- [Description](#synopsis)
	- [Features](#features)
	- [Todo](#todo)
- [Environment variables](#environment-variables)
	- [Make user an admin](#make-user-an-admin)
- [Deploy to Heroku](#deploy-to-heroku)
    - [Deploy automatically](#deploy-automatically) 
    - [Deploy manually](#deploy-manually) 
- [Dependencies](#dependencies)
	- [Heroku Addons](#heroku-addons)
	- [Local development dependencies](#local-development-dependencies)
	- [NuGet dependencies](#nuget-dependencies)
## Description
Yes another simple telegram bot that can manage multiple queues of users.

What this bot can do?
- Manage multiple queues of users

### Features
- [x] Multiple queues support
- [x] Emojis in menus
- [x] Users can enlist/delist in particular queue
- [x] Remember queues list when server is restarted
- [x] Remember openned queue per user when server is restarted
- [x] Admin/user detection
- [x] Add/remove queues from list(managed by admins)

### Todo
- [ ] Multiple languages support
- [ ] Bot settings for each user
- [ ] Make all database operations async
- [ ] Queue visibitity
- [ ] Try to make tests

## Environment variables
Set these variables in Heroku dashboard. Heroku will automatically pass them to deployed Docker container.

When running locally set environment variables in `lanuchSettings.json` 
as shown [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0#development-and-launchsettingsjson).

| **DO NOT COMMIT YOUR `launchSettings.json` TO GIT REPOSITORY. IT CONTAINS SENSITIVE DATA.** |
|------------------------------------------------------------------------------------|

Environment variables table:

| Required | Variable         | Description                                                                  | Example                                     |
|----------|------------------|------------------------------------------------------------------------------|---------------------------------------------|
| yes      | `BOT_TOKEN`      | Telegram bot api token. Created by [BotFather](https://t.me/BotFather).      | `123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11` |
| yes      | `REDISCLOUD_URL` | Redis URL is **PROVIDED** by the **Redis Enterprise Cloud** extension.       | `redis://default:password@redishost:12345`  |
| no       | `ADMINS`         | A string of user_chat_id values, separated by commas.                        | `123456789,872451098`                       |
| no       | `LOG_METHOD`     | Log method. Currently only `none` or `stdout`.                               | `stdout`                                    |
| no       | `DEV`            | If enabled, you can stop the bot by pressing the Enter key (LOCAL USE ONLY). | `enabled`                                   |
### Make user an admin
Run telegram bot command `/get_user_chat_id` by the user you want to make an admin.

Add resulting id to `ADMINS` environment variable in Heroku dashboard.

Separate multiple entries with comma.

> It would be better to keep this id in private, like bot token.<br/>
Although, I don't know what bad could happen if it's publicly available.

## Deploy to Heroku
### Deploy automatically
[![Deploy](https://www.herokucdn.com/deploy/button.svg)](https://heroku.com/deploy)

## Deploy manually
1. Install [addons](#heroku-addons).
1. Set [environment variables](#environment-variables).
1. Set application stack to `container` with `heroku-cli`.
1. Connect Github repository (your fork or whatever).
1. Deploy `main` branch to Heroku.

## Dependencies
### Heroku Addons
- [Redis Enterprise Cloud](https://elements.heroku.com/addons/rediscloud)

### Local development dependencies
- [Redis](https://redis.io/): >=7.0.2(might work with 6.0, though untested)

### NuGet dependencies
- [Telegram.Bot:18.0.0](https://www.nuget.org/packages/Telegram.Bot/18.0.0)
- [GEmojiSharp:2.0.0](https://www.nuget.org/packages/GEmojiSharp/2.0.0)
- [StackExchange.Redis:2.6.4.8](https://www.nuget.org/packages/StackExchange.Redis/2.6.48)
