# Su.AutoCAD2Revit Library Documentation

## Overview
A library that uses Teigha runtime to read AutoCAD DWG files in Revit without requiring AutoCAD installation.

> **📋 Version Support**:
> - Revit: 2013 - 2024
> - AutoCAD: DWG format 2013 and below

## Core Features

### 🔄 Automatic Coordinate Transformation
- Automatically converts AutoCAD coordinate system to Revit coordinate system
- Handles drawing Transform transformations
- Automatically applies level elevation settings

### 🧩 Smart Block Processing
- Automatically resolves nested block structures
- Correctly handles coordinate transformations for block references
- Maintains relative positions of elements within blocks

## Core Classes

### ReadCADService
Main drawing reading service class.

#### Constructors
```csharp
// Create from Revit linked drawing
var cadService = new ReadCADService(importInstance, levelHeight);

// Create from DWG file
var cadService = new ReadCADService(dwgFilePath, levelHeight);
```

### CADTextModel
AutoCAD text data model.

| Property | Description |
|------|------|
| `Location` | Transformed Revit coordinate position |
| `Text` | Text content |
| `Layer` | Layer name |
| `Angle` | Rotation angle |
| `BlockName` | Parent block name |

## Basic Usage

### 1. Read Text from Linked Drawing
```csharp
// Automatically handles coordinate transformation and block transformations
using (var cadService = new ReadCADService(cadLink, level.Elevation))
{
    List<CADTextModel> texts = cadService.GetAllTexts();
    
    foreach (var text in texts)
    {
        // text.Location is already in correct Revit coordinates
        Console.WriteLine($"Text: {text.Text}, Position: {text.Location}");
    }
}
```

### 2. Read Directly from DWG File
```csharp
using (var cadService = new ReadCADService(dwgPath, baseElevation))
{
    var texts = cadService.GetAllTexts();
    // All coordinates are automatically converted to Revit coordinate system
}
```

## Coordinate Transformation Details

The library automatically handles the following coordinate transformations:
- AutoCAD points → Revit points (millimeters to feet)
- Drawing instance Transform transformations
- Absolute elevation settings
- Hierarchical coordinate transformations for nested blocks

## Important Notes
- Use `using` statement to ensure proper resource disposal
- Supports only AutoCAD 2013 and below versions
- All output coordinates are transformed Revit coordinates
