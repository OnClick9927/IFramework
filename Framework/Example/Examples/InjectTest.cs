using IFramework;
using IFramework.Inject;
using System.Collections.Generic;

namespace Example
{
    public class InjectTest : Test
    {
        /// <summary>
        /// 常规注入类的接口
        /// </summary>
        public interface ICommonInjectObject
        {
            void ToDo();
        }
        /// <summary>
        /// 常规注入类的定义
        /// </summary>
        public class CommonObject : ICommonInjectObject
        {
            private string message;
            public CommonObject()
            {
                message = "这是默认构造函数生成的对象";
            }
            public CommonObject(string message)
            {
                this.message = message;
            }
            public void ToDo()
            {
                Log.L(message);
            }
        }
        /// <summary>
        /// 测试反射注入类的接口
        /// </summary>
        public interface IReflectInjectObject
        {
            void ToDo();
        }
        /// <summary>
        /// 反射注入需要的类的定义
        /// </summary>
        public class ReflectObject : IReflectInjectObject
        {
            public void ToDo()
            {
                Log.L("这是反射注入对象调用的方法");
            }
        }
        /// <summary>
        /// 这是另一个用于反射注入的类的定义
        /// </summary>
        public class ReflectAnotherObject : IReflectInjectObject
        {
            public void ToDo()
            {
                Log.L("这是另一个反射注入对象调用的方法");
            }
        }

        [Inject]
        public IReflectInjectObject _reflectObject; //反射注入的子类型变量
        [Inject]
        public ReflectObject _reflectMyselfObject; //反射注入自身类型的变量
        [Inject("Instance1")]
        public IReflectInjectObject _reflectMultiObject1; //Attribute增加name属性的反射注入变量
        [Inject("Instance2")]
        public IReflectInjectObject _reflectMultiObject2; //Attribute增加name属性的反射注入变量
        public ICommonInjectObject _commonObject; //常规注入的变量
        public IInjectModule container; //容器
        protected override void Start()
        {
            //获取容器
            container = Framework.GetEnv(EnvironmentType.Ev0).modules.GetModule<InjectModule>();

            Log.L("");

            CommonInjectTest();  //常规的注入方法
            MultiInjectTest(); //同类型多个实例的注入
            ReflectInject(); //反射注入
            ReflectMultiInject(); //反射同类型多实例注入
        }

        /// <summary>
        /// 简单的依赖注入
        /// </summary>
        private void CommonInjectTest()
        {
            Log.L("****************************************");
            Log.L("--------------常规依赖注入--------------");
            Log.L("****************************************");
            //第一种注入实例的方法
            container.SubscribeInstance<ICommonInjectObject>(new CommonObject());
            //第一种从容器获取实例的方法
            _commonObject = container.GetValue<ICommonInjectObject>();

            Log.L("调用第一个注入实例的ToDo方法:");
            _commonObject.ToDo();

            Log.L("");

            //第二种注入实例的方法
            //如果对同一个目标类注入，将会被覆盖
            container.SubscribeInstance(typeof(ICommonInjectObject),
                                        new CommonObject("这是使用带参构造函数构造的实例"));
            //第二种获取实例的方法
            _commonObject = container.GetValue(typeof(ICommonInjectObject)) as ICommonInjectObject;

            Log.L("调用第二个注入实例的ToDo方法:");
            _commonObject.ToDo();

            //使用容器的Clear方法清除当前所有注入的实例
            container.Clear();
            Log.L("");
            Log.L("容器已清空");
            Log.L("");
        }

