Summer tests overview
====

A large operation for gathering data on 12 different games was done over the summer starting on 02-07-2015 at 18:14.

The list of games being watched, along with their release dates:

- Rise of incarnates: 1 july
- Legends of Eisenwald: 2 july
- Paddington: EU 3 july, US 11 aug
- Formula 1: 10 july
- Godzilla: 14 july
- Rory McIlroy: 14 july
- The Swindle: 28 july
- King's Quest: 28 july
- Everybody's gone to the rapture: 11 august
- Armikrog: 18 august
- Madden 16: 25 august
- Zombie Vikings: tba

The tweet gathering process was followed using TeamViewer, software which can be used to connect remotely over the internet. TeamViewer must be running on the target machine for the connection to work, which is reason for some of the hiccups in the gathering process.

Event log
----

### Initial hashtags/keywords

The first keywords being followed for each game were:
- Godzilla
  -	#godzilla
  -	#godzillathegame
  -	#godzillavideogame
- Madden
  -	#madden16
- Legends of eisenwald
  -	#legendsofeisenwald
  -	Legendsofeisenwald
  -	Legends of eisenwald
- Rory McIlroy PGA Tour
  -	#pgatour
  -	#nextgengolf
  -	#rorymcilroy
  -	#rorymcilroypgatour
- Armikrog
  -	#armikrog
  -	Armikrog
- The swindle
  -	#swindle
  -	#theswindle
  -	Theswindle
  -	The swindle
- Zombie Vikings
  -	#zombievikings
  -	Zombie vikings
  -	Zombievikings
- Formula1
  -	#formula1game
  -	Formula1game
  -	#f1game
- Rise of incarnates
  -	#riseofincarnates
  -	Riseincarnates
  -	Rise of incarnates
- King’s quest
  -	#kingsquest
  -	King’s quest
- Paddington
  -	#paddington
  -	#paddingtongame
  -	#paddingtonadventuresinlondon
  -	Paddington adventures in london
- Everybody’s gone to the rapture
  -	#everybodysgonetotherapture
  -	Everybody’s gone to the rapture
  -	Everybody gone to the rapture

### First Expansion

Query expansion was performed manually on 22-07-2015 and resulted in 4 games receiving new hashtags as follows:
- Godzilla
  -	#godzilla
  -	#kaiju
  -	#gojira
  -	#godzillathegame
  -	#mothra – name of a monster in the game
  -	#eijitsuburaya – name of the main developer
  -	#kaijuroar
  -	#godzilla2014
  -	#tsuburaya
  -	#kingghidorah
  -	#mechagodzilla
  -	#shinjuku
  -	#eiji
  -	#teamkaiju
  -	#kkvg – acronym for king kong vs Godzilla
  -	#ghidorah
  -	#dinosaur
  -	#kaijuly
  -	#kaijumoviemarathon
  -	#kingofthemonsters
  -	#godzillainhell
  -	#kingofmonsters
  -	#godzilla2
  -	#godzilla2018
  -	#hedorah
  -	#kaijuking
  -	#daikaiju
  -	#godzillavideogame
- Madden
  -	#madden16
  -	#madden15
  -	#madden
  -	#maddenseason
  -	#madden16cover
- Legends of eisenwald
  -	No change
  -	#legendsofeisenwald
  -	Legendsofeisenwald
  -	Legends of eisenwald
- Rory McIlroy PGA Tour
  -	No change
  -	#pgatour
  -	#nextgengolf
  -	#rorymcilroy
  -	#rorymcilroypgatour
- Armikrog
  -	No change
  -	#armikrog
  -	Armikrog
- The swindle
  -	No change
  -	#swindle
  -	#theswindle
  -	Theswindle
  -	The swindle
- Zombie Vikings
  -	#zombievikings
  -	Zombie vikings
  -	Zombievikings
  -	Zoinkgames
  -	#zeo
  -	#zombieviking
  -	#zeos
- Formula1
  -	#f12015
  -	#formula1game
  -	Formula1game
  -	#f12015game
  -	#f1game
  -	#codemasters – developers
  -	#believeinmclarenhonda
- Rise of incarnates
  -	No change
  -	#riseofincarnates
  -	Riseincarnates
  -	Rise of incarnates
- King’s quest
  -	No change
  -	#kingsquest
  -	King’s quest
- Paddington
  -	No change
  -	#paddington
  -	#paddingtongame
  -	#paddingtonadventuresinlondon
  -	Paddington adventures in london
- Everybody’s gone to the rapture
  -	No change
  -	#everybodysgonetotherapture
  -	Everybody’s gone to the rapture
  -	Everybody gone to the rapture

### Disconnects, crashes

The first major problem was a disconnect (based on a shutdown) between 27-07-2015 18:06:08 and 03-08-2015 11:30:03. The system event logs were examined and it is unclear what caused the shutdown, but the system was clearly shut down rather than the programs crashing.

