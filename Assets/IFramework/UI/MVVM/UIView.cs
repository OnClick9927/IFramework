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
using UnityEngine.Events;
using UnityEngine.UI;
using static IFramework.UI.MvvmGroups;

namespace IFramework.UI
{
    /// <summary>
    /// UIView 基类
    /// </summary>
    public abstract class UIView : View, IViewStateEventHandler
    {

        public UIPanel panel;
        private ViewEventTpe _lastState= ViewEventTpe.None;

        public ViewEventTpe lastState { get { return _lastState; } }

        protected virtual void Show()
        {
            panel.Show();
        }
        protected virtual void Hide()
        {
            panel.Hide();
        }
        public virtual void Pause()
        {
            panel.Pause(); 
        }
        public virtual void UnPause()
        {
            panel.UnPause();
        }
        public virtual void Close()
        {
            panel.Close();
        }

        protected UIView Bind(Text text, Func<string> getter)
        {
            handler.BindProperty(() => {
                string tmp = getter();
                if (tmp!=text.text)
                {
                    text.text = tmp;
                }});
            return this;
        }
        protected UIView Bind(InputField input, Func<string> getter)
        {
            handler.BindProperty(() => {
                string tmp = getter();
                if (tmp != input.text)
                {
                    input.text = tmp;
                }
            });
            return this;
        }
        protected UIView Bind(Slider slider, Func<float> getter)
        {
            handler.BindProperty(() => {
                float tmp = getter();
                if (slider.value!=tmp)
                {
                    slider.value = tmp;
                } });
            return this;
        }
        protected UIView Bind(Toggle toggle, Func<bool> getter)
        {
            handler.BindProperty(() => {
                bool tmp = getter();
                if (tmp!=toggle.isOn)
                {
                    toggle.isOn = tmp;
                }});
            return this;
        }

        protected UIView OnValueChanged(InputField input,UnityAction<string> callback)
        {
            input.onValueChanged.AddListener(callback);
            return this;
        }
        protected UIView OnValueChanged(Toggle toggle, UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
            return this;
        }
        protected UIView OnValueChanged(Slider slider, UnityAction<float> callback)
        {
            slider.onValueChanged.AddListener(callback);
            return this;
        }
        protected UIView OnEndEdit(InputField input, UnityAction<string> callback)
        {
            input.onEndEdit.AddListener(callback);
            return this;
        }
        protected UIView OnValidateInput(InputField input, InputField.OnValidateInput callback)
        {
            input.onValidateInput= callback;
            return this;
        }
        protected UIView OnClick(Button button, UnityAction callback)
        {
            button.onClick.AddListener(callback);
            return this;
        }


        protected abstract void OnLoad();
        protected abstract void OnTop(UIEventArgs arg);
        protected abstract void OnPress(UIEventArgs arg);
        protected abstract void OnPop(UIEventArgs arg);
        protected abstract void OnClear();

        void IViewStateEventHandler.OnLoad()
        {
            _lastState = ViewEventTpe.OnLoad;
            OnLoad();
        }
        void IViewStateEventHandler.OnTop(UIEventArgs arg)
        {
            _lastState = ViewEventTpe.OnTop;
            OnTop(arg);
        }
        void IViewStateEventHandler.OnPress(UIEventArgs arg)
        {
            _lastState = ViewEventTpe.OnPress;
            OnPress(arg);
        }
        void IViewStateEventHandler.OnPop(UIEventArgs arg)
        {
            _lastState = ViewEventTpe.OnPop;
            OnPop(arg);
        }
        void IViewStateEventHandler.OnClear()
        {
            _lastState = ViewEventTpe.OnClear;
            OnClear();
        }
    }
    public abstract class UIView<VM, Panel> : UIView where VM : UIViewModel where Panel : UIPanel
    {
        public VM Tcontext { get { return this.context as VM; } }
        public Panel Tpanel { get { return this.panel as Panel; } }
    }
}