        /// <summary>
        /// 同类型多个实例的注入
        /// </summary>
        private void MultiInjectTest()
        {
            //一个类型可以注入多个实例，通过name这个属性来区分
            Log.L("****************************************");
            Log.L("----------同类型多个实例的注入----------");
            Log.L("****************************************");
            container.SubscribeInstance<ICommonInjectObject>(new CommonObject("这是第一个实例"), "Instance1");

            container.SubscribeInstance(typeof(ICommonInjectObject),
                                        new CommonObject("这是第二个实例"),
                                        "Instance2");

            //从容器中获取实例需要指定name
            _commonObject = container.GetValue<ICommonInjectObject>("Instance1");
            Log.L($"取出名为Instance1的实例,类型为{_commonObject.GetType()}，调用ToDo方法:");
            _commonObject.ToDo();
            Log.L("");
            _commonObject = container.GetValue(typeof(ICommonInjectObject), "Instance2") as ICommonInjectObject;
            Log.L($"取出名为Instance2的实例,类型为{_commonObject.GetType()}，调用ToDo方法:");
            _commonObject.ToDo();

            Log.L("");
            Log.L("使用容器中的GetValues方法获取ICommonInjectObject所有注入的实例,并调用ToDo方法：");
            IEnumerable<ICommonInjectObject> allInstance = container.GetValues<ICommonInjectObject>();
            //遍历调用取到的所有实例的方法
            foreach (ICommonInjectObject instance in allInstance)
            {
                instance.ToDo();
            }
            Log.L("");

            Log.L("使用另一种重载的方法获取所有实例,并调用ToDo方法：");
            IEnumerable<object> objectInstance = container.GetValues(typeof(ICommonInjectObject));
            foreach (ICommonInjectObject instance in objectInstance)
            {
                instance.ToDo();
            }

            //使用容器的Clear方法清除当前所有注入的实例
            container.Clear();
            Log.L("");
            Log.L("容器已清空");
            Log.L("");
        }

        /// <summary>
        /// 反射注入
        /// </summary>
        private void ReflectInject()
        {
            Log.L("****************************************");
            Log.L("----------------反射注入----------------");
            Log.L("****************************************");
            Log.L("注意：由于使用了反射 性能上会有影响，慎用!!!!");
            Log.L("");
            Log.L("第一种反射注入：以IReflectInjectObject的名义注入ReflectObject类型");
            container.Subscribe<IReflectInjectObject, ReflectObject>();
            //为当前类的含有[Inject]这个Attribute的对应类型的变量注入数据
            container.Inject(this);
            Log.L($"当前对象的类型是{_reflectObject.GetType()}，调用ToDo方法：");
            _reflectObject.ToDo();
            Log.L("");

            Log.L("第二种反射注入：直接注入自身类型");
            container.Subscribe<ReflectObject>();
            container.Inject(this);
            Log.L($"当前对象的类型是{_reflectMyselfObject.GetType()}，调用ToDo方法：");
            _reflectMyselfObject.ToDo();
            Log.L("");

            Log.L("第三种反射注入");
            Log.L("第三种反射注入方法，注册同一个目标类型会被覆盖");
            container.Subscribe(typeof(IReflectInjectObject), typeof(ReflectAnotherObject));
            container.Inject(this);
            Log.L($"当前对象的类型是{_reflectObject.GetType()}，调用ToDo方法：");
            _reflectObject.ToDo();

            //使用容器的Clear方法清除当前所有注入的实例
            container.Clear();
            Log.L("");
            Log.L("容器已清空");
            Log.L("");
        }

        /// <summary>
        /// 反射注入多个实例
        /// </summary>
        private void ReflectMultiInject()
        {
            Log.L("****************************************");
            Log.L("-------对同一类型反射注入多个实例-------");
            Log.L("****************************************");

            Log.L("注入多个实例需要指定name参数");
            Log.L("ReflectObject以IReflectInjectObject类型注入，name参数为\"Instance1\"");
            container.Subscribe<IReflectInjectObject, ReflectObject>("Instance1");
            Log.L("ReflectAnotherObject也以IReflectInjectObject类型注入，name参数为\"Instance2\"");
            container.Subscribe(typeof(IReflectInjectObject), typeof(ReflectAnotherObject), "Instance2");
            container.Inject(this);

            Log.L("");

            Log.L("调用ReflectObject反射注入实例的ToDo方法:");
            _reflectMultiObject1.ToDo();
            Log.L("调用ReflectAnotherObject反射注入实例的ToDo方法:");
            _reflectMultiObject2.ToDo();

            Log.L("");

            Log.L("使用容器的GetValues方法遍历一下以IReflectInjectObject注入的实例");
            IEnumerable<object> objectInstance = container.GetValues(typeof(IReflectInjectObject));
            foreach (IReflectInjectObject instance in objectInstance)
            {
                instance.ToDo();
            }
        }


        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
