/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.318
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using IFramework.UI;

namespace IFramework.Hotfix.Lua
{
    public class LuaGroups : IGroups
    {
        public event Action onDispose;
        public event Func<string, UIPanel> onFindPanel;
        public event Action<string, UIEventArgs> onPop;
        public event Action<string, UIEventArgs> onPress;
        public event Action<string, UIEventArgs> onTop;

        public event Func<UIPanel, bool> onSubscribe;
        public event Func<UIPanel, bool> onUnSubscribe;
        public event Action<string> onShow;
        public event Action<string> onHide;
        public event Action<string> onPause;
        public event Action<string> onUnPause;
        public event Action<string> onClose;

        void IDisposable.Dispose()
        {
            if (onDispose != null)
            {
                onDispose();
            }
            onDispose = null;
            onFindPanel = null;
            onSubscribe = null;
            onUnSubscribe = null;
        }
        void IGroups.OnPop(string name, UIEventArgs arg)
        {
            if (onPop != null)
            {
                onPop(name, arg);
            }
        }

        void IGroups.OnPress(string name, UIEventArgs arg)
        {
            if (onPress != null)
            {
                onPress(name, arg);
            }
        }
        void IGroups.OnTop(string name, UIEventArgs arg)
        {
            if (onTop != null)
            {
                onTop(name, arg);
            }
        }


        UIPanel IGroups.FindPanel(string name)
        {
            if (onFindPanel != null)
            {
                return onFindPanel(name);
            }
            return null;
        }





        void IGroups.OnClose(string name)
        {
            if (onClose != null)
            {
                onClose.Invoke(name);
            }
        }

        void IGroups.OnHide(string name)
        {
            if (onHide != null)
            {
                onHide.Invoke(name);
            }
        }

        void IGroups.OnPause(string name)
        {
            if (onPause != null)
            {
                onPause.Invoke(name);
            }
        }



        void IGroups.OnShow(string name)
        {
            if (onShow != null)
            {
                onShow.Invoke(name);
            }
        }


        void IGroups.OnUnPause(string name)
        {
            if (onUnPause != null)
            {
                onUnPause.Invoke(name);
            }
        }

        bool IGroups.Subscribe(UIPanel panel)
        {

            if (onSubscribe != null)
            {
                return onSubscribe(panel);
            }
            return false;
        }

        bool IGroups.UnSubscribe(UIPanel panel)
        {
            if (onUnSubscribe != null)
            {
                return onUnSubscribe(panel);
            }

            return false;
        }

        void IGroups.Update()
        {

        }
    }

}
