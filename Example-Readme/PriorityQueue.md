[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 优先级队列
> A:请不要插队<br>
B:我有VIP<br>
A:前面全是SVIP在等

根据优先级自动排序的队列，消息模块就用到了这个队列来根据紧急程度排序

## 1、快速优先级队列

特点:
* 优先级值低的排在队列前面
* 相同优先级的节点元素的排序是**随机的**

使用示例:
``` csharp
//节点
public class FastNode : FastPriorityQueueNode
{
    public int value;
}

//创建优先级队列
FastPriorityQueue<FastNode> nodes = new FastPriorityQueue<FastNode>(10);

//节点元素入队
for (int i = 10; i > 0; i--)
{
    nodes.Enqueue(new FastNode() { value = i }, 10 - i);
}

//节点元素出队
while (nodes.count != 0)
{
    Log.L($"出队元素的值为：{nodes.Dequeue().value}");
}
```

## 2、泛型优先级队列
特点:
* 优先级值使用可以对比值大小的泛型
* 优先级值低的排在队列前面
* 相同优先级的节点元素的排序是**先进先出的**

使用示例:
``` csharp
//节点
public class GenericNode : GenericPriorityQueueNode<char>
{
    public int value;
}

//创建队列，这里以char为例
GenericPriorityQueue<GenericNode, char> nodes = new GenericPriorityQueue<GenericNode, char>(10);
//节点元素入队
for (int i = 10; i > 0; i--)
{
    nodes.Enqueue(new GenericNode() { value = i }, Convert.ToChar(64 + i));
}

//节点元素出队
while (nodes.count != 0)
{
    Log.L($"出队元素的值为：{nodes.Dequeue().value}");
}
```

## 3、稳定优先级队列
特点:
* 优先级值低的排在队列前面
* 相同优先级的节点元素的排序是**先进先出的**

和快速优先级队列的区别就只在于出现相同优先级的判断……

## 4、简单优先级队列
特点:
* 队列没有大小限制
* 支持添加空节点元素和重复的节点元素
* 优先级值低的排在队列前面
* 相同优先级的节点元素的排序是**先进先出的**


使用示例：
``` csharp
//创建队列
SimplePriorityQueue<string> nodes = new SimplePriorityQueue<string>();

//节点元素入队
for (int i = 1; i <= 10; i++)
{
    nodes.Enqueue($"{i}", i);
}
//空节点元素和重复节点元素入队
string item = "重复值";
nodes.Enqueue(null, 1);
nodes.Enqueue(null, 3);
nodes.Enqueue(item, 4);
nodes.Enqueue(item, 5);

//节点元素出队
while (nodes.count != 0)
{
    Log.L($"出队元素的值为：{nodes.Dequeue()}");
}
```
[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/PriorityQueueTest.cs)

---
[回到顶部](#)

