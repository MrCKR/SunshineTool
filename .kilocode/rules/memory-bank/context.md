# 项目上下文

## 当前工作重点
- 登录前切换失败：开机登录前切回主屏未能成功执行，调用 [SetDisplayConfig](SunshineTool/DisplayUtil.cs:31) 在安全桌面阶段返回 5（Access Denied），即使延时与重试也无法生效。已在 Release 服务模式启用文件日志 [Util.Log()](SunshineTool/Util.cs:193) 记录此限制。
- 关机触发无效：基于计划任务的关机触发方案（[CreateShutdownTask()](SunshineTool/ServiceHelper.cs)）实测无法在关机阶段执行 `r=close`，当前代码中的“关机服务”处于不可执行状态。
- 冻结改动：目前不再继续修改该项目代码，保留现状以便后续参考与文档化。

## 最近变化
- 修复服务安装路径与权限：单文件发布场景下改用 [Util.ExePath](SunshineTool/Util.cs:20) 作为 binPath；管理员检测与双路径执行，记录 stdout/stderr/ExitCode 并校验安装结果，见 [ServiceHelper.InstallService()](SunshineTool/ServiceHelper.cs)。
- 启用服务模式文件日志：在 Release 下强制写入文件日志以便登录前诊断，见 [Util.Log()](SunshineTool/Util.cs:193)。
- 调整服务启动时机：增加启动延时与重试逻辑，路径在 [ScreenSwitchService.OnStart()](SunshineTool/ScreenSwitchService.cs:16)，但登录前仍受系统限制导致切换失败。
- 集成关机计划任务：安装时创建、卸载时删除计划任务以尝试在关机事件触发 `r=close`，入口在 [ServiceHelper.InstallService()](SunshineTool/ServiceHelper.cs) 与 [ServiceHelper.UninstallService()](SunshineTool/ServiceHelper.cs)。经验证该方案在目标环境下未能执行。

## 下一步计划
- 文档化限制并停止代码改动：明确“登录前无法执行屏幕切换”“关机阶段无法执行计划任务触发”的现状，保留现有实现不再修改。
- 如需替代方案（未来考虑）：
  - 登录后触发：采用 Windows 任务计划程序 Logon 触发，或服务在 [OnSessionChange](SunshineTool/ScreenSwitchService.cs) 处理 SessionLogon/Unlock 后执行切换。
  - 供应商工具：评估显卡供应商提供的显示管理工具可否在安全桌面阶段强制切换。
  - 运行依赖与时机：如需继续探索，可尝试延迟自动启动、增加依赖服务或更长延时，但当前不实施。
