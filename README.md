## 开发者注意事项

本项目通过根目录的 `tModLoader.local.props` 定位本机的 Steam tModLoader 安装目录。该文件是正式项目配置；在 macOS 与 Windows 间切换时，**只修改其中的 `TModLoaderPath`**，不需要改动 `.csproj`、VS Code 任务或启动配置。

```xml
<TModLoaderPath>/你的/tModLoader/安装目录</TModLoaderPath>
```

常见安装路径：

- macOS：`/Users/你的用户名/Library/Application Support/Steam/steamapps/common/tModLoader`
- Windows：`C:/Program Files (x86)/Steam/steamapps/common/tModLoader`

开发机需已安装 Steam 版 tModLoader 和 .NET 8 SDK。VS Code 中打开解决方案后，使用 `⌘⇧B`（Windows 为 `Ctrl+Shift+B`）构建；在 C# Dev Kit 中选择 `Terraria` 启动配置后按 `F5` 调试。
