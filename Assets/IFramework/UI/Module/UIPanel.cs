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
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour
    {

        private CanvasGroup _group;
        private RectTransform _rect;
        public IUIModule module { get; set; }


        public RectTransform rectTransform
        {
            get
            {
                if (_rect == null)
                {
                    _rect = GetComponent<RectTransform>();
                }
                return _rect;
            }
        }

        public CanvasGroup group
        {
            get
            {
                if (_group == null)
                {
                    _group = GetComponent<CanvasGroup>();
                }
                return _group;
            }
        }


        public IEventArgs args { get; private set; }
        public void SetArgs(IEventArgs args)
        {
            this.args = args;
        }

        /// <summary>
        /// 该ui所属层级
        /// </summary>
        public virtual UILayer layer { get { return UILayer.Common; } }
        /// <summary>
        /// 相同层级的比较的值，越大，越后面渲染
        /// </summary>
        public virtual int layerOrder { get { return 0; } }
        /// <summary>
        /// 是否放入堆栈
        /// </summary>
        public virtual bool addToStack { get { return true; } }



        /// <summary>
        /// 使ui不响应事件
        /// </summary>
        public void Pause()
        {
            group.interactable = false;
        }
        /// <summary>
        /// 使ui响应事件
        /// </summary>
        public void UnPause()
        {
            group.interactable = true;
        }
        /// <summary>
        /// 把ui藏起来
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 展示出ui
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

    }
}
