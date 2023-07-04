[[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)](../README.md)

# 单例
快速创建单例类，使其全局唯一

使用方式：继承`SingletonPropertyClass`&lt;`T`&gt;

特点：
* 在第一次调用时会调用构造函数和初始化函数
* 使用静态方法，**不允许**外部访问`instance`

使用示例:
``` csharp
//继承SingletonPropertyClass
public class MySingletonClass : SingletonPropertyClass<MySingletonClass>
{
    private int a;
    private string b;
    private MySingletonClass()
    {
        //构造函数
        a = 1;
        b = "this is a string";
    }

    protected override void OnSingletonInit()
    {
        //初始化
    }

    public static void DO()
    {
        //注意：使用【静态方法】访问实例化对象instance,instance 【不能】 在外部访问
        System.Console.WriteLine($"int:{instance.a} string :{instance.b}");
    }

    protected override void OnDispose()
    {

    }
}

//方法调用
MySingletonClass.DO();
```
[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/SingletonTest.cs)

---
[回到顶部](#)