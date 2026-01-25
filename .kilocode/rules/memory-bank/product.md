# 产品文档

## 项目目标

SunshineTool 是一个 Windows 服务工具，旨在为多显示器用户提供便捷的屏幕管理解决方案。该工具能够自动管理屏幕显示模式，特别是在多显示器环境下实现屏幕切换功能，提升专业用户的工作效率。

## 解决的问题

1. 多显示器管理复杂：在多显示器环境下，手动切换屏幕模式需要频繁操作，影响工作效率。
2. 开机自启动与登录前行为：确保系统启动后在用户登录前即可恢复到预期主屏显示，避免登录界面显示在错误屏幕上。
3. 参数配置不便：提供灵活的命令行参数控制，满足不同场景的分辨率与刷新率需求。
4. 系统稳定性要求：屏幕切换操作需要稳定可靠，具备延时与重试机制以适应显示系统就绪时间。

## 核心价值

- 自动化管理：通过 Windows 服务实现开机自启动，系统启动后自动执行主屏恢复。
- 灵活控制：支持命令行参数，快速切换外接显示模式与分辨率。
- 稳定可靠：使用 Windows API 调用与重试机制保证操作成功率。
- 专业用户友好：适用于开发者、设计师等多显示器重度用户。

## 默认行为与运行模式

- 服务模式（开机自启动）
  - 服务以 LocalSystem 自动启动，创建方式见 [ServiceHelper.InstallService()](SunshineTool/ServiceHelper.cs:31)。
  - 服务入口 [ScreenSwitchService.OnStart()](SunshineTool/ScreenSwitchService.cs:16) 延时 3 秒并最多重试 3 次，调用 [Util.SwitchToMainScreen()](SunshineTool/Util.cs:58)。
  - 主屏恢复流程：
    - 切换拓扑为 INTERNAL（主屏）[DisplayUtil.SwitchDisplayMode()](SunshineTool/DisplayUtil.cs:45) 使用 type=0。
    - 恢复分辨率为配置的主屏参数 [Cfg.MainWidth](SunshineTool/Cfg.cs:3)、[Cfg.MainHeight](SunshineTool/Cfg.cs:4)、[Cfg.MainFps](SunshineTool/Cfg.cs:5)，调用 [DisplayUtil.ChangeResolution()](SunshineTool/DisplayUtil.cs:71)。

- 交互式模式（命令行）
  - 程序入口 [Program.cs](SunshineTool/Program.cs) 在交互模式下解析参数 [Util.ParseArgs()](SunshineTool/Util.cs:64)。
  - 当 r=open 时：执行 [Util.Do()](SunshineTool/Util.cs:139)，目标语义为切换到双屏扩展模式并设置分辨率。
    - 设计语义：拓扑 EXTEND，调用 [DisplayUtil.SwitchDisplayMode()](SunshineTool/DisplayUtil.cs:45) 使用 type=2。
    - 当前实现：使用 type=3 为 EXTERNAL（仅外接屏），后续将在代码层按需求修正为 EXTEND。
    - 分辨率参数：可通过命令行 [x,y,fps](SunshineTool/Util.cs:148) 指定，否则默认 1920x1080@60。
    - 可选行为：当 [steam](SunshineTool/Util.cs:156) 参数为 true 时，执行 [Util.ShowBigSteam(true)](SunshineTool/Util.cs:132)。
  - 当 r=close 时：执行 [Util.Undo()](SunshineTool/Util.cs:163)，切回主屏拓扑 INTERNAL（type=0），并恢复配置的主屏分辨率与刷新率。

## 配置管理

- 配置文件：程序首次运行于应用目录 [Util.AppDir](SunshineTool/Util.cs:19) 下生成 cfg.json [Util.LoadConfig()](SunshineTool/Util.cs:24)。
- 默认值来源：若不存在配置文件，初始化时读取当前分辨率 [DisplayUtil.GetCurResolution()](SunshineTool/DisplayUtil.cs:117) 作为默认 [Cfg](SunshineTool/Cfg.cs)。
- 配置项：
  - [Cfg.MainWidth](SunshineTool/Cfg.cs:3)
  - [Cfg.MainHeight](SunshineTool/Cfg.cs:4)
  - [Cfg.MainFps](SunshineTool/Cfg.cs:5)

## 命令行参数

- r：运行模式，open 或 close [Program.cs](SunshineTool/Program.cs:41)、[Program.cs](SunshineTool/Program.cs:47)。
- x,y,fps：扩展模式下分辨率与刷新率 [Util.ArgGetInt()](SunshineTool/Util.cs:105)。
- steam：布尔值，控制是否打开或关闭 Steam 大屏模式 [Util.ArgGetBool()](SunshineTool/Util.cs:119)、[Util.ShowBigSteam()](SunshineTool/Util.cs:132)。

## 技术优势

- 编程语言：C#，运行环境：.NET 8.0 [TargetFramework](SunshineTool/SunshineTool.csproj:5)。
- Windows API 调用：PInvoke [SetDisplayConfig](SunshineTool/DisplayUtil.cs:31)、[EnumDisplaySettings](SunshineTool/DisplayUtil.cs:35)、[ChangeDisplaySettings](SunshineTool/DisplayUtil.cs:37)。
- 服务管理：SC.exe create/delete [ServiceHelper.InstallService()](SunshineTool/ServiceHelper.cs:31)、[ServiceHelper.UninstallService()](SunshineTool/ServiceHelper.cs:52)。
- 发布配置：单文件、自包含、压缩，参见 [SunshineTool.csproj](SunshineTool/SunshineTool.csproj)。

## 用户场景

1. 开机登录前需要强制主屏显示，保证登录界面在主屏出现。
2. 登录后快速切换到扩展模式用于多任务处理或演示。
3. 根据不同外接显示设备临时指定分辨率与刷新率。
4. 结合 Steam 大屏模式进行娱乐或展示场景。

## 重要说明

- 语义与实现一致性：产品语义确定 r=open 对应拓扑 EXTEND（type=2），r=close 对应拓扑 INTERNAL（type=0）。当前代码中 [Util.Do()](SunshineTool/Util.cs:139) 使用 type=3 EXTERNAL，后续如恢复改动可按语义修正为 EXTEND。
- 登录前限制：开机登录前调用 [SetDisplayConfig](SunshineTool/DisplayUtil.cs:31) 切换主屏在目标环境下返回 5（Access Denied），即使延时与重试也无法生效，入口路径为 [ScreenSwitchService.OnStart()](SunshineTool/ScreenSwitchService.cs:16) → [Util.SwitchToMainScreen()](SunshineTool/Util.cs:58) → [DisplayUtil.SwitchDisplayMode()](SunshineTool/DisplayUtil.cs:45)。
- 关机阶段限制：基于计划任务的关机事件触发 `r=close` 已集成在安装/卸载逻辑 [ServiceHelper.InstallService()](SunshineTool/ServiceHelper.cs:31)、[ServiceHelper.UninstallService()](SunshineTool/ServiceHelper.cs:52)，但在目标环境下未能执行，当前“关机服务”处于不可执行状态。
- 改动冻结：当前不再继续修改项目代码，保留现状以便后续参考与文档化；在 Release 服务模式下已启用文件日志 [Util.Log()](SunshineTool/Util.cs:193) 用于诊断。
