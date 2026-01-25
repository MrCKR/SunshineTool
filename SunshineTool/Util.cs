using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text.Json;
using System.Threading.Tasks;

public enum ArgType
{
    r,
    x,
    y,
    fps,
    steam,
}

public static class Util
{
    public static string AppDir => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
    public static string ExePath => Process.GetCurrentProcess().MainModule.FileName;
    public static Dictionary<string, string> Args { get; private set; } = new();
    public static Cfg Cfg => LoadConfig();

    public static Cfg LoadConfig()
    {
        Cfg cfg = null;
        var cfgPath = Path.Combine(AppDir, "cfg.json");
        try
        {
            if (File.Exists(cfgPath))
            {
                var json = File.ReadAllText(cfgPath);
                cfg = JsonSerializer.Deserialize(json, CfgJsonContext.Default.Cfg);
            }
        }
        catch (Exception e)
        {
            Util.Log("读取配置失败: " + e.Message);
        }

        if (cfg == null)
        {
            cfg = InitCfg();
            try
            {
                var jsonOut = JsonSerializer.Serialize(cfg, CfgJsonContext.Default.Cfg);
                File.WriteAllText(cfgPath, jsonOut);
            }
            catch (Exception e)
            {
                Util.Log("写入配置失败: " + e.Message);
            }
        }
        return cfg;
    }

    static Cfg InitCfg()
    {
        var cfg = new Cfg();
        var curResolution = DisplayUtil.GetCurResolution();
        cfg.MainWidth = curResolution.Item1;
        cfg.MainHeight = curResolution.Item2;
        cfg.MainFps = curResolution.Item3;
        return cfg;
    }

    public static async Task SwitchToMainScreen()
    {
        await Util.Undo();
    }

    // 解析参数
    public static void ParseArgs(string[] args)
    {
        Util.Log("解析参数");
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach (string t in args)
        {
            var arg = t.Trim().ToLower();
            // 假设参数格式为 key=value
            string[] parts = arg.Split('=');
            if (parts.Length != 2) continue;
            string key = parts[0];
            string value = parts[1];

            // 检查值是否包含环境变量格式
            if (value.StartsWith("%") && value.EndsWith("%"))
            {
                string envVar = value.Trim('%'); // 提取环境变量名
                string envValue = Environment.GetEnvironmentVariable(envVar); // 获取环境变量值
                if (envValue != null)
                {
                    value = envValue; // 替换为环境变量的实际值
                }
            }
            Util.Log($"{key}={value}");
            result[key] = value;
        }
        Args = result;
    }

    // 扩展方法，用于获取参数值
    public static string ArgGetString(ArgType type, string defaultValue)
    {
        var key = type.ToString().ToLower();
        if (Args.ContainsKey(key))
        {
            return Args[key];
        }
        return defaultValue;
    }

    // 扩展方法，用于获取参数值为整数
    public static int ArgGetInt(ArgType type, int defaultValue)
    {
        var key = type.ToString().ToLower();
        if (Args.ContainsKey(key))
        {
            if (int.TryParse(Args[key], out int value))
            {
                return value;
            }
        }
        return defaultValue;
    }

    // 扩展方法，用于获取参数值为布尔
    public static bool ArgGetBool(ArgType type, bool defaultValue)
    {
        var key = type.ToString().ToLower();
        if (Args.ContainsKey(key))
        {
            if (bool.TryParse(Args[key], out bool value))
            {
                return value;
            }
        }
        return defaultValue;
    }

    public static void ShowBigSteam(bool show)
    {
        var arg = show ? "open" : "close";
        arg = $"start steam://{arg}/bigpicture";
        Cmd(arg);
    }

    public static async Task Do()
    {
        //切换拓展屏
        Util.Log("切换拓展屏");
        DisplayUtil.SwitchDisplayMode(3);
        //等待100毫秒
        Util.Log("等待100毫秒");
        await Task.Delay(100);
        //设置分辨率
        var x = ArgGetInt(ArgType.x, 1920);
        var y = ArgGetInt(ArgType.y, 1080);
        var fps = ArgGetInt(ArgType.fps, 60);
        Util.Log($"设置分辨率, x={x}, y={y}, fps={fps}");
        DisplayUtil.ChangeResolution(x, y, fps);
        Util.Log("等待100毫秒");
        await Task.Delay(100);
        //开启steam
        if (ArgGetBool(ArgType.steam, false))
        {
            Util.Log("开启steam");
            ShowBigSteam(true);
        }
    }

    public static async Task Undo()
    {
        try
        {
            // 回到主屏幕（拓扑 INTERNAL）
            Util.Log("回到主屏幕");
            DisplayUtil.SwitchDisplayMode(0);
            Util.Log("拓扑切换到 INTERNAL 完成，准备等待稳定");

            // 增加等待以保证图形栈稳定
            Util.Log("等待1000毫秒");
            await Task.Delay(1000);

            // 恢复分辨率
            var x = Cfg.MainWidth;
            var y = Cfg.MainHeight;
            var fps = Cfg.MainFps;
            Util.Log($"恢复分辨率, x={x}, y={y}, fps={fps}");
            DisplayUtil.ChangeResolution(x, y, fps);
            
            // 追加短暂等待
            Util.Log("等待500毫秒");
            await Task.Delay(500);

            // 关闭 steam（可选）
            if (ArgGetBool(ArgType.steam, false))
            {
                Util.Log("关闭steam");
                ShowBigSteam(false);
            }
        }
        catch (Exception ex)
        {
            Util.Log("Undo 执行异常: " + ex);
        }
    }

    public static void Cmd(string cmd)
    {
        Util.Log($"执行命令:{cmd}");
        Process.Start("cmd.exe", $"/c {cmd}");
    }

    public static void Log(string msg)
    {
        msg = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {msg}";
        Console.WriteLine(msg);

        bool shouldWrite = true;
#if !DEBUG
        // Release 构建：在服务模式（非交互）下写入文件日志；交互模式可仅控制台
        shouldWrite = !Environment.UserInteractive;
#endif
        if (!shouldWrite) return;

        var logPath = Path.Combine(AppDir, "log.log");
        try
        {
            if (!File.Exists(logPath))
            {
                using var _ = File.Create(logPath);
            }
            File.AppendAllText(logPath, msg + "\r\n");
        }
        catch
        {
            // 避免服务模式因日志写入报错而中断
        }
    }

}
