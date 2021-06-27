/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using IFramework.Modules.MVVM;

namespace IFramework.UI
{
    /// <summary>
    /// ui MVVM 组容器
    /// </summary>
    public class MvvmGroups : IGroups
    {
        internal interface IViewStateEventHandler
        {
            void OnLoad();
            void OnTop(UIEventArgs arg);
            void OnPress(UIEventArgs arg);
            void OnPop(UIEventArgs arg);
            void OnClear();

            void OnShow();

            void OnHide();

            void OnPause();
            void OnUnPause();

            void OnClose();
        }
        private MVVMModule _moudule;
        private Dictionary<string, Tuple<Type, Type, Type>> _map;

        public MvvmGroups(Dictionary<string, Tuple<Type, Type, Type>> map)
        {
            _moudule = MVVMModule.CreatInstance<MVVMModule>("UIGroup");
            this._map = map;
        }

        private MVVMGroup FindGroup(string name)
        {
            return _moudule.FindGroup(name);
        }




        UIPanel IGroups.FindPanel(string name)
        {
            var group = FindGroup(name);
            if (group == null) return null;
            return (group.view as UIView).panel;
        }
        bool IGroups.Subscribe(UIPanel panel)
        {
            var _group = FindGroup(panel.name);
            if (_group != null) {
                Log.E(string.Format("Have Subscribe Panel Name: {0} ready", panel.name));
                return false;
            } 

            Tuple<Type, Type, Type> tuple;
            _map.TryGetValue(panel.name, out tuple);
            if (tuple == null)
            {
                Log.L(string.Format("Could Not Find map with Type: {0}", panel.name));
                return false;
            } 
            var model = Activator.CreateInstance(tuple.Item1) as IModel;

            var view = Activator.CreateInstance(tuple.Item2) as UIView;
            var vm = Activator.CreateInstance(tuple.Item3) as UIViewModel;
            view.panel = panel;

            var group = new MVVMGroup(panel.name, view, vm, model);
            _moudule.AddGroup(group);
            (view as IViewStateEventHandler).OnLoad();
            return true;
        }
        bool IGroups.UnSubscribe(UIPanel panel)
        {
            var group = FindGroup(panel.name);
            if (group != null)
            {
                (group.view as IViewStateEventHandler).OnClear();
                group.Dispose();
                return true;
            }
            return false;
        }
        void IDisposable.Dispose()
        {
            _moudule.Dispose();
        }
        void IGroups.Update()
        {
            _moudule.Update();
        }

        void IGroups.OnShow(string panel)
        {
            (FindGroup(panel).view as IViewStateEventHandler).OnShow();
        }

        void IGroups.OnHide(string panel)
        {
            (FindGroup(panel).view as IViewStateEventHandler).OnHide();

        }

        void IGroups.OnPause(string panel)
        {
            (FindGroup(panel).view as IViewStateEventHandler).OnPause();

        }

        void IGroups.OnUnPause(string panel)
        {
            (FindGroup(panel).view as IViewStateEventHandler).OnUnPause();
        }

        void IGroups.OnClose(string panel)
        {
            (FindGroup(panel).view as IViewStateEventHandler).OnClose();
        }

         void IGroups.OnPress(string panel, UIEventArgs arg)
        {
          (FindGroup(panel).view as IViewStateEventHandler).OnPress(arg);

        }

         void IGroups.OnTop(string panel, UIEventArgs arg)
        {
            (FindGroup(panel).view as IViewStateEventHandler).OnTop(arg);
        }

        void IGroups.OnPop(string panel, UIEventArgs arg)
        {
            (FindGroup(panel).view as IViewStateEventHandler).OnPop(arg);
        }
    }
}
