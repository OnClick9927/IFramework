[返回README](../README.md) &emsp; [返回主页](https://github.com/OnClick9927/IFramework)

# 序列化

我先变成你，你再变成我，是谁变成了我，而我又变成了谁
## 1、对象与字符串的互相转化
支持`struct` 、`class`、`array`、`list`、`dictionary`外加部分集合转换成`string`

使用示例:
``` csharp
HuMan hu = new HuMan() { age = 6, Name = "abc", sex = "sex", heigth = 16, width = 20 };
Dictionary<int, HuMan> hus = new Dictionary<int, HuMan>() {
    { 1, hu },
    { 2, hu }
};
//对象转换成字符串
var str = StringConvert.ConvertToString(hus);
//字符串转换成对象
Dictionary<int, HuMan> hus2;
bool success=StringConvert.TryConvert(str, out hus2);
```

## 2、数据表格（csv）的读写
将数据写入csv文件或者从csv文件还原数据

特点：
* 支持表头的重命名、表头忽略、按索引读写等
* 支持自定义分离符

使用示例:
``` csharp
//结构体定义
struct HuMan
{
    public int age;
    [DataColumnName("The Sex")]
    public string sex;
    public string Name;
    [DataReadColumnIndex(0)]
    public int heigth;
    [DataIgnore]
    [NonSerialized]
    public int width;
}
string path = "Mans.csv";

//创建一个列表
List<HuMan> cs = new List<HuMan>()
{
    new HuMan(){ age=1,sex="m",Name="xm",heigth=0},
    new HuMan(){ age=2,sex="m1",Name="xm1",heigth=0},
    new HuMan(){ age=3,sex="m2",Name="xm2",heigth=0},
};

//写入CSV
var w = DataTableTool.CreateWriter(new System.IO.StreamWriter(path, false),
    new DataRow(),
    new DataExplainer()); //在这里可以设置分离符
w.Write(cs);
w.Dispose();

//读取CSV
var r =
    DataTableTool.CreateReader(new System.IO.StreamReader(path, System.Text.Encoding.UTF8),
    new DataRow(),
    new DataExplainer()); //分离符要与写入时一致
var cc = r.Get<HuMan>();
r.Dispose();
```
[示例代码](https://github.com/OnClick9927/IFramework_CS/blob/master/Framework/Example/Examples/SerializationTest.cs)

---
[回到顶部](#)