/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework.UI
{
    /// <summary>
    /// ui 基类
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        /// <summary>
        /// 该ui所属层级
        /// </summary>
        public virtual UILayer layer { get { return UILayer.Common; } }
        /// <summary>
        /// 是否放入堆栈
        /// </summary>
        public virtual bool addToStack { get { return true; } }
    }
}
