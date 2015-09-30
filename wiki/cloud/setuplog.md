# Pre-Setup

This section describes the setup from scratch, up to the point of being ready for a (re)start of a group of Tweet Listeners.

The setup for tweet gathering was done on 30-09-2015 using [Microsoft Azure](https://portal.azure.com) services. On the platform, a virtual machine (classic) was set up on the A1 tier (Standard A1 (1 Core, 1.75 GB memory)) - the second lowest tier, for reasons of testing performance. Setting up a proper database was not planned properly, and therefore a local database was installed on the virtual machine with [xampp](https://www.apachefriends.org/index.html). The machine offers local storage of about 120 GB on the local disk, therefore it should be plenty for the database requirements of 20 initial tweet listeners.

## Startup procedure

This section allows an inexperienced user to successfully start data gathering with minimal configuration options.

- Step 1. Starting up the virtual machine
  - The virtual machine must be accessed through the Azure portal or a Remote Desktop Connection provided from the portal beforehand. Once a machine is setup, the connection can be accessed from anywhere with the correct credentials. For the current configuration, the username and password are: `horatiu665` and `Twitter665`. All programs needed for running data gathering are available on the desktop of the machine.
  
- Step 1.1. Make sure the local server and database is running, by checking the XAMPP control panel from the icon in the lower right corner. Apache and MySQL should be running as seen in the image below.

![XAMPP settings](setup/xamppsettings.png)
  
- Step 2. Find the folder TweetListeners on the desktop
  - Inside this folder, there is a batch file, which is the only file needed to start data gathering for all games, as inside it, the configuration settings are already set up for each game.
  - Note that the batch file is not updated as the settings are changed inside the software. As a result, after tweaking the settings in the software, please be sure to change the .bat file if you want those changes to be available after a potential restart.
  
- Step 3. Run the file "TweetListenerBatch.bat" and make sure all the expected TweetListener windows are open, one for each game.
  
  
