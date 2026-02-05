# AGENTS.md

## 变量命名规范

- **public** 和 **protected** 成员：使用 PascalCase 形式
  ```csharp
  public int TestName { get; set; }
  protected void DoSomething() { }
  ```

- **private** 成员：使用 `_camelCase` 形式
  ```csharp
  private int _testName;
  private void _doSomething() { }
  ```

## Using 子句规范

- using 子句只能放在文件开头，不能放在命名空间内部

**正确示例：**
```csharp
using System;
using UnityEngine;

namespace TestNamespace
{
    public class TestClass { }
}
```

**错误示例：**
```csharp
namespace TestNamespace
{
    using System;
    using UnityEngine;

    public class TestClass { }
}
```

## 函数内部空行规范

- 函数内部不使用空行，除非是明显的功能分界处

**正确示例：**
```csharp
public void TestMethod()
{
    var a = 1;
    var b = 2;
    var c = a + b;

    // 明显的功能分界
    Debug.Log(c);
}
```

**错误示例：**
```csharp
public void TestMethod()
{
    var a = 1;

    var b = 2;

    var c = a + b;

    Debug.Log(c);
}
```
