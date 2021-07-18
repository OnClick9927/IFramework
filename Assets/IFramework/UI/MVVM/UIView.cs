/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1.440
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Modules.Message;
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
        private ViewEventType _lastState = ViewEventType.None;

        public ViewEventType lastState { get { return _lastState; } }
        protected void Publish(IEventArgs args)
        {
            this.message.Publish(this.GetType(), args);
        }
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

        protected UIView Bind(Text text, Func<string> getter)
        {
            handler.BindProperty(() => {
                string tmp = getter();
                if (tmp != text.text)
                {
                    text.text = tmp;
                }
            });
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
                if (slider.value != tmp)
                {
                    slider.value = tmp;
                }
            });
            return this;
        }
        protected UIView Bind(Toggle toggle, Func<bool> getter)
        {
            handler.BindProperty(() => {
                bool tmp = getter();
                if (tmp != toggle.isOn)
                {
                    toggle.isOn = tmp;
                }
            });
            return this;
        }

        protected UIView BindInputField(InputField input, UnityAction<string> callback)
        {
            input.onValueChanged.AddListener(callback);
            return this;
        }
        protected UIView BindToggle(Toggle toggle, UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
            return this;
        }
        protected UIView BindSlider(Slider slider, UnityAction<float> callback)
        {
            slider.onValueChanged.AddListener(callback);
            return this;
        }
        protected UIView BindOnEndEdit(InputField input, UnityAction<string> callback)
        {
            input.onEndEdit.AddListener(callback);
            return this;
        }
        protected UIView BindOnValidateInput(InputField input, InputField.OnValidateInput callback)
        {
            input.onValidateInput = callback;
            return this;
        }
        protected UIView BindButton(Button button, UnityAction callback)
        {
            button.onClick.AddListener(callback);
            return this;
        }


        protected abstract void OnLoad();
        protected abstract void OnTop(UIEventArgs arg);
        protected abstract void OnPress(UIEventArgs arg);
        protected abstract void OnPop(UIEventArgs arg);
        protected abstract void OnClear();

        protected abstract void OnShow();
        protected abstract void OnHide();
        protected abstract void OnPause();
        protected abstract void OnUnPause();
        protected abstract void OnClose();

        void IViewStateEventHandler.OnLoad()
        {
            _lastState = ViewEventType.OnLoad;
            OnLoad();
        }
        void IViewStateEventHandler.OnTop(UIEventArgs arg)
        {
            _lastState = ViewEventType.OnTop;
            OnTop(arg);
        }
        void IViewStateEventHandler.OnPress(UIEventArgs arg)
        {
            _lastState = ViewEventType.OnPress;
            OnPress(arg);
        }
        void IViewStateEventHandler.OnPop(UIEventArgs arg)
        {
            _lastState = ViewEventType.OnPop;
            OnPop(arg);
        }
        void IViewStateEventHandler.OnClear()
        {
            _lastState = ViewEventType.OnClear;
            OnClear();
        }

        void IViewStateEventHandler.OnShow()
        {
            OnShow();
        }

        void IViewStateEventHandler.OnHide()
        {
            OnHide();
        }

        void IViewStateEventHandler.OnPause()
        {
            OnPause();
        }

        void IViewStateEventHandler.OnUnPause()
        {
            OnUnPause();
        }

        void IViewStateEventHandler.OnClose()
        {
            OnClose();
        }
    }
    public abstract class UIView<VM, Panel> : UIView where VM : UIViewModel where Panel : UIPanel
    {
        public VM Tcontext { get { return this.context as VM; } }
        public Panel Tpanel { get { return this.panel as Panel; } }
    }
}
