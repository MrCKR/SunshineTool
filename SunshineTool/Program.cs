// See https://aka.ms/new-console-template for more information

using System.ServiceProcess;

Util.Log(Util.ExePath);
Util.Log(Util.AppDir);


//如果是服务运行模式(即开机自启动时,非主动调用)
if (!Environment.UserInteractive)
{
    ServiceBase.Run(new ScreenSwitchService());
    return;
}

// 解析参数
Util.ParseArgs(args);

//没有参数，设置开机自启
//同时切换回主屏幕
if (Util.Args.Count == 0)
{
    // ---- 服务安装 ----
    if (!ServiceHelper.IsServiceInstalled(ServiceHelper.ServiceNameConst))
    {
        Util.Log("服务未安装，开始安装...");
        ServiceHelper.InstallService();
        return;
    }
}

if (Util.ArgGetString(ArgType.r, string.Empty) == "uninstall")
{
    ServiceHelper.UninstallService();
    return;
}

if (Util.ArgGetString(ArgType.r, string.Empty) == "open")
{
    Util.Log("打开");
    await Util.Do();
}

if (Util.ArgGetString(ArgType.r, string.Empty) == "close")
{
    Util.Log("关闭");
    await Util.Undo();
}

Util.Log("完成");
