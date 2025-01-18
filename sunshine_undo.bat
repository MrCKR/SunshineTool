::@echo off
::定义默认值
set width=2560
set height=1080
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
if "%~1"=="--steam" (
    set steam=%~2
    shift
)
shift
goto parseargs

:endparseargs
:: ***遍历输入参数

::切换到主显示器
DisplaySwitch.exe /internal
::延迟2秒,上面的切换显示器不会马上完成
timeout /t 2 /nobreak > nul
::修改分辨率,最佳帧率
%Script_Dir%\qres.exe /X:%width% /Y:%height% /r:-1
::打开steam
if %steam%==true (
    start steam://close/bigpicture
)