[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 可回收对象


一生尽在掌握

特点:
* 从环境中分配出来，用完还回去(内部实现为对象池)
* 可回收对象带有生命周期


使用示例:
``` csharp
//继承RecyclableObject创建可回收对象类
public class MyObject : RecyclableObject
{
    private int value;
    protected override void OnAllocate()
    {
        base.OnAllocate();
        Log.L("OnAllocate");
        //数值变动之后要SetDirty
        //没有设置Dirty表示没有变动，回收时不会重置数据
        value = 10;
        SetDataDirty();
    }
    protected override void OnRecyle()
    {
        Log.L("OnRecyle");
        base.OnRecyle();
    }
    protected override void OnDispose()
    {
        Log.L("OnDispose");
    }
    protected override void OnDataReset()
    {
        //设置对象的重置
        Log.L("OnDataReset");
        value = default;
    }

    public void PrintValue()
    {
        Log.L($"value的值为{value}");
    }
}


//从环境中分配一个对象
MyObject _object = MyObject.Allocate<MyObject>(EnvironmentType.Ev0);
_object.PrintValue();
//回收
_object.Recyle();
```
[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/RecyclableObjectTest.cs)

---
[回到顶部](#)