using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text.Json;
using WinAPI;

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
    public static Dictionary<string, string> Args { get; private set; }
    public static Cfg Cfg => LoadConfig();
    private const string AppKey = "alpsckr_sunshine_tool";



    public static Cfg LoadConfig()
    {
        Cfg cfg = null;
        var cfgPath = Path.Combine(AppDir, "cfg.json");
        try
        {
            if (File.Exists(cfgPath))
            {
                cfg = JsonSerializer.Deserialize<Cfg>(File.ReadAllText(cfgPath));
            }
        }
        catch (Exception e)
        {

        }

        if (cfg == null)
        {
            cfg = InitCfg();
            File.WriteAllText(cfgPath, JsonSerializer.Serialize(cfg));
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

    //设置开机自启
    public static void SetStartup()
    {
        try
        {
            Util.Log("设置开机自启");
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (registryKey == null)
            {
                Util.Log("注册表不存在，创建注册表");
                registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }
            if (registryKey != null)
            {
                registryKey.SetValue(AppKey, ExePath);
            }
        }
        catch (System.Exception e)
        {
            Util.Log("设置开机自启失败,错误:" + e.Message);
        }
    }

    // 取消开机自启
    public static void RemoveStartup()
    {
        string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey, true))
        {
            if (key != null)
            {
                key.DeleteValue(AppKey, false); // false 表示如果不存在则不引发异常
            }
        }
    }

    // 解析参数
    public static void ParseArgs(string[] args)
    {
        Util.Log("解析参数");
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach (string arg in args)
        {
            arg.Trim();
            arg.ToLower();
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

    public static void Do()
    {
        //切换拓展屏
        Util.Log("切换拓展屏");
        DisplayUtil.SwitchDisplayMode(3);
        //等待100毫秒
        Util.Log("等待100毫秒");
        Task.Delay(100);
        //设置分辨率
        var x = ArgGetInt(ArgType.x, 1920);
        var y = ArgGetInt(ArgType.y, 1080);
        var fps = ArgGetInt(ArgType.fps, 60);
        Util.Log($"设置分辨率, x={x}, y={y}, fps={fps}");
        DisplayUtil.ChangeResolution(x, y, fps);
        Util.Log("等待100毫秒");
        Task.Delay(100);
        //开启steam
        if (ArgGetBool(ArgType.steam, false))
        {
            Util.Log("开启steam");
            ShowBigSteam(true);
        }
    }

    public static void Undo()
    {
        //回到主屏幕
        Util.Log("回到主屏幕");
        DisplayUtil.SwitchDisplayMode(0);
        //等待100毫秒
        Util.Log("等待100毫秒");
        Task.Delay(100);
        //恢复分辨率
        var x = Cfg.MainWidth;
        var y = Cfg.MainHeight;
        var fps = Cfg.MainFps;
        Util.Log($"恢复分辨率, x={x}, y={y}, fps={fps}");
        DisplayUtil.ChangeResolution(x, y, fps);
        Util.Log("等待100毫秒");
        Task.Delay(100);
        //关闭steam
        if (ArgGetBool(ArgType.steam, false))
        {
            Util.Log("关闭steam");
            ShowBigSteam(false);
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
#if !DEBUG
        return;
#endif
        var logPath = Path.Combine(AppDir, "log.log");
        if (!File.Exists(logPath))
        {
            using (var fs = File.Create(logPath)) { }
        }
        File.AppendAllText(logPath, msg + "\r\n");
    }

}