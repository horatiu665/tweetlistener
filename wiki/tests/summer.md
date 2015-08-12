Summer tests overview
====

A large operation for gathering data on 12 different games was done over the summer starting on 02-07-2015 at 18:14.

The list of games being watched is:
- list of games with hashtags

The tweet gathering process was followed using TeamViewer, software which can be used to connect remotely over the internet. TeamViewer must be running on the target machine for the connection to work, which is reason for some of the hiccups in the gathering process.

Event log
----

Query expansion was performed manually on 22-07-2015 and resulted in 4 games receiving new hashtags as follows:
- list of games with hashtags

The first major problem was a disconnect (based on a shutdown) between 27-07-2015 18:06:08 and 03-08-2015 11:30:03. The system event logs were examined and it is unclear what caused the shutdown, but the system was clearly shut down rather than the programs crashing.

This was solved by restarting all tweetlisteners and adding the expanded hashtags manually for each game. For each game, REST was used to gather data between the previous disconnect and the restart date, and it is unknown whether all data was gathered or some of it was missed. It is possible to estimate how much data was lost by comparing the gathering frequency in the uptime with the posts gathered using REST.

- COMPARE UPTIME DATA WITH REST AND CONCLUDE IF DATA MIGHT HAVE BEEN LOST

A second disconnect occurred between 11-08-2015 23:00:00 and 12-08-2015 11:30:00 (when the system was restarted), and it was solved in the same way as the previous disconnect. Additionally, some of the tweetlisteners had some errors, hanged, and crashed.

An overview of gathered data up to this point is as follows:

- overview of gathered data
