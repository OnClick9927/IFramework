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

        private CanvasGroup _group;
        private RectTransform _rect;

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

        /// <summary>
        /// 该ui所属层级
        /// </summary>
        public virtual UILayer layer { get { return UILayer.Common; } }
        /// <summary>
        /// 是否放入堆栈
        /// </summary>
        public virtual bool addToStack { get { return true; } }




        public void Pause()
        {
            group.interactable = false;
        }
        public void UnPause()
        {
            group.interactable = true;
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Close()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
