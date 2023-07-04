[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 消息


> A:回复T即可退订<br>
B:T<br>
A:回复T即可退订<br>
B:T即可<br>
A:退订成功

消息是软件开发中减少模块耦合的有效方式

模块特点:
* 支持流量控制
* 支持子类型匹配(根据子类型监听父类型)
* 支持异步等待
* 支持消息优先级

使用示例:
``` csharp
//需要监听的类
public interface IPub { }
public class Pub : IPub { }

//自定义监听器类
 public class Listener : IMessageListener
{
    public Listener()
    {
        //构造函数，在这里可以直接写对某个类型的监听
    }

    //实现接口里的消息回调方法
    public void Listen(IMessage message)
    {
        Log.L($"收到类型为{message.subject}的消息，消息值为{message.code}");
    }
}
//获取消息模块
messageModule = Framework.GetEnv(EnvironmentType.Ev0).modules.Message;
//匹配子类型开关
messageModule.fitSubType = true;
//每帧处理的消息量
messageModule.processesPerFrame = 1;

//实例化一个消息监听器
Listener listenner = new Listener(); 

//根据IPub监听
messageModule.Subscribe<IPub>(listenner);

//发送一条重要程度为Important的消息，消息值为100
messageModule.Publish<Pub>(null,MessageUrgencyType.Important).SetCode(100);
```

[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/MessageExample.cs)

---
[回到顶部](#)