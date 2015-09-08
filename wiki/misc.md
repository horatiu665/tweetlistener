# Misc
This section contains what does not belong anywhere else: outcast documentation. Also stuff that is not proper enough to deserve its proper section. 

As soon as relevant stuff starts forming here, please move it to a proper section linked from somewhere.

## Making sure tweets stay online

One of the problems with the gathering process is related to the ability to check that the program is running properly and avoid crashes. The solution to this comes in the form of e-mails: at every disconnect which is longer than a customizable interval of 6 hours, an e-mail is sent to the author with the name of the game and the disconnection date. This tells us whether there are any problems, provided that the computer is still running and connected to the internet. However, if the computer goes offline for any reason, or there is a crash, it obviously cannot inform anyone of this, therefore an e-mail is sent every 24 hours (arbitrarily chosen) to inform the author that the programs are successfully running. If there is no e-mail received in 24 hours, there is a problem and the program must be restarted or at least verified.

## Optimizing tweet data for storage and analysis

*“Since in our cases we’re going to be re-broadcasting this out at an extremely high rate to all the streaming servers, we want to trim this down to conserve bandwidth. Thus we create a new JSON blob from a hash containing just the bare minimum to construct a tweet: tweet ID, text, and author info (permalink URLs are predictable and can be recreated with this info). This reduces the size by 10-20x.”* - [Emojitracker post on Medium.com](https://medium.com/@mroth/how-i-built-emojitracker-179cfd8238ac)

## Misc DB get n-th row query challenge
I wanted to query the DB for the n-th row, defined with the assumption that the rows are ordered arbitrarily. This is to be used for the TweetViewer when creating graphs of the tweets.

Solution is here:

http://dba.stackexchange.com/questions/56168/select-every-n-th-row-dont-pull-the-entire-table 

First of all, a query which returns all dates in the table, with the tweet amount up to that date is 

```
SELECT created_at, (SELECT COUNT(*) FROM `json` t1 WHERE t1.created_at <= t2.created_at ) AS count FROM `json` t2 
```

However, this is extremely expensive and takes way too long even when asking for maximum 500 rows (by adding a limit 500 to the query).

this is the correct query, adapted for the json table:
```
set @row:=-1; SELECT json.* FROM json INNER JOIN ( SELECT id FROM ( SELECT @row:=@row+1 AS rownum, id FROM ( SELECT id FROM json ORDER BY id ) AS sorted ) as ranked WHERE rownum % 3 = 0 ) AS subset ON subset.id = json.id
```

In phpmyadmin, after running it, it shows the code only from the first occurrence of “SELECT”, but the first part is very important.

This query takes every 3 rows. Can be changed at “rownum % 3”

More work must be done to make it work in browser, right now “Query does not work” some problem in php.

