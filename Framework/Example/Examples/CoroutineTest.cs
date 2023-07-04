using System.Collections;
using System.Threading;
using IFramework;
using IFramework.Coroutine;

namespace Example
{
    //协程测试
    public class CoroutineTest : Test
    {
        private ICoroutineModule coroutineModule; //协程模块
        private bool flag = false;//用于等待条件的判断
        private ICoroutine myCoroutine;
        protected override void Start()
        {
            //获取协程模块
            coroutineModule = Framework.GetEnv(EnvironmentType.Ev0).modules.GetModule<CoroutineModule>();

            //调用协程示例方法
            CoroutineExample();
        }

        /// <summary>
        /// 协程示例方法(异步）
        /// </summary>
        async void CoroutineExample()
        {
            Log.L("使用协程模块的StartCoroutine方法开始运行协程：\n");
            await coroutineModule.StartCoroutine(StartExample());
            Log.L("协程示例结束");
        }

        /// <summary>
        /// 协程开始
        /// </summary>
        /// <returns></returns>
        IEnumerator StartExample()
        {
            Log.L("协程StartExample开始");
            Log.L("使用嵌套的协程WaitExample\n");
            yield return WaitExample();
            Log.L("协程StartExample结束");
        }

        /// <summary>
        /// 协程 时间等待 例子
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitExample()
        {
            Log.L("协程WaitExample开始");

            yield return new WaitForFrame();
            Log.L("一帧过去了");

            yield return new WaitForFrames(20);
            Log.L("20帧过去了");

            yield return new WaitForHours(0.0004);
            Log.L("0.0004小时过去了");

            yield return new WaitForMinutes(0.02);
            Log.L("0.02分钟过去了");

            yield return new WaitForSeconds(3);
            Log.L("3秒过去了");

            yield return new WaitForMilliseconds(2000);
            Log.L("2000毫秒过去了");

            yield return new WaitForTicks(100000000);
            Log.L("一亿个一百纳秒过去了");

            //本质上都是以这个为原型的
            yield return new WaitForTimeSpan(new System.TimeSpan(0, 0, 1));
            Log.L("1秒的间隔过去了\n");

            Log.L("以下开始条件等待的示例");
            Log.L("使用嵌套的协程ConditionExample");
            yield return ConditionExample();
            Log.L("协程WaitExample结束");
        }

        /// <summary>
        /// 条件等待例子
        /// </summary>
        /// <returns></returns>
        IEnumerator ConditionExample()
        {
            flag = false;
            Log.L("协程ConditionExample开始");
            myCoroutine = coroutineModule.StartCoroutine(WaitAndChangeFlag());

            //等待 条件成立
            yield return new WaitUtil(CatchFlag);
            Log.L("ConditionExample:flag已变为true");

            //等待 条件不成立
            yield return new WaitWhile(CatchFlag);
            Log.L("ConditionExample:flag已变为false");

            Log.L("使用Compelete方法强制停止协程WaitAndChangeFlag");
            myCoroutine.Compelete();
            //coroutineModule.StopCoroutine(myCoroutine);
            Log.L("协程ConditionExample结束");
        }

        /// <summary>
        /// 一定时间后更改flag的值
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitAndChangeFlag()
        {
            yield return new WaitForSeconds(3);
            flag = true;
            Log.L("将flag改为true");

            yield return new WaitForSeconds(3);
            flag = false;
            Log.L("将flag改为false");

            yield return new WaitForSeconds(10);
            Log.L("WaitAndChangeFlag结束");
            //但是因为在上面调用了complete提前结束了所以不会调用到
        }

        /// <summary>
        /// 返回flag
        /// </summary>
        /// <returns></returns>
        bool CatchFlag()
        {
            //Log.L("当前flag是" + flag);
            return flag;
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
