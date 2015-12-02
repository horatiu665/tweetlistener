REM delete the logs and rawBackups from the Release folder
del "Release\log*.txt"
del "Release\rawJsonBackup*.txt"
REM zip the Release folder
REM rename the .zip to TweetListener2 %current date%
7za a -tzip "TweetListener2-%DATE%.zip" "Release"
REM copy the .zip into Dropbox and KU folder on the F drive
%systemroot%\system32\xcopy "TweetListener2-%DATE%.zip" "F:\Dropbox\a KU\tweets\tweetlistener builds\" /e /y
%systemroot%\system32\xcopy "TweetListener2-%DATE%.zip" "F:\KU\" /e /y
