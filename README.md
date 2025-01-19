自用的sunshine工具
因为我觉得基地版有点麻烦,而且在我的使用体验中有些奇奇怪怪的问题,所以我选择使用hdmi诱骗工具,在加上自动切换屏幕,实现串流时电脑屏幕黑屏的功能
使用方法:
1. 购买一个hdmi诱骗工具(几块钱),插在显卡上
2. 右键桌面打开显示设置,确认自己的主屏幕为正常显示时的那个屏幕,不要将hdmi诱骗工具的屏幕设置为主屏幕,不要将hdmi诱骗工具的屏幕设置为主屏幕,不要将hdmi诱骗工具的屏幕设置为主屏幕
3. 安装官方版的sunshine
4. 下载release放到任意目录(目录不要有中文或空格),例如就在C盘下
5. 配置sunshine的app命令
6. 打开应用时执行命令:C:\SunshineTool\SunshineTool.exe r=open x= %SUNSHINE_CLIENT_WIDTH% y=%SUNSHINE_CLIENT_HEIGHT% fps=%SUNSHINE_CLIENT_FPS% steam=true
7. 退出应用时执行命令:C:\SunshineTool\SunshineTool.exe r=close steam=true
8. 上面的C:\SunshineTool\SunshineTool.exe应该修改为你自己的目录
9. 上面的 steam=true 代表启动或退出steam大屏幕模式,不需要就修改为 steam=true 在app的配置中自己选择,如果配置steam大屏幕app时,记得把sunshine原本的命令删除
10. 同文件夹下的cfg.json是配置文件,里面的参数代表结束串流时,主屏幕默认的分辨率,帧率,记得安装自己的显示器参数修改
11. 如果出现了黑屏的情况,要么将hdmi诱骗工具拔了,要么使用键盘 win+p 等半秒钟再回车 ,这是切换屏幕模式的快捷键,多试几次
