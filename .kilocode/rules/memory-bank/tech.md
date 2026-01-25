# 技术栈

## 编程语言
- C#

## 运行环境
- .NET 8.0（目标框架参见 [SunshineTool.csproj](SunshineTool/SunshineTool.csproj)）
- Windows 10/11 桌面环境

## 核心功能
- 通过 Windows API 进行屏幕控制 [DisplayUtil.SwitchDisplayMode()](SunshineTool/DisplayUtil.cs:45)、[DisplayUtil.ChangeResolution()](SunshineTool/DisplayUtil.cs:71)
- Windows 服务运行模式 [ScreenSwitchService](SunshineTool/ScreenSwitchService.cs)
- 交互式命令行模式入口 [Program.cs](SunshineTool/Program.cs)

## 依赖库与系统组件
- NuGet：System.ServiceProcess.ServiceController 9.0.10（参见 [PackageReference](SunshineTool/SunshineTool.csproj:16)）
- P/Invoke：user32.dll [SetDisplayConfig](SunshineTool/DisplayUtil.cs:31)、[EnumDisplaySettings](SunshineTool/DisplayUtil.cs:35)、[ChangeDisplaySettings](SunshineTool/DisplayUtil.cs:37)
- 运行时组件：SC.exe（服务安装/卸载，参见 [ServiceHelper.InstallService()](SunshineTool/ServiceHelper.cs:31)、[ServiceHelper.UninstallService()](SunshineTool/ServiceHelper.cs:52)）

## 构建与发布
- 项目配置：
  - PublishSingleFile=true [SunshineTool.csproj](SunshineTool/SunshineTool.csproj:8)
  - PublishTrimmed=true [SunshineTool.csproj](SunshineTool/SunshineTool.csproj:9)
  - SelfContained=true [SunshineTool.csproj](SunshineTool/SunshineTool.csproj:10)
  - RuntimeIdentifiers=win-x64;win-x86 [SunshineTool.csproj](SunshineTool/SunshineTool.csproj:11)
  - EnableCompressionInSingleFile=true [SunshineTool.csproj](SunshineTool/SunshineTool.csproj:12)
- 发布命令建议：
  - [dotnet publish](SunshineTool/Program.cs:4) 示例：dotnet publish -c Release -r win-x64 --self-contained true -o ./publish

## 项目结构
- 入口与模式切换：[Program.cs](SunshineTool/Program.cs)
- 服务管理：[ServiceHelper.cs](SunshineTool/ServiceHelper.cs)
- Windows 服务类：[ScreenSwitchService.cs](SunshineTool/ScreenSwitchService.cs)
- 显示控制：[DisplayUtil.cs](SunshineTool/DisplayUtil.cs)
- 工具与配置：[Util.cs](SunshineTool/Util.cs)、[Cfg.cs](SunshineTool/Cfg.cs)
- 项目文件：[SunshineTool.csproj](SunshineTool/SunshineTool.csproj)

## 架构模式
- 模块化设计，单一职责原则
- 服务与交互双运行模式 [ServiceBase.Run(new ScreenSwitchService())](SunshineTool/Program.cs:15)

## 性能与可靠性考虑
- 启动延时与重试：服务在 [ScreenSwitchService.OnStart()](SunshineTool/ScreenSwitchService.cs:16) 中实现延时与重试逻辑，但登录前安全桌面阶段调用 [SetDisplayConfig](SunshineTool/DisplayUtil.cs:31) 返回 5（Access Denied），切换无法生效。
- 错误处理与日志：在 Release 服务模式下启用文件日志 [Util.Log()](SunshineTool/Util.cs:193)，用于登录前诊断；分辨率恢复逻辑可根据需要启用或跳过 [DisplayUtil.ChangeResolution()](SunshineTool/DisplayUtil.cs:71)。
- 关机阶段：已尝试通过计划任务触发 `r=close`，安装/卸载入口见 [ServiceHelper.InstallService()](SunshineTool/ServiceHelper.cs:31)、[ServiceHelper.UninstallService()](SunshineTool/ServiceHelper.cs:52)，但在目标环境下未能执行，当前“关机服务”不可用。
- 当前状态：项目改动已冻结，保留现有实现与文档以供后续参考。
