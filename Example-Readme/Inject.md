[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 依赖注入

放什么拿什么

特点:
* 支持直接注入和反射注入
* 支持根据name注入同类型的多个实例

直接注入使用示例:

``` csharp
public interface ICommonInjectObject
{
    void ToDo();
}

public class CommonObject : ICommonInjectObject
{
    public void ToDo()
    {
        //Do sth
    }
}

//获取容器
container = Framework.GetEnv(EnvironmentType.Ev0).container;

//往容器中注册实例
container.SubscribeInstance<ICommonInjectObject>(new CommonObject());

//获取实例
var _commonObject = container.GetValue<ICommonInjectObject>();

//通过name区分来注册多个同类型实例
container.SubscribeInstance<ICommonInjectObject>(new CommonObject(), "Instance1");
//根据name获取指定类型的实例
_commonObject = container.GetValue<ICommonInjectObject>("Instance1");

//使用容器的Clear方法清除当前所有注入的实例
container.Clear();
```
---

***反射注入强调性能之处慎用***

反射注入使用示例:
``` csharp
public interface IReflectInjectObject
{
    void ToDo();
}

public class ReflectObject : IReflectInjectObject
{
    public void ToDo()
    {
        //Do sth
    }
}

//使用Inject这个Attribute标记需要反射注入的变量
[Inject]
public IReflectInjectObject _reflectObject;
//Attribute增加name属性
[Inject("Instance1")]
public IReflectInjectObject _reflectMultiObject1; 

//以IReflectInjectObject的名义注入ReflectObject类型
container.Subscribe<IReflectInjectObject, ReflectObject>();

container.Subscribe<IReflectInjectObject, ReflectObject>("Instance1");

//为当前类的含有[Inject]这个Attribute的对应类型的变量注入数据
container.Inject(this);
```

[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/InjectTest.cs)

---
[回到顶部](#)

