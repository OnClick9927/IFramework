/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.UI
{
    /// <summary>
    /// ui 组
    /// </summary>
    public interface IGroups : IDisposable
    {
        /// <summary>
        /// 刷新
        /// </summary>
        void Update();
        /// <summary>
        /// 寻找 ui
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        UIPanel FindPanel(string name);
        /// <summary>
        /// 触发 事件
        /// </summary>
        /// <param name="arg"></param>
        void InvokeViewState(UIEventArgs arg);
        /// <summary>
        /// 添加 ui 入组
        /// </summary>
        /// <param name="panel"></param>
        void Subscribe(UIPanel panel);
        /// <summary>
        /// 移除 ui
        /// </summary>
        /// <param name="panel"></param>
        void UnSubscribe(UIPanel panel);
    }
}
