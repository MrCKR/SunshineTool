自用的sunshine工具
因为我觉得基地版有点麻烦,而且在我的使用体验中有些奇奇怪怪的问题,所以我选择使用hdmi诱骗工具,在加上自动切换屏幕,实现串流时电脑屏幕黑屏的功能
使用方法:
1. 购买一个hdmi诱骗工具(几块钱)
2. 安装官方版的sunshine
3. 下载本仓库内容放到任意目录(目录不要有中文或空格),例如就在C盘下
4. 配置app命令
5. 打开应用时执行命令:C:\SunshineTool\sunshine_do.bat --width %width% --height %height% --fps %fps% --steam true
6. 退出应用时执行命令:C:\SunshineTool\sunshine_undo.bat --width "1920" --height "1080" --steam true 
7. 上面的C:\SunshineTool\sunshine_undo.bat应该修改为你自己的目录
8. 上面的--steam true 代表启动或退出steam大屏幕模式,不需要就修改为--steam false
9. 退出命令中--width "1920" --height "1080" 代表退出后恢复的分辨率,根据自己的主屏幕分辨率修改数字
