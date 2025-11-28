# Su.AutoCAD2Revit 库使用文档

## 概述
使用 Teigha 运行时在 Revit 中读取 AutoCAD DWG 文件，无需安装 AutoCAD。

> **📋 版本支持**:
> - Revit: 2013 - 2024
> - AutoCAD: 2013 及以下版本 DWG 格式

## 核心特性

### 🔄 自动坐标转换
- 自动将 AutoCAD 坐标系转换为 Revit 坐标系
- 处理图纸的 Transform 变换
- 自动应用标高设置

### 🧩 智能块处理
- 自动解析嵌套块结构
- 正确处理块参照的坐标变换
- 保持块内元素的相对位置关系

## 核心类

### ReadCADService
主要的图纸读取服务类。

#### 构造函数
```csharp
// 从 Revit 链接图纸创建
var cadService = new ReadCADService(importInstance, levelHeight);

// 从 DWG 文件创建  
var cadService = new ReadCADService(dwgFilePath, levelHeight);
```

### CADTextModel
AutoCAD 文本数据模型。

| 属性 | 说明 |
|------|------|
| `Location` | 转换后的 Revit 坐标位置 |
| `Text` | 文本内容 |
| `Layer` | 图层名称 |
| `Angle` | 旋转角度 |
| `BlockName` | 所属块名称 |

## 基础用法

### 1. 读取链接图纸文字
```csharp
// 自动处理坐标转换和块变换
using (var cadService = new ReadCADService(cadLink, level.Elevation))
{
    List<CADTextModel> texts = cadService.GetAllTexts();
    
    foreach (var text in texts)
    {
        // text.Location 已经是正确的 Revit 坐标
        Console.WriteLine($"文字: {text.Text}, 位置: {text.Location}");
    }
}
```

### 2. 直接读取 DWG 文件
```csharp
using (var cadService = new ReadCADService(dwgPath, baseElevation))
{
    var texts = cadService.GetAllTexts();
    // 所有坐标已自动转换到 Revit 坐标系
}
```

## 坐标转换说明

库自动处理以下坐标变换：
- AutoCAD 点 → Revit 点（毫米到英尺）
- 图纸实例的 Transform 变换
- 绝对标高设置
- 嵌套块的层级坐标变换

## 注意事项
- 使用 `using` 语句确保资源释放
- 仅支持 AutoCAD 2013 及以下版本
- 所有坐标输出均为转换后的 Revit 坐标