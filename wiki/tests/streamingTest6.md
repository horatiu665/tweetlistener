# New test 10-02-2015
- Filter: Evolve (release date is today woohoo)
- update to the code: added a small log to the big log, where only important messages are saved, such as disconnects and limit rates
- 8 days test resulted in ~191000 tweets, ran from `2015-02-10 19:15:00` to `2015-02-18 16:59:19` for a total of 7 days, 21 hours, 44 minutes and 19 seconds
- the program stopped at the 35th disconnection with the error message “Stream was not readable”. Likely cause is that the program attempts to read stream before it is successfully started, even though it reports that it has started successfully.
- Noob error: log stopped at first disconnect, so most of the log has been lost. (35 reconnections and 2 limit messages). First disconnect happened at 19:00 on 10-02-2015, after 35000 tweets, far less than total amount.
- Based on the console log, there were 2 limit messages, but I cannot know what values they showed because log stopped.
