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
    public class MvvmGroups:IGroups
    {
        internal interface IViewStateEventHandler
        {
            void OnLoad();
            void OnTop(UIEventArgs arg);
            void OnPress(UIEventArgs arg);
            void OnPop(UIEventArgs arg);
            void OnClear();
        }
        private MVVMModule _moudule;
        private Dictionary<Type, Tuple<Type, Type, Type>> _map;

        public MvvmGroups(Dictionary<Type, Tuple<Type, Type, Type>> map)
        {
            _moudule = MVVMModule.CreatInstance<MVVMModule>("UIGroup");
            this._map = map;
        }

        private MVVMGroup FindGroup(string name)
        {
            return _moudule.FindGroup(name);
        }
        private MVVMGroup FindGroup(UIPanel panel)
        {
            return FindGroup(panel.name);
        }




        UIPanel IGroups.FindPanel(string name)
        {
            var group = FindGroup(name);
            if (group == null) return null;
                return (group.view as UIView).panel;
        }
        void IGroups.InvokeViewState(UIEventArgs arg)
        {
            if (arg.pressPanel != null)
                (FindGroup(arg.pressPanel).view as IViewStateEventHandler).OnPress(arg);
            if (arg.popPanel != null)
                (FindGroup(arg.popPanel).view as IViewStateEventHandler).OnPop(arg);
            if (arg.curPanel != null)
                (FindGroup(arg.curPanel).view as IViewStateEventHandler).OnTop(arg);
        }
        void IGroups.Subscribe(UIPanel panel)
        {
            var _group = FindGroup(panel);
            if (_group != null) throw new Exception(string.Format("Have Subscribe Panel Name: {0}", panel.name));

            Tuple<Type, Type, Type> tuple;
            _map.TryGetValue(panel.GetType(), out tuple);
            if (tuple == null) throw new Exception(string.Format("Could Not Find map with Type: {0}", panel.GetType()));


            var model = Activator.CreateInstance(tuple.Item1) as IModel;
            var view = Activator.CreateInstance(tuple.Item2) as UIView;
            var vm = Activator.CreateInstance(tuple.Item3) as UIViewModel;
            view.panel = panel;

            var group = new MVVMGroup(panel.name, view, vm, model);
            _moudule.AddGroup(group);
            (view as IViewStateEventHandler).OnLoad();
        }
        void IGroups.UnSubscribe(UIPanel panel)
        {
            var group = FindGroup(panel);
            if (group != null)
            {
                (group.view as IViewStateEventHandler).OnClear();
                group.Dispose();
            }
        }
        void IDisposable.Dispose()
        {
            _moudule.Dispose();
        }
        void IGroups.Update()
        {
            _moudule.Update();
        }
    }
}
