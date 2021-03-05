/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.223
 *UnityVersion:   2018.4.24f1
 *Date:           2021-03-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.UI
{
    /// <summary>
    /// ui模块事件
    /// </summary>
    public struct ViewEvent: IUIEvent<ViewEventTpe, ViewEvent>
    {
        private ViewEventTpe _type;
        public ViewEventTpe type { get { return _type; } }
        public ViewEvent SetType(ViewEventTpe type)
        {
            _type = type;
            return this;
        }
    }
}
