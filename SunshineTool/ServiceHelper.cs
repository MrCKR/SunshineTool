using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace DisplayFixService
{
    // =======================
    // 安装 / 卸载 工具类
    // =======================
    public static class ServiceHelper
    {
        public const string ServiceNameConst = "ScreenSwitchService";

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
                string exePath = Assembly.GetExecutingAssembly().Location;
                var psi = new ProcessStartInfo("sc.exe", $"create {ServiceNameConst} binPath= \"{exePath}\" start= auto")
                {
                    Verb = "runas",
                    UseShellExecute = true
                };
                Process.Start(psi)?.WaitForExit();
                Util.Log("服务安装命令已执行。");
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

}

