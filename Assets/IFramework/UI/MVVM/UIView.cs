/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1.440
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Modules.MVVM;
using System;
using UnityEngine.UI;
using static IFramework.UI.MvvmGroups;

namespace IFramework.UI
{
    /// <summary>
    /// UIView 基类
    /// </summary>
    public abstract class UIView : View, IViewStateEventHandler
    {
        /// <summary>
        /// ui 状态
        /// </summary>
        public enum ViewState
        {
           None, Load, Top, Press, Pop, Clear
        }
        public UIPanel panel;
        private ViewState _lastState= ViewState.None;

        public ViewState lastState { get { return _lastState; } }

        protected void Show()
        {
            panel.gameObject.SetActive(true);
        }
        protected void Hide()
        {
            panel.gameObject.SetActive(false);
        }

        protected void BindText(Text text, Func<object> getter)
        {
            handler.BindProperty(() => {
                string tmp = getter().ToString();
                if (tmp!=text.text)
                {
                    text.text = tmp;
                }});
        }
        protected void BindSlider(Slider slider, Func<float> getter)
        {
            handler.BindProperty(() => {
                float tmp = getter();
                if (slider.value!=tmp)
                {
                    slider.value = tmp;
                } });
        }
        protected void BindToogle(Toggle toggle, Func<bool> getter)
        {
            handler.BindProperty(() => {
                bool tmp = getter();
                if (tmp!=toggle.isOn)
                {
                    toggle.isOn = tmp;
                }});
        }

        protected abstract void OnLoad();
        protected abstract void OnTop(UIEventArgs arg);
        protected abstract void OnPress(UIEventArgs arg);
        protected abstract void OnPop(UIEventArgs arg);
        protected abstract void OnClear();

        void IViewStateEventHandler.OnLoad()
        {
            _lastState = ViewState.Load;
            OnLoad();
        }
        void IViewStateEventHandler.OnTop(UIEventArgs arg)
        {
            _lastState = ViewState.Top;
            OnTop(arg);
        }
        void IViewStateEventHandler.OnPress(UIEventArgs arg)
        {
            _lastState = ViewState.Press;
            OnPress(arg);
        }
        void IViewStateEventHandler.OnPop(UIEventArgs arg)
        {
            _lastState = ViewState.Pop;
            OnPop(arg);
        }
        void IViewStateEventHandler.OnClear()
        {
            _lastState = ViewState.Clear;
            OnClear();
        }
    }
    public abstract class UIView<VM, Panel> : UIView where VM : UIViewModel where Panel : UIPanel
    {
        public VM Tcontext { get { return this.context as VM; } }
        public Panel Tpanel { get { return this.panel as Panel; } }
    }
}
