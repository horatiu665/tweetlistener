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

### Analysis of tweets
This section describes the steps taken for creating an overview of the gathered data.

#### Tweets per date
To count the amount of tweets per day, the created_at column can be split by date and time, and the columns with the same date can be grouped and counted, using the following query. 

SELECT *, DATE_FORMAT(`created_at`, '%Y-%m-%d') DATEONLY, DATE_FORMAT(`created_at`, '%H:%i:%s') TIMEONLY, Count(*) FROM `godzilla` group by DATEONLY

This results in a query result in phpmysql, which can then be turned into a chart using a button in phpmyadmin. The only issue with this is that the chart will show and x axis filled with dates which are drawn on top of each other which cannot be read. Another problem is that the histogram which is practically created is too dispersed, and it would benefit from having wider bins. Therefore the following query can be used, to merge results over several days.

SELECT DATE_FORMAT(DATE_SUB(`created_at`, INTERVAL MOD(DAY(`created_at`), 3) DAY), '%Y-%m-%d') as DATES, Count(*) AS COUNT FROM `godzilla` group by DATES

The query subtracts an amount of days equal to the remainder to the division by 3 (arbitrary amount of days) from the date, and therefore groups and counts the results based on this formula, yielding wider bins for the histogram. This makes it possible to have readable x-axis values and group the bins for an easier overview over long periods of time.

