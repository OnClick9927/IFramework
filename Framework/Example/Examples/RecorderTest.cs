using System;
using IFramework;
using IFramework.Recorder;

namespace Example
{
    //记录模块例子(撤销与反撤销)
    public class RecorderTest : Test
    {
        static int value = 0; //本例子的主角,全都改在它身上

        //让Value增加1的Command操作
        struct AddValueCommand : ICommand
        {
            public void Excute()
            {
                value++;
            }
        }

        //让Value减少1的Command操作
        struct SubValueCommand : ICommand
        {
            public void Excute()
            {
                value--;
            }
        }

        //获取模块
        IOperationRecorderModule recorderModule { get { return Framework.GetEnv(EnvironmentType.Ev0).modules.recoder; } }

        protected override void Start()
        {

            Log.L("操作分为CommandState、ActionState、CommandGroupState、ActionGroupState");
            Log.L("State为一次操作的声明，其中包含了Redo和Undo两个步骤，也就是操作和撤回");
            Log.L("使用State的Subscribe方法将操作注册到记录模块中，此时就会开始State的Redo步骤");

            Log.L($"例子开始：Value的值是{value} \n");

            Log.L("注册一个ActionState，使Value的值增加16，并给这一步操作赋予一个名字   ");
            var actionState = recorderModule.AllocateAction().SetCommand(() => { value += 16; }, () => { value -= 16; });
            actionState.SetName("增加了16");
            actionState.Subscribe();
            Log.L($"此时Value的值为{value} \n");

            Log.L("注册一个CommandState，使Value的值增加1");
            var commandState = recorderModule.AllocateCommand().SetCommand(new AddValueCommand(), new SubValueCommand());
            commandState.SetName("增加了1");
            commandState.Subscribe();
            Log.L($"此时Value的值为{value} \n");

            Log.L("注册一组ActionState，使Value的值依次增加1、2、3");
            var actionGroupState = recorderModule.AllocateActionGroup()
                                                 .SetGroupCommand(() => { value += 1; }, () => { value -= 1; })
                                                 .SetGroupCommand(() => { value += 2; }, () => { value -= 2; })
                                                 .SetGroupCommand(() => { value += 3; }, () => { value -= 3; });
            actionGroupState.SetName("增加了6");
            actionGroupState.Subscribe();
            Log.L($"此时Value的值为{value} \n");

            Log.L("注册一组CommandState，使Value的值增加三次1");
            var commandGroupState = recorderModule.AllocateCommandGroup()
                                                 .SetGroupCommand(new AddValueCommand(), new SubValueCommand())
                                                 .SetGroupCommand(new AddValueCommand(), new SubValueCommand())
                                                 .SetGroupCommand(new AddValueCommand(), new SubValueCommand());
            commandGroupState.SetName("增加了3");
            commandGroupState.Subscribe();
            Log.L($"此时Value的值为{value} \n");

            Log.L("保存的state可以多次注册,将commandGroupState调用三次");
            commandGroupState.Subscribe();
            commandGroupState.Subscribe();
            commandGroupState.Subscribe();
            Log.L($"此时Value的值为{value} \n");


            Log.L("按A撤销，按D反撤销，按S注册新的操作，按W获取当前操作记录\n");
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
            if (Console.ReadKey().Key == ConsoleKey.A)
            {
                bool bo = recorderModule.Undo();
                Log.L(bo?"撤销成功" : "撤销失败");
                Log.L($"此时Value的值为{value}，当前的操作是{recorderModule.GetCurrentRecordName()}\n");
            }
            if (Console.ReadKey().Key == ConsoleKey.D)
            {
                bool bo = recorderModule.Redo();
                Log.L(bo ? "反撤销成功" : "反撤销失败");
                Log.L($"此时Value的值为{value}，当前的操作是{recorderModule.GetCurrentRecordName()}\n");
            }
            if (Console.ReadKey().Key == ConsoleKey.S)
            {
                int i = new Random().Next(20);
                var state = recorderModule.AllocateAction().SetCommand(() => { value += i; }, () => { value -= i; });
                state.SetName($"增加了{i}");
                state.Subscribe();
                Log.L($"此时Value的值为{value}，当前的操作是{recorderModule.GetCurrentRecordName()}\n");
            }
            if (Console.ReadKey().Key == ConsoleKey.W)
            {
                var records = recorderModule.GetRecordNames(out int index);
                Log.L("当前的操作列表为：");
                foreach (var item in records)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine($"当前的操作位置为：{index}");
            }
        }
    }
}
