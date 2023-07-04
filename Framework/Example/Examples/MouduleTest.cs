using IFramework;
using IFramework;

namespace Example
{
    //自定义模块示例
    public class MouduleTest : Test
    {
        /// <summary>
        /// 自定义模块类
        /// </summary>
        private class MyModule : IFramework.UpdateModule
        {
            protected override void Awake()
            {
                Log.L("这是模块Awake方法的调用" + GetHashCode());
            }
            protected override void OnEnable()
            {
                Log.L("这是模块OnEnable方法的调用" + GetHashCode());

            }
            protected override void OnUpdate()
            {
                Log.L("这是模块OnUpdate方法的调用" + GetHashCode());

            }
            protected override void OnDisable()
            {
                Log.L("这是模块OnDisable方法的调用" + GetHashCode());

            }
            public void Say()
            {
                Log.L("说了句话……" + GetHashCode());

            }
            protected override void OnDispose()
            {
                Log.L("这是模块OnDispose方法的调用" + GetHashCode());

            }
        }

        //获取模块
        IModules moduleContainer { get { return Framework.GetEnv(EnvironmentType.Ev0).modules; } }
        protected override void Start()
        {
            Log.L("模块的创建需要继承IFramework.Module这个类\n");
            Log.L("如果需要有生命周期的管理则可以继承IFramework.UpdateModule这个类");
            Log.L("并对其Awake、OnEnable、OnUpdate、OnDisable、OnDispose这几个生命周期进行重写\n");

            Log.L("用module的模块容器的CreateModule<T>方法创建对应的模块\n");
            moduleContainer.CreateModule<MyModule>();

            Log.L("");
            Log.L("用module的模块容器的FindModule<T>方法获取容器内对应的对象");
            var module = moduleContainer.FindModule<MyModule>();

            Log.L("模块HashCodde是" + module.GetHashCode());

            Log.L("");
            Log.L("调用模块的Say方法：");
            module.Say();

            //Log.L("使用模块的UnBind方法将模块从容器中去除");
            //Log.L("UnBind方法传入参数dispose默认为true，在没有任何引用时释放掉这个对象");
            //module.UnBind(false);

            //Log.L("可以使用Bind方法将模块绑定到容器中\n");
            //module.Bind(moduleContainer);

            //module.UnBind();
            //Log.L("由于当前方法块中还存在module变量引用这个模块，所以还是能调用模块的Say方法");
            //module.Say();

            Log.L("");
            Log.L("使用容器的GetModule方法可以获取容器中对应的模块，如果不存在则自动创建");
            Log.L("指定name属性可以注册多个同一类型的模块");
            for (int i = 0; i < 10; i++)
            {
                Log.L(moduleContainer.GetModule<MyModule>("XXL").GetHashCode());
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
