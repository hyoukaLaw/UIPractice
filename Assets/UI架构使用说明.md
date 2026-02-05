# UI框架使用说明

## 文件结构

```
Assets/
├── UIManager.cs                          # UI管理器单例
├── Scripts/UI/
│   ├── Core/                            # 纯C#逻辑层（与Unity解耦）
│   │   ├── BaseUIPanel.cs              # UI面板抽象基类
│   │   └── UIStack.cs                  # UI栈数据结构
│   ├── Data/                            # 数据和配置
│   │   ├── UIPanelType.cs              # UI面板类型枚举
│   │   ├── UIPanelData.cs              # UI面板运行时数据
│   │   ├── UILayer.cs                  # UI层级定义
│   │   └── UIPanelConfig.cs            # UI面板配置
│   ├── Adapters/                        # 适配器层（Unity表现层）
│   │   ├── MonoPanel.cs                # Mono基类（挂载到预制体）
│   │   ├── UIPanelFactory.cs           # 面板工厂
│   │   └── UIAssetLoader.cs            # 资源加载器
│   └── Panels/                          # 具体面板实现
│       ├── CharacterPanel.cs           # 纯C#逻辑
│       └── MonoCharacterPanel.cs       # Unity适配器
└── Prefabs/UI/
    └── CharacterPanel.prefab            # UI预制体（需创建）
```

## 如何使用

### 1. 创建新的UI面板

#### 步骤1：创建UI预制体
- 在 `Assets/Prefabs/UI/` 下创建预制体（例如 `CharacterPanel.prefab`）
- 添加UI组件（Button、Text等）
- 挂载 `MonoCharacterPanel.cs` 脚本

#### 步骤2：注册配置
在 `UIPanelConfigRegistry.cs` 的静态构造函数中添加配置：
```csharp
Register(new UIPanelConfig
{
    Type = UIPanelType.Character,
    PrefabPath = "Prefabs/UI/CharacterPanel",
    Layer = UILayer.Normal,
    IsModal = false
});
```

#### 步骤3：创建纯C#逻辑类
```csharp
// CharacterPanel.cs - 纯业务逻辑，与Unity解耦
public class CharacterPanel : BaseUIPanel
{
    public override void OnEnter(params object[] args)
    {
        // 处理进入逻辑
    }
    public override void OnExit() { }
    public override void OnPause() { }
    public override void OnResume() { }
}
```

#### 步骤4：创建Unity适配器
```csharp
// MonoCharacterPanel.cs - Unity表现层，继承MonoPanel
public class MonoCharacterPanel : MonoPanel
{
    protected override BaseUIPanel CreateLogicPanel()
    {
        return new CharacterPanel();
    }
    
    protected override void OnShow(params object[] args)
    {
        base.OnShow(args);
        // 更新UI显示
    }
}
```

### 2. 显示/隐藏UI

```csharp
// 打开角色面板，传入参数
UIManager.Instance.ShowPanel(UIPanelType.Character, "character_001", 50);

// 关闭角色面板
UIManager.Instance.HidePanel(UIPanelType.Character);

// 关闭顶层面板
UIManager.Instance.HideTopPanel();

// 清除所有UI
UIManager.Instance.ClearAll();
```

### 3. 在MainPanel中打开CharacterPanel

在MainPanel的按钮点击事件中调用：
```csharp
public class MonoMainPanel : MonoPanel
{
    public Button openCharacterButton;

    private void Awake()
    {
        openCharacterButton.onClick.AddListener(OnOpenCharacterClick);
    }

    private void OnOpenCharacterClick()
    {
        UIManager.Instance.ShowPanel(UIPanelType.Character);
    }
}
```

## 架构说明

### 三层架构

1. **纯C#逻辑层**（Core、Panels/*.cs）
   - 完全不依赖Unity
   - 处理业务逻辑、数据管理
   - 易于测试和移植

2. **适配器层**（Adapters、Panels/Mono*.cs）
   - 连接Unity UI和纯C#逻辑
   - 负责Unity生命周期、事件绑定、UI更新

3. **Unity表现层**（Prefabs）
   - 纯表现，通过MonoPanel脚本与逻辑层交互

### UI栈工作原理

```
Push操作: ShowPanel() → UIStack.Push() → 面板显示
Pop操作:  HidePanel() → UIStack.Pop() → 面板隐藏
Peek操作: 获取顶层面板（不删除）
```

### 关键类说明

- **UIManager**: 单例，统一管理所有UI面板
- **UIStack**: 栈数据结构，维护UI显示顺序
- **BaseUIPanel**: 纯C#面板基类
- **MonoPanel**: Unity适配器基类
- **UIPanelFactory**: 根据类型创建面板实例
- **UIAssetLoader**: 使用Resources加载预制体

## 注意事项

1. 所有UI预制体必须放在 `Resources/Prefabs/UI/` 目录下
2. 预制体必须挂载继承自 `MonoPanel` 的脚本
3. 在UIPanelConfig中注册的PrefabPath必须是Resources下的相对路径
4. 纯C#逻辑类不应引用UnityEngine命名空间