This was solved by restarting all tweetlisteners and adding the expanded hashtags manually for each game. For each game, REST was used to gather data between the previous disconnect and the restart date, and it is unknown whether all data was gathered or some of it was missed. Due to the nature of Twitter's API, it is impossible to know for sure the amount of data missed.

It is possible to estimate how much data was lost by comparing the gathering frequency in the uptime with the posts gathered using REST. It can be seen from the charts that there was a varying amount of data being recovered for different days during the crash period, for example in the king's quest chart, the amount of data gathered for 27-28-29 july is the maximum amount of data gathered throughout the whole gathering period, which is consistent with the release date of 28 july for this game, while at the same time 1-2-3 august yields a low amount of tweets - which means that the REST system did not merely gather a set maximum amount of tweets per day, but possibly returned all the tweets found by its query. However, this does not conclude that it did return all tweets, only that there was no maximum limit. It is possible that some of the tweets from the later part of the 27-july to 03-august were lost due to faulty gathering, such as the case of the game The Swindle (see histogram below), where on the 3rd august there are surprisingly few tweets compared to all neighboring days.

A second disconnect occurred between 11-08-2015 23:00:00 and 12-08-2015 11:30:00 (when the system was restarted), and it was solved in the same way as the previous disconnect. Additionally, some of the tweetlisteners had some errors, hanged, and crashed. They have all been restarted and data was recovered using REST between 9 and 12 august, to make sure all tweets were gathered. It is more likely that all data was recovered from this event due to its reduced duration and quicker response time.


### Data gathering overview

An overview of gathered data up to this point is as follows:

Note: The possible duplicate tweets from the crash periods were not cleaned up for the list below.

The total number of tweets gathered in the database for each game is:
- Armikrog: 627
- Legends of Eisenwald: 1090
- Everybody's gone to the rapture: 3870
- Formula 1: 19383
- Godzilla: 17829
- King's quest: 11889
- Madden 16: 11915
- Paddington: 3010
- Rise of incarnates: database 61 (backup text file 1092 - will be added to database at some point)
- Rory McIlroy: 26618
- The Swindle: 12425
- Zombie Vikings: 985

For each game, there are charts below showing the amount of data gathered per 3 day intervals between june and 12 august. Some games have an extra chart where the amount per day is shown, around the games' release dates.

### Analysis of tweets
This section describes the steps taken for creating an overview of the gathered data

#### Tweets per date. Method
To count the amount of tweets per day, the created_at column can be split by date and time, and the columns with the same date can be grouped and counted, using the following query. 

``` sql
SELECT *, DATE_FORMAT(`created_at`, '%Y-%m-%d') DATEONLY, DATE_FORMAT(`created_at`, '%H:%i:%s') TIMEONLY, Count(*) FROM `godzilla` group by DATEONLY
```

The query result can then be turned into a chart using a button in phpmyadmin. The only issue with this is that the chart will show an x axis filled with dates which are drawn on top of each other, which cannot be read. Another problem is that the histogram, which is practically created, is too dispersed, and it would benefit from having wider bins. Therefore the following query can be used, to merge results over several days.

``` sql
SELECT DATE_FORMAT(DATE_SUB(`created_at`, INTERVAL MOD(DAY(`created_at`), 3) DAY), '%Y-%m-%d') as DATES, Count(*) AS COUNT FROM `godzilla` group by DATES
```

The query subtracts an amount of days equal to the remainder to the division by 3 (arbitrary amount of days) from the date, and therefore groups and counts the results based on this formula, yielding wider bins for the histogram. This makes it possible to have readable x-axis values and group the bins for an easier overview over long periods of time.

Another filter to apply to this query is a limitation of the dates between june and september, to fit the data within the bounds of the experiment. For this, a WHERE extension can be applied to the previous query, yielding:

``` sql
SELECT DATE_FORMAT(DATE_SUB(`created_at`, INTERVAL MOD(DAY(`created_at`), 3) DAY), '%Y-%m-%d') as DATES, Count(*) AS COUNT FROM `godzilla` WHERE (`created_at` BETWEEN '2015-06-01 00:00:00' AND '2015-09-01 00:00:00' ) group by DATES
```

The query generates compact charts with a good overview, but some of the games have interesting evolutions near their release dates, therefore the query was modified to include every day instead of 3 days at a time - by changing the number inside the MOD() function to 1.

Later on it was clear that the format %Y-%m-%d was too long, and it was changed to %m-%d for two of the games. For one of the games (Rise of incarnates), the data was not saved in the database by mistake, only in a text file as backup. As such, charts could not be made using this method, leaving this game for later analysis.

Another trick to simplify the process of generating charts based on game release dates was to use `DATE_ADD` and `DATE_SUB` with the release date as argument. The following query was used for the second batch of charts:

