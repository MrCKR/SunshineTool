# SunshineTool 项目简介

## 项目概述
SunshineTool 是一个 Windows 服务工具，用于自动管理屏幕显示模式，特别是在多显示器环境下实现屏幕切换功能。

## 主要目标
- 实现屏幕显示模式的自动化切换
- 支持开机自启动服务
- 提供命令行参数控制
- 确保系统稳定性和可靠性

## 关键功能
1. **屏幕模式切换**：支持主屏、双屏复制、双屏扩展等多种显示模式
2. **服务管理**：支持服务的安装、卸载和状态检查
3. **参数配置**：通过命令行参数进行灵活配置
4. **错误处理**：完善的错误检测和重试机制

## 技术栈
- **编程语言**：C#
- **运行环境**：.NET 8.0，Windows 10/11 桌面环境，目标框架参见 [SunshineTool.csproj](SunshineTool/SunshineTool.csproj)
- **核心功能**：使用 Windows API 进行屏幕控制，见 [DisplayUtil.SwitchDisplayMode()](SunshineTool/DisplayUtil.cs:45)、[DisplayUtil.ChangeResolution()](SunshineTool/DisplayUtil.cs:71)
- **依赖库**：System.ServiceProcess, System.Diagnostics, System.Runtime.InteropServices

## 重要限制与现状
- 登录前切换失败：安全桌面阶段调用 [SetDisplayConfig](SunshineTool/DisplayUtil.cs:31) 返回 5（Access Denied），即使延时与重试也无法生效，入口路径为 [ScreenSwitchService.OnStart()](SunshineTool/ScreenSwitchService.cs:16) → [Util.SwitchToMainScreen()](SunshineTool/Util.cs:58) → [DisplayUtil.SwitchDisplayMode()](SunshineTool/DisplayUtil.cs:45)。
- 关机触发无效：基于计划任务的关机事件触发 `r=close` 的方案集成在安装/卸载逻辑 [ServiceHelper.InstallService()](SunshineTool/ServiceHelper.cs:31)、[ServiceHelper.UninstallService()](SunshineTool/ServiceHelper.cs:52)，但在目标环境下未能执行，当前“关机服务”处于不可执行状态。
- 改动冻结：目前不再继续修改该项目代码，保留现状以便后续参考与文档化。

## 项目意义
该工具为多显示器用户提供了便捷的屏幕管理解决方案，特别适用于需要频繁切换显示模式的专业用户，如开发者、设计师等。通过自动化服务，确保系统启动后自动执行屏幕配置，提升用户体验和工作效率。