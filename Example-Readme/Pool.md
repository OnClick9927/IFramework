[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 对象池
提供多种对象池供开发者使用
## 1、自动管理对象池
最简单的对象池使用方式

特点：
* 内部自动实例化新对象，创建之后直接使用，非常简便
* 支持含有多个参数的构造方法的对象

使用示例:
``` csharp
//定义一个类
public class Human
{
    public readonly int age;
    public readonly string name;

    public Human(int age, string name)
    {
        this.age = age;
        this.name = name;
    }

    public void Say()
    {
        Log.L($"我的名字叫{name},{age}岁");
    }
}

//创建对象池
ActivatorCreatePool<Human> pool = new ActivatorCreatePool<Human>(33, "jljy");
//从对象池中获取对象
var human = pool.Get();
human.Say();
//将对象放回对象池
pool.Set(human);
```

# 2、定制生命周期的对象池
自己定制生命周期，特别好用的对象池

特点：
* 可以对对象的生成、获取、放回等生命周期进行定制

示例代码:
``` csharp
public class Obj_A { }
//继承ObjectPool<T>创建一个对象池类型MyPool
private class MyPool : ObjectPool<Obj_A>
{
    protected override Obj_A CreatNew(IEventArgs arg)
    {
        Log.L("*对象池：创建了一个实例");
        return new Obj_A();
    }
    protected override void OnCreate(Obj_A t, IEventArgs arg)
    {
        Log.L("*对象池：实例正在被创建");
        base.OnCreate(t, arg);
    }
    protected override void OnGet(Obj_A t, IEventArgs arg)
    {
        Log.L("*对象池：正在获取实例");
        base.OnGet(t, arg);
    }
    protected override bool OnSet(Obj_A t, IEventArgs arg)
    {
        Log.L("*对象池：正在回收实例");
        return base.OnSet(t, arg);
    }
    protected override void OnClear(Obj_A t, IEventArgs arg)
    {
        Log.L("*对象池：正在清除池子里的实例");
        base.OnClear(t, arg);
    }
    protected override void OnDispose()
    {
        Log.L("*对象池：池子正在被释放");
        base.OnDispose();
    }
}
```
# 3、基类对象池
鱼池里有什么鱼？

特点：
* 能从对象池中拿出父类对象

示例代码:
``` csharp
//定义一个IObject接口和父类Obj_A,Obj_B
public interface IObject { }
public class Obj_A : IObject { }
public class Obj_B : IObject { }

//创建基类对象池类型
private class MutiPool : BaseTypePool<IObject> { }

//创建对象池
MutiPool pool = new MutiPool();
//获取Obj_A类型的对象
IObject _obj = pool.Get<Obj_A>();
pool.Set(_obj);
//获取Obj_B类型的对象
_obj = pool.Get(typeof(Obj_B));
pool.Set(_obj);
```
# 4、全局对象池
我的资源竟然是公家的！

特点:
* 对象池不需要自己创建，获取对象直接从框架中申请
* ***注意回收时要自己初始化数据！！！***

示例代码:
``` csharp
//从全局对象池中获取一个长度为10的数组对象
var arr = Framework.GlobalAllocateArray<Human>(10);
//Human类的定义在第一个示例
arr[0] = new Human(33, "吉良吉影");
arr[0].Say();
//回收掉这个数组对象
arr.GlobalRecyle();
重新从全局对象池中获取长度为10的数组对象
arr = Framework.GlobalAllocateArray<Human>(10);
arr[0].Say();
arr.GlobalRecyle();
//可以发现是和原先同样的对象
//因此回收的时候需要注意初始化数据！！！！
```
[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/PoolTest.cs)


---
[回到顶部](#)

