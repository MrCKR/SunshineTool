// See https://aka.ms/new-console-template for more information

Console.WriteLine(Util.ExePath);
Console.WriteLine(Util.AppDir);
// 解析参数
Util.ParseArgs(args);
//没有参数，设置开机自启
//同时切换回主屏幕
if (Util.Args.Count == 0)
{
    Console.WriteLine("没有参数，设置开机自启");
    Util.SetStartup();
    Console.WriteLine("切换回主屏幕");
    ChangeDisplay.SwitchDisplayMode(0);
    return;
}

if (Util.ArgGetString(ArgType.r, string.Empty) == "open")
{
    Console.WriteLine("打开");
    Util.Do();
}

if (Util.ArgGetString(ArgType.r, string.Empty) == "close")
{
    Console.WriteLine("关闭");
    Util.Undo();
}

Console.WriteLine("完成");
