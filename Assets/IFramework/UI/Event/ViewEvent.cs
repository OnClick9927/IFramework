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
    public struct ViewEvent: IUIEvent<ViewEventType, ViewEvent>
    {
        private ViewEventType _type;
        private IEventArgs _panelArgs;

        public IEventArgs panelArgs { get { return _panelArgs; } }
        public ViewEventType type { get { return _type; } }
        public ViewEvent SetType(ViewEventType type)
        {
            _type = type;
            return this;
        }
        public ViewEvent SetPanelArgs(IEventArgs arg)
        {
            _panelArgs = arg;
            return this;
        }
    }
}
