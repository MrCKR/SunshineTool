using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

// =======================
// Windows Service 类
// =======================
public class ScreenSwitchService : ServiceBase
{
    public ScreenSwitchService()
    {
        ServiceName = ServiceHelper.ServiceNameConst;
    }

    protected override void OnStart(string[] args)
    {
        Util.Log("服务启动，执行屏幕切换逻辑...");
        try
        {
            Util.SwitchToMainScreen();
            Util.Log("屏幕切换完成。准备停止服务...");
        }
        catch (Exception ex)
        {
            Util.Log("屏幕切换失败：" + ex);
        }

        // 延迟一点点再停止，防止系统认为服务异常退出
        Task.Delay(2000).ContinueWith(_ => Stop());
    }

    protected override void OnStop()
    {
        Util.Log("服务已停止。");
    }

    // 手动调试模式（非服务运行）
    public void TestRun()
    {
        OnStart(Array.Empty<string>());
        OnStop();
    }
}
