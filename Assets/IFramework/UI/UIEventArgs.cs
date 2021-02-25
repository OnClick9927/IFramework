/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace IFramework.UI
{
    /// <summary>
    /// ui 操作 消息记录
    /// </summary>
    public class UIEventArgs : RecyclableObject
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public enum Code
        {
            /// <summary>
            /// 后退
            /// </summary>
            GoBack,
            /// <summary>
            /// 前进
            /// </summary>
            GoForward,
            /// <summary>
            /// 压栈
            /// </summary>
            Push
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public Code code;
        /// <summary>
        /// 弹出的ui
        /// </summary>
        public UIPanel popPanel;
        /// <summary>
        /// 当前 ui
        /// </summary>
        public UIPanel curPanel;
        /// <summary>
        /// 压栈 ui
        /// </summary>
        public UIPanel pressPanel;
        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void OnDataReset()
        {
            popPanel = null;
            curPanel = null;
            pressPanel = null;
        }
    }
}
