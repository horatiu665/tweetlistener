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

Gathering ended on 03-09-2015 at 9:19, except for Armikrog, Madden16 and Zombie Vikings for which it continued until a later point.

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

Query expansion was performed manually on 22-07-2015. [The expansion process is detailed here](../expansionprocess.md).

Expansion resulted in 4 games receiving new hashtags as follows:
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

An overview of gathered data up to this point (12-08-2015 11:30:00) is as follows:

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

Charts were made for each game, presenting gathering amounts per 3 days and per day intervals.

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

![Armikrog - 08-18](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/armikrog%2018%20aug%20copy.png)

An important mention is that the [release date for Armikrog was pushed back to 8 September](http://www.gamerheadlines.com/2015/08/armikrog-release-date-pushed-back/).

![Legends of Eisenwald - 07-02](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/eisenwald%202%20july%20copy.png)

Legends of Eisenwald presents the problem that gathering started during the release of the game, therefore the numbers do not accurately describe its popularity on Twitter. Also, the tweets before 07-02 were not recovered using REST, and recovery was only attempted long after that date, and the tweets are no longer available to retrieve using the API. Furthermore, some tweets from 29 and 30 June could be found by manually searching on Twitter, which confirm the fact that there are many missing tweets from the gathered data before the 2nd of July. REST recovery was attempted on 27 august for the dates prior to 02 july, with no results being retrieved.

![Everybody's gone to the rapture - 08-11](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/everybody%20rapture%2011%20aug%20copy.png)

![Formula 1 - 07-10](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/formula%201%2010%20july%20copy.png)

![Godzilla - 07-14](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/godzilla%2014%20july%20us%2017%20july%20eu%20copy.png)

![King's quest - 07-28](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/kings%20quest%2028%20july%20copy.png)

![Madden16 - 08-25](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/madden16%2025%20aug%20copy.png)

![Paddington - 07-03](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/paddington%203%20july%2011%20aug.png)

Among the tweets using the hashtag #Paddington there are numerous advertisements for escorts (~400) which were deleted. Another type of tweet appears around 200 times, a presumably automatic tweet containing the text `#WIN a #Paddington Summer Prize Package from @oopsimpregnant  Ends 7/10 ` followed by a different link each time. Those were also discarded. Another batch of tweets which are clearly unrelated to the game were related to an incident involving a gas explosion in Sussex Gardens - they were around 100 and were identified by containing either #sussexgardens or the word `gas`. Four of the tweets among them were related to the game, all the rest were deleted. Another batch of tweets containing the word `strike` was deleted, since it was related to the Tube strikes, and not the game or movie Paddington (around 100 tweets). Another batch containing `#Win a #Paddington Prize Package (movie, plush &amp; more) at @She_Scribes #giveaway` was deleted (27 tweets) because even though it is related to the game, it is probably posted automatically by a robot (it was posted by different users). Tweets containing `#PADDINGTON POLICE STATION ` were also deleted because they made no sense and were posted repeatedly by multiple users and were unrelated to the game (55 tweets). There are many other tweets that can be deleted among those containing #paddington, apart from those listed here.

![Rory McIlroy - 07-14](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/rory%20mcilroy%2014%20july%20copy.png)

![The Swindle - 07-28](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/swindle%2028%20jul%20copy.png)

![Zombie Vikings - tba](https://github.com/horatiu665/tweetlistener/blob/newMaster/wiki/tests/tweethistograms/14%20days/zombie%20vikings.png)

Zombie vikings does not have a release date for now, and therefore the chart shows the whole month of august plus some of july.

Rise of incarnates is not yet in the database, only in a backup text file which must be uploaded to the database. It also has the issue of missing pre-release tweets, since the release date was July 1st.

### Random sample of tweets

A random sample of 200 tweets from the interval of 14 days before and after release dates for each game must be taken. For this, a SQL query is created. The query filters by english language, only tweets created 14 days before or after the release date, and, in a random order, the ones that fit the rule that `rand() < x`, where `x` is a small number proportional to how many tweets there are in the collection - this avoids long waiting times to calculate random numbers for each tweet and sorting them all, when the tweet population might be very large. `order by id` is used to sort the tweets chronologically, and therefore achieve a more uniform distribution of tweets from the point of view of their dates. For this, the `x` mentioned earlier must be fine tuned so the results yield the right amount of tweets to span the whole period but not too many.

```sql
SELECT * FROM `armikrog` WHERE lang = 'en' and (`created_at` BETWEEN DATE_SUB('2015-08-18 00:00:00', INTERVAL 14 DAY) AND DATE_ADD('2015-08-18 00:00:00', INTERVAL 14 DAY) ) and rand() < 0.3 order by id limit 200
```

In order to not extract a random sample of junk tweets, some cleanup is done for each game. This did not take place when creating the histograms seen at the previous section. Even though some junk is removed, the deleted tweets are still somewhat relevant to the games, such as tweets posted by people as a response to an article or video.

Note for exporting tweets from the db: export using the open documents spreadsheet format, because it preserves all characters (including unicode strange cat faces and such), and the tweet ids. 

#### Armikrog
The tweets containing '#gamingnews Armikrog release date announced for August' were deleted because they were spam. Same goes for tweets containing 'incredible claymation world: Classic adventure gaming gets a claymation'. Also for tweets referring to 'Armikrog release date' (~80 tweets except one were all made by bots or game news sites) or 'Armikrog release is pushed into September' (30 identical tweets).

#### Legends of Eisenwald
Tweets containing `скачать` were deleted because that means `download` in russian and they were all links to illegal torrent sites, posted automatically by russian pages. Tweets containing `legends of eisenwald review` were also deleted - 325 in total - because the vast majority (all observable by a scroll through) simply followed a link towards a review website, and did not provide any useful information apart from additional hashtags such as #review, #gamespot and #fun...

Most other tweets in the eisenwald set contained links to other sites, and a variety of patterns such as `Is Legends of Eisenwald the new Planescape Torment? <link>` or `Legends of Eisenwald Full PC game Free Download`. These were not removed, even though they most likely oversaturate the sample of tweets - the total amount of tweets seems full of this type of tweet and it would be easy to go through all of them by just skimming a long list sorted alphabetically by tweet, and it would be obvious which tweets are user-made and which are automatic spam.

#### Everybody's gone to the rapture
All of the tweets containing `@Youtube` are spammed by youtube being linked somehow to twitter using some app. There were 859 tweets deleted containing this. Interesting note is that the tweets containing `#EverybodysGoneToTheRapture #PS4share http` seemed like spam, because there were many in a row with that format which only contained a link at the end, however many were not spam at close inspection, because they contained this phrase at the end after some useful content, such as the tweet `Very original and interesting game, bit of a grind for the platinum tho #EverybodysGoneToTheRapture #PS4share http://t.co/ezMZiVzlcX`. However, the tweets starting with that phrase were not useful, therefore all 54 of them were discarded. Note that this type of close inspection has been done for all deleted tweets in all games.

94 more tweets were deleted containing `#gamingnews Watch Everybody's Gone To The Rapture's mysterious launch trailer - http` - all identical except for the link. Out of the 9000 remaining tweets, there are too many patterns of too few tweets each to clean up in a reasonable timeframe, so the rest were left intact.

#### Formula 1
The random sample seemed to contain relevant tweets at first glance, so no cleanup was done here.

#### Madden 16

No cleanup, seems like a lot of relevant tweets.

#### Paddington

No cleanup, seems like 99% spam.

#### RoryMcIlroy

No cleanup, seems like most tweets are about golf, also apparent from the distribution of tweets which has nothing to do with the release date of the game.

#### The Swindle

No cleanup.

#### Zombie Vikings

No cleanup.

#### Rise of Incarnates

No export - data is not in the database, only in backup text file, due to some errors. Will be done at some point.

### Recovering deleted tweets from backups

An idea came before cleaning up all the other tables like the first ones, that instead of deleting tweets from them it would be better to mark them as `relevant` or `not relevant` with some kind of binary tag. Therefore all deleted data had to be restored, and it was done with the following query:

```sql
set @backuptable = 'riseofincarnates3';
set @targettable = 'riseofincarnates2';
set @statement = concat('insert into ',@targettable,' select ',@backuptable,'.* from ',@backuptable,' left outer join ',@targettable,' on ',@targettable,'.id = ',@backuptable,'.id where ',@targettable,'.id is null');
prepare statement1 from @statement;
execute statement1;
deallocate prepare statement1;
```

Query based on a [question on stackoverflow about left outer join](http://stackoverflow.com/questions/13053052/insert-missing-records-from-one-table-to-another-using-mysql) and another about [variable table names and prepared statements] ( http://stackoverflow.com/questions/8809943/how-to-select-from-mysql-where-table-name-is-variable). The backup table contains tweets that are no longer in target table, but target table might contain new tweets which should not be lost. The query adds the missing tweets from backup into target. The prepare statement part of the query is there in order to simplify performing it for multiple tables, and being able to use a table name as a variable.

After all the backup data was recovered into all the tables, the structure of the tables was altered by adding an extra column called `ignore`, and which by default is zero. When a row or a set of rows is meant to be ignored, this column is set to nonzero, meaning this column should be ignored, though it is not actually deleted.

The query below adds the column to a table.
```sql
ALTER TABLE `eisenwald2` ADD `ignore` BOOLEAN NOT NULL ;
```

The query below updates a set of tweets to be ignored.
```sql
UPDATE `twitter`.`armikrog2` SET `ignore` = '1' WHERE tweet LIKE '%Armikrog, el sucesor espiritual de Neverhood%'
```

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
