# Pre-Setup

This section describes the setup from scratch, up to the point of being ready for a (re)start of a group of Tweet Listeners.

The setup for tweet gathering was done on 30-09-2015 using [Microsoft Azure](https://portal.azure.com) services. On the platform, a virtual machine (classic) (this type is available when choosing what type of virtual machine to create in the Azure portal menu, but does not seem to be documented online) was set up on the A1 tier (Standard A1 (1 Core, 1.75 GB memory)) - the second lowest tier, for reasons of testing performance. Setting up a proper database was not planned properly, and therefore a local database was installed on the virtual machine with [xampp](https://www.apachefriends.org/index.html). The machine offers local storage of about 120 GB on the local disk, therefore it should be plenty for the database requirements of 20 initial tweet listeners (which based on previous experience should be less than 500 MB).

## Startup procedure

This section allows an inexperienced user to successfully start data gathering with minimal configuration options.

- Step 1. Starting up the virtual machine
  - The virtual machine (VM) must be accessed through the Azure portal or a Remote Desktop Connection provided from the portal beforehand. Once a machine is setup, the connection can be accessed from anywhere with the correct credentials. For the current configuration, the username and password are: `horatiu665` and `Twitter665`. All programs needed for running data gathering are available on the desktop of the VM.
  
- Step 1.1. Make sure the local server and database is running, by checking the XAMPP control panel from the icon in the lower right corner. Apache and MySQL should be running as seen in the image below. If there is no icon, please start XAMPP manually from the Start menu.

![XAMPP settings](setup/xamppsettings.png)
  
- Step 2. Find the folder TweetListeners on the desktop
  - Inside this folder, there is a batch file called "run oct-dec games.bat", which is the only file needed to start data gathering for all games. Inside it, the configuration settings are already set up for each game.
  - Note that the batch file is not updated when the settings are changed inside the TweetListener software. As a result, after tweaking the settings in the software, please be sure to change the .bat file if you want those changes to be available after a potential restart. This should not be required unless the changes in the software are significant and unless a restart is required using the batch file.
  
- Step 3. Run the file "run oct-dec games.bat" and make sure all the expected TweetListener windows are open, one for each game.
  - the windows will be named accordingly, so a mouse-hover over the icons on the task bar on the bottom of the screen should be enough.
  
- (optional) Step 4. Check each individual TweetListener instance by looking at the Log window in each of them. 
  - This step might be intense old-chineseman work. A future version of the software should ideally have a much better interface for running/verifying multiple streams at a glance.
  - How to check if streams are running: Is the last Log message something along the lines of "Stream started successfully" and "Rest Gathering started/completed"? Then the stream is running. The stream is not running only when there is a log message "Stream disconnected" after the latest "Stream started successfully".
  
### Process log
*Due to the A1 tier and the fact that the VM has only one core, starting 24 tweet listeners completely froze the computer. At a glance, it was obvious that the data gathering did not work as well, because there was an error with the database connection. This must be fixed and tested before the proper gathering can be started.*
The fix came in the form of creating some more timed retries in the application (i.e. when an error came from Twitter, a retry is attempted after a second or so, and the same goes for errors from the database system or other potential problems). Another major change was upgrading to the A3 tier, which provides 14 GB RAM and 4 cores to the VM. This change allowed the machine to be much more responsive when all the applications were started simultaneously. Another option attempted was to start the applications 12-at-a-time, therefore reducing the initial workload and allowing them to stabilize before the others were started, which resulted in a stable configuration where all the applications were started.

The problem now is to observe if the applications will run safely over a period of a few days. The restart procedure is simply running the batch file again, and the data gathering should take care of itself.

## List of games, release dates and queries

The list of games is based on the Wikipedia page [2015 in video gaming] (https://en.wikipedia.org/wiki/2015_in_video_gaming#Game_releases)

[This is a work in progress document where the games being searched for are described and the queries prepared](https://docs.google.com/spreadsheets/d/1ZXYjN8EHy2IchDqg0WT9960YriWKkpUA8guh68HA8C8/edit#gid=0)
