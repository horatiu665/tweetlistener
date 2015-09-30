 start "WPFTwitter" "WPFTwitter.exe" ^
 /phpPostPath "http://localhost/hhh/tweetlistenerweb/php/saveJson.php" ^
 /logPath "log.txt" ^
 /textFileDbPath "rawJsonBackup.txt" ^
 /dbTableName "gametest1" ^
 /dbDestination "mysql" ^
 /dbConnectionStr "server=localhost;userid=root;password=;database=testing" ^
 /saveToDatabase 1 ^
 /saveToTextFile 0 ^
 /saveToRam 0 ^
 /outputEventCounters 0 ^
 /outputDatabaseMessages 0 ^
 /logEveryJson 0 ^
 /emailDisco 06:00:00 ^
 /emailConnected 23:59:59 ^
 /onlyEnglish 1 ^
 /onlyWithHashtags 0 ^
 /credentials 7 ^
 /keywords "#callofduty,#cod,#ps4,#pc,#xbox" ^
 /windowTitle "Game test 1" ^
 /startStream 0

  start "WPFTwitter2" "WPFTwitter2.exe" ^
 /phpPostPath "http://localhost/hhh/tweetlistenerweb/php/saveJson.php" ^
 /logPath "log.txt" ^
 /textFileDbPath "rawJsonBackup.txt" ^
 /dbTableName "gametest2" ^
 /dbDestination "php" ^
 /dbConnectionStr "server=localhost;userid=root;password=;database=twitter" ^
 /saveToDatabase 0 ^
 /saveToTextFile 1 ^
 /saveToRam 1 ^
 /outputEventCounters 1 ^
 /outputDatabaseMessages 0 ^
 /logEveryJson 0 ^
 /emailDisco 06:00:00 ^
 /emailConnected 23:59:59 ^
 /onlyEnglish 1 ^
 /onlyWithHashtags 0 ^
 /credentials 3 ^
 /keywords "mysql, gingernigger, google spartan" ^
 /windowTitle "Game test 2222222222222222222222222222222222222222" ^
 /startStream 0
