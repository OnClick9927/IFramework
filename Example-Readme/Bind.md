[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 数据绑定

## 单向绑定（数据监听）

特点:
* 监听单个值的变化

单向绑定使用示例:

``` csharp
//监听的类需要继承ObservableObject
class Observable_A : ObservableObject
{
    private int _value;

    public int value
    {
        get
        {
            return GetProperty(ref _value);
        }
        set
        {
            SetProperty(ref _value, value);
        }
    }
}

//创建一个监听器
ObservableObjectHandler binder = new ObservableObjectHandler();
//创建监听的实例
Observable_A observedA = new Observable_A();

//监听value
binder.BindProperty(
    (value) =>
    {
        observedA.value = value;
        Log.L($"值被改变，新值为{value}");
    },
    () =>
    {
        return observedA.value;
    }
);

observedA.value = 1;

observedA.value = 2;
//观察控制台的输出

```
---

## 多向绑定（数据同步变化）
特点：
* 多个绑定，值同步变化

多向绑定使用示例:
``` csharp
//监听的类需要继承BindableObject
class Binder_A : BindableObject
{
    private int _value;

    public int value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
}
class Binder_B : BindableObject
{
    private int _value;

    public int value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
}

class Binder_C : BindableObject
{
    private int _value;

    public int value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
}

//创建监听器
BindableObjectHandler binder = new BindableObjectHandler();

//创建实例
Binder_A a = new Binder_A();
Binder_B b = new Binder_B();
Binder_C c = new Binder_C();

//绑定
binder.BindProperty((value) => { a.value = value; }, () => { return a.value; });
binder.BindProperty((value) => { b.value = value; }, () => { return b.value; });
binder.BindProperty((value) => { c.value = value; }, () => { return c.value; });


Log.L($"a的value值为{a.value}\tb的value值为{b.value}\tc的value值为{c.value}\n");
a.value = 1;
Log.L($"a的value值为{a.value}\tb的value值为{b.value}\tc的value值为{c.value}\n");
c.value = 2;
Log.L($"a的value值为{a.value}\tb的value值为{b.value}\tc的value值为{c.value}\n");
```

[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/BindTest.cs)

---
[回到顶部](#)

