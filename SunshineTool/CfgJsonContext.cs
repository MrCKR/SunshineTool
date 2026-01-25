using System.Text.Json.Serialization;

// System.Text.Json 源生成上下文，避免 Trim 环境下的反射序列化禁用问题
// 用法：
// 反序列化：JsonSerializer.Deserialize(json, CfgJsonContext.Default.Cfg)
// 序列化：JsonSerializer.Serialize(cfg, CfgJsonContext.Default.Cfg)

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Cfg))]
internal partial class CfgJsonContext : JsonSerializerContext
{
}
