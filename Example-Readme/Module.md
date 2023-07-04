[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 自定义模块

自己造轮子

特点:
* 自己定义一个符合框架生命周期的模块

使用示例:

``` csharp
//继承UpdateModule创建一个自定义模块
private class MyModule : IFramework.Modules.UpdateModule
{
    protected override void Awake()
    {
        Log.L("Awake");
    }
    protected override void OnEnable()
    {
        Log.L("OnEnable");
    }
    protected override void OnUpdate()
    {
        Log.L("OnUpdate");
    }
    protected override void OnDisable()
    {
        Log.L("OnDisable");
    }
    public void Do()
    {
        //Do sth
    }
    protected override void OnDispose()
    {
        Log.L("OnDispose");
    }
}

//获取模块容器
IModules moduleContainer= Framework.GetEnv(EnvironmentType.Ev0).modules;

//创建模块实例
moduleContainer.CreateModule<MyModule>();
//获取模块实例
//也可以使用GetModule在没有实例的时候自动创建实例
var module = moduleContainer.FindModule<MyModule>();

```
[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/MouduleTest.cs)

---
[回到顶部](#)