# UICanvasManager 配置说明

## Unity编辑器配置步骤

### 1. 创建UICanvasManager对象

在Unity编辑器中：
1. 右键Hierarchy → Create Empty
2. 重命名为 `UICanvasManager`

### 2. 挂载UICanvasManager脚本

1. 选中 `UICanvasManager` GameObject
2. 在Inspector面板点击 `Add Component`
3. 搜索并添加 `UICanvasManager` 脚本

### 3. 创建Canvas对象

1. 右键 `UICanvasManager` → UI → Canvas
2. 重命名为 `NormalCanvas`

### 4. 配置Canvas属性

在 `NormalCanvas` 的Inspector中设置：
- **Render Mode**: Screen Space - Overlay
- **Canvas Scaler**: 
  - UI Scale Mode: Scale With Screen Size
  - Reference Resolution: 1920 x 1080
  - Match: 0.5 (width和height平衡)

### 5. 将Canvas拖拽到UICanvasManager

在 `UICanvasManager` 的Inspector中：
1. 找到 `Normal Canvas` 字段
2. 将 `NormalCanvas` 从Hierarchy拖拽到该字段

### 6. 最终Hierarchy结构

```
UICanvasManager (GameObject)
└── NormalCanvas (Canvas)
    └── EventSystem (可选)
```

## 注意事项

1. **场景中只能有一个UICanvasManager**，否则会自动销毁重复的实例
2. **Normal Canvas必须手动指定**，不会自动创建
3. 如果场景中没有EventSystem，Unity会自动创建（UI交互必需）
4. **所有UI面板会自动实例化到Normal Canvas下**

## 验证配置是否成功

在Unity编辑器中：
1. 点击Play运行游戏
2. 调用 `UIManager.Instance.ShowPanel(UIPanelType.Character)`
3. 在Hierarchy中应该能看到：
   - `NormalCanvas` 下新增了 `CharacterPanel(Clone)` 对象

## 扩展多层级Canvas（未来）

如果需要扩展为多层级Canvas：
1. 修改 `UICanvasManager.cs`，添加更多Canvas字段（如 `modalCanvas`、`popupCanvas`等）
2. 修改 `GetLayerTransform()` 方法，根据不同UILayer返回对应的Canvas
3. 在UIPanelConfig中为不同面板设置不同的UILayer
4. 在Unity中创建对应的Canvas对象并拖拽到UICanvasManager

当前简化版所有UI都使用 `UILayer.Normal`。
