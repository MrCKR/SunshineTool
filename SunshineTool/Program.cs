// See https://aka.ms/new-console-template for more information

Util.Log(Util.ExePath);
Util.Log(Util.AppDir);
// 解析参数
Util.ParseArgs(args);
//没有参数，设置开机自启
//同时切换回主屏幕
if (Util.Args.Count == 0)
{
    //等待10秒
    //Thread.Sleep(10000);
    Util.Log("没有参数，设置开机自启");
    Util.SetStartup();
    Util.Log("切换回主屏幕");
    Util.Undo();
    Util.Log("默认启动" + DateTime.Now);
    return;
}

if (Util.ArgGetString(ArgType.r, string.Empty) == "open")
{
    Util.Log("打开");
    Util.Do();
}

if (Util.ArgGetString(ArgType.r, string.Empty) == "close")
{
    Util.Log("关闭");
    Util.Undo();
}

Util.Log("完成");
