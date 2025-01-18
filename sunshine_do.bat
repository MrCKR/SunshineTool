::C:\SunshineTool\sunshine_do.bat --width %width% --height %height% --fps %fps% --steam true
::C:\SunshineTool\sunshine_undo.bat --steam true
::@echo off
::定义默认值
set width=1920
set height=1080
set fps=60
set steam="false"
set Script_Dir=%~dp0


:: ***遍历输入参数
:parseargs
if "%~1"=="" goto endparseargs
if "%~1"=="--width" (
    set width=%~2
    shift
)
if "%~1"=="--height" (
    set height=%~2
    shift
)
if "%~1"=="--fps" (
    set fps=%~2
    shift
)
if "%~1"=="--steam" (
    set steam=%~2
    shift
)
shift
goto parseargs

:endparseargs
:: ***遍历输入参数

::切换到外部显示器
::后面的internal（仅在屏幕1显示） 可以换成 external（仅在屏幕2显示）,extend（扩展屏幕）,clone(复制两个屏幕) 
DisplaySwitch.exe /external
::延迟2秒,上面的切换显示器不会马上完成
timeout /t 2 /nobreak > nul
::修改分辨率,帧率
%Script_Dir%qres.exe /x:%width% /y:%height% /r:%fps%
::打开steam
if %steam%==true (
    start steam://open/bigpicture
)