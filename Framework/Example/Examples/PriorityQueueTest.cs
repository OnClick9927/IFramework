using IFramework;
using IFramework.Queue;
using System;

namespace Example
{
    public class PriorityQueueTest : Test
    {
        /// <summary>
        /// 快速优先级队列节点
        /// </summary>
        public class FastNode : FastPriorityQueueNode
        {
            public int value;
        }
        /// <summary>
        /// 泛型队列节点
        /// </summary>
        public class GenericNode : GenericPriorityQueueNode<char>
        {
            public int value;
        }
        /// <summary>
        /// 稳定队列节点
        /// </summary>
        public class StableNode : StablePriorityQueueNode
        {
            public int value;
        }
        protected override void Start()
        {
            FastPQueueTest();
            GenericPQueueTest();
            StablePQueueTest();
            SimplePQueueTest();
        }



        /// <summary>
        /// 快速优先级队列例子
        /// </summary>
        private void FastPQueueTest()
        {
            Log.L("——————————————————");
            Log.L("开始快速优先级队列测试\n");
            Log.L("新建一个大小为10的快速优先级队列");
            FastPriorityQueue<FastNode> nodes = new FastPriorityQueue<FastNode>(10);
            Log.L("往队列中添加value依次为1-10，优先级值依次为10-1的十个节点元素");
            Log.L("优先级值越小（也就是越紧急）的节点元素会排在队列的前面");
            Log.L("相同优先级的节点元素的排序是随机的");
            Log.L("(如果需要先进队列的元素的优先级高则使用稳定优先级队列）");
            for (int i = 10; i > 0; i--)
            {
                nodes.Enqueue(new FastNode() { value = i }, 10 - i);
            }

            Log.L("输出当前队列里的所有元素的值、优先级和位置");
            //这里遍历出来的顺序不代表最终出队的顺序（也就是位置不一定是对的）
            foreach (var node in nodes)
            {
                Log.L($"节点的value为{node.value}，优先级值为{node.priority}，当前位置是{node.position}");
            }
            Console.WriteLine();

            Log.L("重设队列大小，设置为11");
            nodes.Resize(11);

            //如果队列已经满了则会报错（开启DEBUG）
            var newNode = new FastNode() { value = 999 };
            Log.L("插入一个优先级值为7的value为999的newNode节点\n");
            //在队列中的元素无法重复加入
            //nodes.Enqueue(newNode, 1);
            nodes.Enqueue(newNode, 7);

            Log.L("判断newNode节点是否在列表中");
            Log.L(nodes.Contains(newNode));

            Log.L("更新newNode节点的优先级值为2,优先级更改必须调用UpdatePriority方法");
            nodes.UpdatePriority(newNode, 2);
            Log.L($"当前newNode的优先级值为{newNode.priority}\n");

            Log.L("将newNode节点直接从队列里删除");
            nodes.Remove(newNode);
            Log.L($"当前队列里的数量为{nodes.count}\n");

            Log.L("循环出队直到队列为空");
            while (nodes.count != 0)
            {
                Log.L($"出队元素的值为：{nodes.Dequeue().value}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 泛型优先级队列例子
        /// </summary>
        private void GenericPQueueTest()
        {
            Log.L("——————————————————");
            Log.L("开始泛型优先级队列测试\n");
            Log.L("新建一个大小为10的泛型优先级队列");
            Log.L("泛型优先级队列特点在于优先级可以用任何可以比较的泛型，同优先级先进先出");
            GenericPriorityQueue<GenericNode, char> nodes = new GenericPriorityQueue<GenericNode, char>(10);
            Log.L("往队列中添加value依次为A-J，优先级值依次为10-1的十个节点元素");
            Log.L("优先级值越小（也就是越紧急）的节点元素会排在队列的前面");
            Log.L("相同优先级的节点元素的排序是随机的");
            Log.L("(如果需要先进队列的元素的优先级高则使用稳定优先级队列）");
            for (int i = 10; i > 0; i--)
            {
                nodes.Enqueue(new GenericNode() { value = i }, Convert.ToChar(64 + i));
            }

            Log.L("输出当前队列里的所有元素的值、优先级和位置");
            //这里遍历出来的顺序不代表最终出队的顺序（也就是位置不一定是对的）
            foreach (var node in nodes)
            {
                Log.L($"节点的value为{node.value}，优先级值为{node.priority}，当前位置是{node.position}");
            }
            Console.WriteLine();

            Log.L("重设队列大小，设置为11");
            nodes.Resize(11);

            //如果队列已经满了则会报错（开启DEBUG）
            var newNode = new GenericNode() { value = 999 };
            Log.L("插入一个优先级值为B的value为999的newNode节点\n");
            //在队列中的元素无法重复加入
            //nodes.Enqueue(newNode, 1);
            nodes.Enqueue(newNode, 'B');

            Log.L("判断newNode节点是否在列表中");
            Log.L(nodes.Contains(newNode));

            Log.L("更新newNode节点的优先级值为A,优先级更改必须调用UpdatePriority方法");
            nodes.UpdatePriority(newNode, 'A');
            Log.L($"当前newNode的优先级值为{newNode.priority}\n");

            Log.L("将newNode节点直接从队列里删除");
            nodes.Remove(newNode);
            Log.L($"当前队列里的数量为{nodes.count}\n");

            Log.L("循环出队直到队列为空");
            while (nodes.count != 0)
            {
                Log.L($"出队元素的值为：{nodes.Dequeue().value}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 稳定优先级队列例子
        /// </summary>
        private void StablePQueueTest()
        {
            Log.L("——————————————————");
            Log.L("开始稳定优先级队列测试\n");
            Log.L("新建一个大小为10的稳定优先级队列");
            Log.L("稳定优先级队列和快速优先级队列的区别在于同优先级之间遵循先进先出");
            StablePriorityQueue<StableNode> nodes = new StablePriorityQueue<StableNode>(10);
            Log.L("往队列中添加value依次为1-10，优先级值依次为10-1的十个节点元素");
            Log.L("优先级值越小（也就是越紧急）的节点元素会排在队列的前面");
            Log.L("相同优先级的节点元素的排序是随机的");
            Log.L("(如果需要先进队列的元素的优先级高则使用稳定优先级队列）");
            for (int i = 10; i > 0; i--)
            {
                nodes.Enqueue(new StableNode() { value = i }, 10 - i);
            }

            Log.L("输出当前队列里的所有元素的值、优先级和位置");
            //这里遍历出来的顺序不代表最终出队的顺序（也就是位置不一定是对的）
            foreach (var node in nodes)
            {
                Log.L($"节点的value为{node.value}，优先级值为{node.priority}，当前位置是{node.position}");
            }
            Console.WriteLine();

            Log.L("重设队列大小，设置为11");
            nodes.Resize(11);

            //如果队列已经满了则会报错（开启DEBUG）
            var newNode = new StableNode() { value = 999 };
            Log.L("插入一个优先级值为7的value为999的newNode节点\n");
            //在队列中的元素无法重复加入
            //nodes.Enqueue(newNode, 1);
            nodes.Enqueue(newNode, 7);

            Log.L("判断newNode节点是否在列表中");
            Log.L(nodes.Contains(newNode));

            Log.L("更新newNode节点的优先级值为2,优先级更改必须调用UpdatePriority方法");
            nodes.UpdatePriority(newNode, 2);
            Log.L($"当前newNode的优先级值为{newNode.priority}\n");

            //Log.L("将newNode节点直接从队列里删除");
            //nodes.Remove(newNode);
            //Log.L($"当前队列里的数量为{nodes.count}\n");

            Log.L("循环出队直到队列为空");
            while (nodes.count != 0)
            {
                Log.L($"出队元素的值为：{nodes.Dequeue().value}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 简单优先级队列例子
        /// </summary>
        private void SimplePQueueTest()
        {
            Log.L("——————————————————");
            Log.L("开始简单优先级队列测试\n");
            Log.L("简单优先级队列不用考虑队列大小，且可以添加空节点元素和重复节点元素");
            Log.L("不用新建节点类型来包装数据，直接将对应类型的数据入队即可");
            Log.L("重复元素遵循先进先出");
            Log.L("");
            SimplePriorityQueue<string> nodes = new SimplePriorityQueue<string>();
            Log.L("添加10个元素优先级为1-10");
            for (int i = 1; i <= 10; i++)
            {
                nodes.Enqueue($"第{i}个生成的元素,优先级是{i}", i);
            }
            Log.L("将优先级为1和3的两个null值入队，再添加优先级分别为4和5的两个重复值\n");
            string item = "重复值";
            nodes.Enqueue(null, 1);
            nodes.Enqueue(null, 3);
            nodes.Enqueue(item, 4);
            nodes.Enqueue(item, 5);


            Log.L("遍历出队：");
            while (nodes.count != 0)
            {
                Log.L($"出队元素的值为：{nodes.Dequeue()}");
            }

            Console.WriteLine();
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