```sql
SELECT DATE_FORMAT(DATE_SUB(`created_at`, INTERVAL MOD(DAY(`created_at`), 1) DAY), '%m-%d') as DATES, Count(*) AS COUNT FROM `armikrog` WHERE (`created_at` BETWEEN DATE_SUB('2015-07-01 00:00:00', INTERVAL 14 DAY) AND DATE_ADD('2015-07-01 00:00:00', INTERVAL 14 DAY)  ) group by DATES
```

#### Duplicates removal

Due to the crashes, it is possible that some of the tweets were saved multiple times in the database. The duplicates must be cleaned up for proper measurements, and the following technique was used to remove duplicates.

The following MySQL query was used to delete the duplicate tweets from a test table called everybodyrapture2.

``` sql
DELETE FROM everybodyrapture2
 WHERE id NOT IN (SELECT * 
                    FROM (SELECT MIN(n.id)
                            FROM everybodyrapture2 n
                        GROUP BY n.tweet_id_str) x)
                        
```

The query deletes all rows with ids larger than the minimum id for each group of tweets having the same tweet_id_str. This is based on [this answer on StackOverflow] (http://stackoverflow.com/a/4685232).

The following query returns the duplicates grouped by tweet_id_str, and is based on the question [mysql count duplicates] (http://stackoverflow.com/a/3935097). If it returns a null result, it means there are no duplicates in the specified table.

```sql
SELECT *,
  (
    SELECT COUNT(*)
    FROM everybodyrapture
    WHERE tweet_id_str = t1.tweet_id_str
  ) AS count
FROM everybodyrapture AS t1 where (
    SELECT COUNT(*)
    FROM everybodyrapture
    WHERE tweet_id_str = t1.tweet_id_str
  ) > 1
group by tweet_id_str
```

As a security measure, before executing the deletion, a backup copy of all tables was performed by duplicating each table and inserting all data from the previous table into the new ones.

```sql
create table eisenwald2 like eisenwald;
insert into eisenwald2 select * from eisenwald;
```

Deleting all duplicates in all tables resulted in approx. 3000 entries being deleted in total, proportional to the amount of rows in each table - an average of 2% of each table. The duplicates were most likely due to errors in the php script sending multiple calls to the same query before being executed by MySQL, leading to it not detecting that some tweets have already been saved.

#### Tweets per date. Raw data and charts

Here are histograms showing the amount of tweets gathered between 14 days before and after each game's release date, clamped to the data gathering interval, which is 2 july - 27 august (at the time of writing).

![Armikrog - 08-18](asdfasdfasdf)
![Legends of Eisenwald - 07-02](asdfsdfsdf)
Legends of Eisenwald presents the problem that gathering started during the release of the game, therefore the numbers do not accurately describe its popularity on Twitter. Also, the tweets before 07-02 were not recovered using REST, and recovery was only attempted long after that date, and the tweets are no longer available to retrieve using the API. Furthermore, some tweets from 29 and 30 June could be found by manually searching on Twitter, which confirm the fact that there are many missing tweets from the gathered data before the 2nd of July. REST recovery was attempted on 27 august for the dates prior to 02 july, with no results being retrieved.

Paddington
Among the tweets using the hashtag #Paddington there are numerous advertisements for escorts (~400) which were deleted. Another type of tweet appears around 200 times, a presumably automatic tweet containing the text `#WIN a #Paddington Summer Prize Package from @oopsimpregnant  Ends 7/10 ` followed by a different link each time. Those were also discarded. Another batch of tweets which are clearly unrelated to the game were related to an incident involving a gas explosion in Sussex Gardens - they were around 100 and were identified by containing either #sussexgardens or the word `gas`. Four of the tweets among them were related to the game, all the rest were deleted. Another batch of tweets containing the word `strike` was deleted, since it was related to the Tube strikes, and not the game or movie Paddington (around 100 tweets). Another batch containing `#Win a #Paddington Prize Package (movie, plush &amp; more) at @She_Scribes #giveaway` was deleted (27 tweets) because even though it is related to the game, it is probably posted automatically by a robot (it was posted by different users). Tweets containing `#PADDINGTON POLICE STATION ` were also deleted because they made no sense and were posted repeatedly by multiple users and were unrelated to the game (55 tweets). There are many other tweets that can be deleted among those containing #paddington, apart from those listed here.

Rise of incarnates is not yet in the database, only in a backup text file which must be uploaded to the database. It also has the issue of missing pre-release tweets, since the release date was July 1st.

Zombie vikings does not have a release date for now, and therefore the chart shows the whole month of august plus some of july.


### Reference of database table names

Here is a reference list of all games and their database table names, alphabetical order - for development purposes only:

- armikrog
- eisenwald
- everybodyrapture
- formula1
- godzilla
- kingsquest
- madden16
- paddington
- riseofincarnates
- rorymcilroy
- swindle
- zombievikings
