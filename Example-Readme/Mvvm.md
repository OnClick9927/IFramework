[[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)](../README.md)

# Mvvm
特点:
* 提供对应的类简化编写

使用示例:
``` csharp
//model
class MyModel : IModel
{
    public int value = 2;
}

//viewModel
 class MyViewModel : ViewModel<MyModel>
{
    private int _value;

    //需要监听的字段
    public int value
    {
        get { return GetProperty(ref _value); }
        set
        {
            //将值同步到Model
            Tmodel.value = value;
            SetProperty(ref _value, value);
        }
    }

    protected override void Initialize()
    {
        Log.L("ViewModel:初始化操作");
        value = 5;
    }

    protected override void Listen(IEventArgs message)
    {
        Log.E("ViewModel:接收到消息,数据+1");
        value++;
    }

    protected override void SyncModelValue()
    {
        Log.L("ViewModel:同步操作");
        value = Tmodel.value;
    }
}

//view
class MyView : View<MyViewModel>
{
    //View类里面有监听器，这里只需要重写监听方法就可以了
    protected override void BindProperty()
    {
        base.BindProperty();
        this.handler.BindProperty(() =>
        {
            //在这里写对UI的更变操作
            var value = Tcontext.value;
            Log.E($"View:数据出现更改，当前的值为：{value}");
        });

        Log.E("View:发送空消息");
        //发送消息
        Publish(null);
    }
}

```
[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/MvvmTest.cs)

---
[回到顶部](#)