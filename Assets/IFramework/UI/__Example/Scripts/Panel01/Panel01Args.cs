/*********************************************************************************
 *Author:         爱吃水蜜桃
 *Version:        1.0
 *UnityVersion:   2018.4.24f1
 *Date:           2021-06-27
 *Description:    Description
 *History:        2021-06-27--
*********************************************************************************/
namespace IFramework.UI.Example
{
    public enum Panel01ArgsEventType
    {
        Add,Sub,Next
    }

    public class Panel01Args : IEventArgs, IUIEvent<Panel01ArgsEventType, Panel01Args>
    {
        //write your args fields here
        public Panel01ArgsEventType type { get; set; }

        public Panel01Args SetType(Panel01ArgsEventType type)
        {
            this.type = type;
            return this;
        }
    }
}