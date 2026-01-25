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
        // 允许接收系统关机事件
        this.CanShutdown = true;
    }

    protected override void OnStart(string[] args)
    {
        Util.Log("服务已启动。");
        // 启动异步任务执行屏幕切换
        Task.Run(async () => await SwitchToMainScreen());
    }

    protected override void OnStop()
    {
        Util.Log("服务已停止。");
    }

    // 系统关机时切回主屏
    protected override void OnShutdown()
    {
        Util.Log("系统关机通知，准备切回主屏...");
        try
        {
            // 关机阶段尽量快速，不做长等待
            DisplayUtil.SwitchDisplayMode(0);
            Util.Log("关机前拓扑切回 INTERNAL 已调用");
        }
        catch (Exception ex)
        {
            Util.Log("关机前切回主屏异常: " + ex.Message);
        }
    }

    async Task SwitchToMainScreen()
    {
        Util.Log("服务启动，准备执行屏幕切换...");
        // 等待显示系统就绪（8秒）
        await Task.Delay(8000);
        // 重试机制（最多5次）
        int maxRetries = 5;
        bool success = false;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                Util.Log($"尝试切换屏幕 (第{i + 1}次)...");
                await Util.SwitchToMainScreen();
                Util.Log("屏幕切换成功!");
                success = true;
                break;
            }
            catch (Exception ex)
            {
                Util.Log($"第{i + 1}次尝试失败: {ex.Message}");
                if (i < maxRetries - 1)
                {
                    await Task.Delay(1000); // 失败后等待1秒再重试
                }
            }
        }

        if (!success)
        {
            Util.Log("警告：所有尝试均失败，请检查日志和硬件连接");
        }

        // 延迟一点点再停止，防止系统认为服务异常退出
        await Task.Delay(2000);
        Stop();
    }
}
