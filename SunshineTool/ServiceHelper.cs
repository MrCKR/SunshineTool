using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.IO;
using System.Text;

// =======================
// 安装 / 卸载 工具类 + 关机计划任务
// =======================
public static class ServiceHelper
{
    public const string ServiceNameConst = "SunshineToolService";

    public static bool IsServiceInstalled(string name)
    {
        try
        {
            using var sc = new ServiceController(name);
            _ = sc.Status; // 若未安装，这里会抛异常
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void InstallService()
    {
        try
        {
            // 单文件发布下 Assembly.Location 可能为空，改用实际进程路径
            string exePath = Util.ExePath;
            Util.Log($"准备安装服务，binPath={exePath}");

            bool IsAdmin()
            {
                try
                {
                    var id = System.Security.Principal.WindowsIdentity.GetCurrent();
                    var principal = new System.Security.Principal.WindowsPrincipal(id);
                    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }
                catch
                {
                    return false;
                }
            }

            if (IsAdmin())
            {
                // 管理员上下文：直接运行 sc，并重定向输出
                var psi = new ProcessStartInfo("sc.exe", $"create {ServiceNameConst} binPath= \"{exePath}\" start= auto type= own obj= LocalSystem")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit();
                string stdout = proc?.StandardOutput.ReadToEnd() ?? string.Empty;
                string stderr = proc?.StandardError.ReadToEnd() ?? string.Empty;
                Util.Log("sc create stdout: " + stdout.Trim());
                Util.Log("sc create stderr: " + stderr.Trim());
                Util.Log("sc create exit code: " + (proc?.ExitCode ?? -1));
            }
            else
            {
                // 非管理员上下文：触发 UAC，通过 cmd.exe /c 调用 sc
                var psi = new ProcessStartInfo("cmd.exe", $"/c sc create {ServiceNameConst} binPath= \"{exePath}\" start= auto type= own obj= LocalSystem")
                {
                    Verb = "runas",
                    UseShellExecute = true
                };
                Process.Start(psi)?.WaitForExit();
                Util.Log("已通过 UAC 调用 cmd.exe 执行 sc create");
            }

            // 校验安装结果
            if (IsServiceInstalled(ServiceNameConst))
            {
                Util.Log("服务安装成功！");
            }
            else
            {
                Util.Log("服务安装失败：请在管理员终端手动执行以下命令以安装服务");
                Util.Log($"sc create {ServiceNameConst} binPath= \"{exePath}\" start= auto type= own obj= LocalSystem");
            }
        }
        catch (Exception ex)
        {
            Util.Log("安装服务失败：" + ex);
        }
    }

    public static void UninstallService()
    {
        try
        {
            // 再卸载服务
            var psi = new ProcessStartInfo("sc.exe", $"delete {ServiceNameConst}")
            {
                Verb = "runas",
                UseShellExecute = true
            };
            Process.Start(psi)?.WaitForExit();
            Util.Log("服务卸载命令已执行。");
        }
        catch (Exception ex)
        {
            Util.Log("卸载服务失败：" + ex);
        }
    }
}
