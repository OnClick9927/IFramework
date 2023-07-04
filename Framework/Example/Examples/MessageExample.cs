using System;
using IFramework;
using IFramework.Message;

namespace Example
{
    //消息模块示例
    public class MessageExample : Test
    {
        /// <summary>
        /// 消息类接口
        /// </summary>
        public interface IPub { }
        /// <summary>
        /// 消息类的定义
        /// </summary>
        public class Pub : IPub { }
        /// <summary>
        /// 消息监听器类
        /// </summary>
        public class Listener : IMessageListener
        {
            public Listener()
            {
                //以Listen方法为回调监听IPub的消息
                Framework.GetEnv(EnvironmentType.Ev0).modules.message.Subscribe<IPub>(Listen);

                //以监听器类注册监听MessageExample的消息
                Framework.GetEnv(EnvironmentType.Ev0).modules.message.Subscribe<MessageExample>(this);
                Log.L("已对IPub和MessageExample的消息监听进行注册");
            }

            //实现接口里的消息回调方法
            public void Listen(IMessage message)
            {
                Log.L($"收到类型为{message.subject}的消息，消息值为{message.code}");
            }
        }

        IMessageModule messageModule; //消息模块
        protected override void Start()
        {
            //获取消息模块
            messageModule = Framework.GetEnv(EnvironmentType.Ev0).modules.message;

            Log.L("设置可以监听注册的类型的所有子类型");
            messageModule.fitSubType = true;
            Log.L("设置每帧处理的消息量为1");
            messageModule.processesPerFrame = 1;

            Log.L("");
            Log.L("注册监听使用消息模块的Subscribe方法");
            Log.L("Subscribe方法可以对一个监听器进行注册，也可以对单独的委托方法进行注册");
            Log.L("注意：使用监听器为参数注册的实质是注册了监听器的Listen方法");
            Log.L("");
            Log.L("在消息监听器的构造函数中就可以直接设置对IPub和MessageExample的监听");
            Log.L("开始实例化一个消息监听器");

            Listener listenner = new Listener(); //实例化一个消息监听器

            Log.L("");

            Log.L("使用消息模块的UnSubscribe方法可以对监听取消注册");
            //可以对监听器实例进行解绑
            messageModule.UnSubscribe<IPub>(listenner);
            //也可以对监听器里的Listen回调方法进行解绑
            messageModule.UnSubscribe<IPub>(listenner.Listen);
            Log.L("已取消对IPub和MessageExample的监听");

            Log.L("");

            Log.L("也可以用重载的非泛型的Subscribe方法注册监听,取消监听也一样有非泛型的方法");
            messageModule.Subscribe(typeof(IPub), listenner);
            messageModule.Subscribe(typeof(MessageExample), listenner.Listen);

            Log.L("已重新对IPub和MessageExample的消息监听进行注册");
            Log.L("");

            Log.L("以Pub的名义发送一个消息值为100的消息");
            messageModule.Publish<Pub>(null)
                        .SetCode(100)
                        .OnCompelete((msg) =>
                        {
                            Log.L("可以看到我们只监听了IPub但是还是收到了Pub的消息");
                            Log.L($"当前消息的状态为{msg.state}   错误码为{msg.errorCode}");
                            Log.L("");
                            Log.L("发送的消息可以添加MessageUrgencyType参数来提高优先级");
                            Log.L("紧急程度为立即的消息不受processesPerFrame规则影响");
                            Log.L("----------------开始按键----------------");
                            Log.L("按键之后将分别发送三条普通、三条不重要、三条非常紧急、一条立即");
                            Log.L("然后再以异步的方式发送一条重要的消息");
                        });

        }

        //此处的Update支持异步调用
        protected async override void Update()
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape) return;
            Log.L($"检测到按键  {key.Key} 并且发送消息 ,  按键序号为    {(int)key.Key}");

            Log.L($"剩余消息条数  {messageModule.count}");
            SendMessage();

            Log.L("开始发送紧急类型为重要的一条消息");
            await messageModule.PublishByNumber(this, null, (int)MessageUrgencyType.Important).SetCode(10);
            Log.L("已发送紧急类型为重要的一条消息");

        }
        /// <summary>
        /// 发送三条普通、三条不重要、三条非常紧急程度的消息
        /// </summary>
        private void SendMessage()
        {
            for (int i = 0; i < 3; i++)
            {
                messageModule.Publish(this, null, 0, MessageUrgencyType.Common).SetCode(i + 1);
            }
            Log.L("已发送紧急类型为普通的三条消息");
            for (int i = 0; i < 3; i++)
            {
                messageModule.Publish<MessageExample>(null, 0, MessageUrgencyType.Unimportant).SetCode(i + 4);
            }
            Log.L("已发送紧急类型为不重要的三条消息");
            for (int i = 0; i < 3; i++)
            {
                messageModule.Publish(this, null, 0, MessageUrgencyType.VeryUrgent).SetCode(i + 7);
            }
            Log.L("已发送紧急类型为非常紧急的三条消息");

            messageModule.PublishByNumber(this, null, MessageUrgency.Immediately);
            Log.L("已发送紧急类型为立即的一条消息");
            Log.L("立即执行的消息不在等待队列中，因此无法设置值\n");

        }

        protected override void Stop()
        {

        }
    }

}
