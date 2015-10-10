xcopy "Release\rawJsonBackup.txt" "Backups\rawJson.txt*"
xcopy "Release - Copy\rawJsonBackup.txt" "Backups\rawJson (1).txt*"
for /l %%i in (2,1,24) do (
	xcopy "Release - Copy (%%i)\rawJsonBackup.txt" "Backups\rawJson (%%i).txt*"
	)