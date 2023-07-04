using IFramework;
using IFramework;
using IFramework.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class TimerTest : Test
    {
        IModules moduleContainer { get { return Framework.GetEnv(EnvironmentType.Ev0).modules; } }
        protected override void Start()
        {
            moduleContainer.CreateModule<TimerModule>();

            var timeModule = moduleContainer.GetModule<TimerModule>();

            ITimerEntity actionItem = timeModule.Allocate(say, 4000,1,5000f,3f);
            actionItem.SetTimeScale(2f);
            ITimerEntity actionItem1 = timeModule.Allocate(say1, 4000, 5, 2f);
            ITimerEntity actionItem2 = timeModule.Allocate(say2, 4000, -1, 2f);
            actionItem1.SetInnerTimer(actionItem2);
            actionItem1.Subscribe();
            actionItem.Subscribe();
        }

        void say()
        {
            Log.L($"111111一次调用{DateTime.Now.ToString("HH:mm:ss,FFFF")}");
        }

        int n = 0;
        void say1()
        {
            n++;
            Log.L($"22222第{n}次调用{DateTime.Now.ToString("HH:mm:ss,FFFF")}");
        }

        int m = 0;
        void say2()
        {
            m++;
            Log.L($"333333第{m}次调用{DateTime.Now.ToString("HH:mm:ss,FFFF")}");
        }

        protected override void Stop()
        {

        }

        protected override void Update()
        {
        
        }
    }
}
