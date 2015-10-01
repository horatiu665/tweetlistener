set /p count="How many folders you want? >1 : "
xcopy "Release" "Release - Copy" /e /i
for /l %%i in (2,1,%count%-1) do (
	xcopy "Release" "Release - Copy (%%i)" /e /i
	)