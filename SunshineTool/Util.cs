using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text.Json;

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
            cfg = new Cfg();
            File.WriteAllText(cfgPath, JsonSerializer.Serialize(cfg));
        }
        return cfg;
    }

    //设置开机自启
    public static void SetStartup()
    {
        // 注册表路径，一般用户的启动项
        string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        // 打开注册表
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey, true))
        {
            // 添加程序路径到注册表
            if (key != null)
            {
                key.SetValue(AppKey, ExePath);
            }
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
        Console.WriteLine("解析参数");
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach (string arg in args)
        {
            arg.Trim();
            arg.ToLower();
            // 假设参数格式为 key=value
            string[] parts = arg.Split('=');
            if (parts.Length != 2) continue;
            Console.WriteLine($"{parts[0]}={parts[1]}");
            result[parts[0]] = parts[1];
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
        Console.WriteLine("切换拓展屏");
        ChangeDisplay.SwitchDisplayMode(3);
        //等待100毫秒
        Console.WriteLine("等待100毫秒");
        Task.Delay(100);
        //设置分辨率
        var x = ArgGetInt(ArgType.x, 1920);
        var y = ArgGetInt(ArgType.y, 1080);
        var fps = ArgGetInt(ArgType.fps, 60);
        Console.WriteLine($"设置分辨率, x={x}, y={y}, fps={fps}");
        ChangeDisplay.ChangeResolution(x, y, fps);
        Console.WriteLine("等待100毫秒");
        Task.Delay(100);
        //开启steam
        if (ArgGetBool(ArgType.steam, false))
        {
            Console.WriteLine("开启steam");
            ShowBigSteam(true);
        }
    }

    public static void Undo()
    {
        //回到主屏幕
        Console.WriteLine("回到主屏幕");
        ChangeDisplay.SwitchDisplayMode(0);
        //等待100毫秒
        Console.WriteLine("等待100毫秒");
        Task.Delay(100);
        //恢复分辨率
        var x = Cfg.MainWidth;
        var y = Cfg.MainHeight;
        var fps = Cfg.MainFps;
        Console.WriteLine($"恢复分辨率, x={x}, y={y}, fps={fps}");
        ChangeDisplay.ChangeResolution(x, y, fps);
        Console.WriteLine("等待100毫秒");
        Task.Delay(100);
        //关闭steam
        if (ArgGetBool(ArgType.steam, false))
        {
            Console.WriteLine("关闭steam");
            ShowBigSteam(false);
        }
    }

    public static void Cmd(string cmd)
    {
        Console.WriteLine($"执行命令:{cmd}");
        Process.Start("cmd.exe", $"/c {cmd}");
    }

}