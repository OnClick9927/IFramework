using IFramework;
using IFramework.Singleton;

namespace Example
{
    public class SingletonTest : Test
    {

        public class MySingletonClass : SingletonPropertyClass<MySingletonClass>
        {
            private int a;
            private string b;
            private MySingletonClass()
            {
                Log.L("*构造函数调用方法");
                a = 1;
                b = "this is a string";
            }

            protected override void OnSingletonInit()
            {
                Log.L("*单例被创建时调用方法");
            }

            public static void DO()
            {
                Log.L("注意：使用静态方法访问实例化对象instance,instance不能在外部访问");
                Log.L($"*调用Do方法，获取instance实例化对象的的字段值：int:{instance.a} string :{instance.b}");
            }
            protected override void OnDispose()
            {

            }
        }
        protected override void Start()
        {
            System.Console.WriteLine();

            MySingletonClass.DO();

            System.Console.WriteLine();
            Log.L("以上流程的执行顺序为：");
            Log.L("1、调用静态方法Do");
            Log.L("2、开始内部实例化出对象instance");
            Log.L("3、调用构造函数");
            Log.L("4、调用OnSingletonInit");
            Log.L("5、DO方法调用完毕");
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
