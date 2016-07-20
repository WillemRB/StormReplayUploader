# StormReplay Uploader
StormReplay Uploader is a Windows Service that uploads your [Heroes of the Storm](http://us.battle.net/heroes/en/) replays to different services.

Currently the following sites are supported by the uploader:
* [HotsLogs](https://www.hotslogs.com/Default)
* [Hero.gg](http://www.hero.gg/)
* [StormLogs](https://www.stormlogs.com/)

More sites can easily be added by implementing the [IStormReplayTarget](https://github.com/WillemRB/StormReplayUploader/blob/master/StormReplayUploader/IStormReplayTarget.cs) interface and adding the target to the list in the app.config.

# Configuration
When you run the installer it will automatically try to determine where your replays are located. You can manually change this path in the configuration file that is inside the installation folder you selected during installation.

To do this you must open StormReplayUploader.exe.config in a text editor and change different settings for the service.

If changes are made to the configuration file you must restart the service or your PC.
