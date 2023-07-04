[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 记录模块

拿小本本记下一举一动

模块特点:
* 撤销与反撤销
* 支持以Command或Action作为操作
* 支持将多个操作合并成一个组调用

使用示例:
``` csharp
//获取模块
IOperationRecorderModule recorderModule=Framework.GetEnv(EnvironmentType.Ev0).modules.Recoder;

static int value = 0;

//增加数值Command
struct AddValueCommand : ICommand
{
    public void Excute()
    {
        value++;
    }
}
//减少数值Command
struct SubValueCommand : ICommand
{
    public void Excute()
    {
        value--;
    }
}

//注册Action操作和其回退操作
var actionState = recorderModule.AllocateAction().SetCommand(() => { value += 16; }, () => { value -= 16; });
actionState.SetName("增加了16");
actionState.Subscribe();
//State可以多次调用
//但是请【不要】在注册之后修改State，否则会对前面的操作有影响
actionState.Subscribe();
actionState.Subscribe();

//注册Command操作和其回退操作
var commandState = recorderModule.AllocateCommand().SetCommand(new AddValueCommand(), new SubValueCommand());
commandState.SetName("增加了1");
commandState.Subscribe();

//注册Action一组操作和其回退操作
var actionGroupState = 
    recorderModule.AllocateActionGroup()
    .SetGroupCommand(() => { value += 1; }, () => { value -= 1; })
    .SetGroupCommand(() => { value += 2; }, () => { value -= 2; })
actionGroupState.SetName("增加了3");
actionGroupState.Subscribe();

Log.L(value);
//撤销操作 返回值为是否撤销成功
bool bo = recorderModule.Undo();
Log.L(value);
//反撤销操作 返回值为是否反撤销成功
bool bo = recorderModule.Redo();
Log.L(value);
```
[示例代码](https://github.com/OnClick9927/IFramework/blob/master/Framework/Example/Examples/RecorderTest.cs)

---
[回到顶部](#)