/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ui 模块存储示意图                                 push/get ( if memory exist panels ,the memory will clear auto )
//                                                  |
//    |--------------------------------------       |                     --------------------------------------|
//    |    stack                               <----|                                                    memory |
//    |                                        <----------- ui ---------- go forward                            |
//    |                                        go back ---- ui ----------------->                               |
//    |--------------------------------------                             --------------------------------------|
//    1     CreateCanvas  or SetCanvas                              -- this set ui module root canvas for uipanels
//    2     AddLoader                                               -- this tell ui module how to load uipanels if the panel have not been loaded
//    3     SetGroups                                               -- this group tell ui module how to handle the uipanel events
//                                                                  -- we can use default group MvvmGroups ; also you can extend your own groups
//
//    4.1   use  (Get , GoForWard , GoBack ,ClearMemory) to  control  uipanels          -- this work with two stacks in module
//          this will call (OnLoad ,  OnPress , OnTop , OnPop , OnClear) in uiview 
//    4.2   use  (Show , Hide , Pause ,UnPause , Close) to  control  uipanels           -- this will not use stacks
//          this will call (OnLoad, OnShow, OnHide, OnPause, OnUnPause , OnClose , OnClear) in uiview
//
//    you can control uipanels with  4.1 or 4.2
//
//    if you can understand (ui moudule) thoroughly  , you  can  use 4.1 and 4.2 at the same time

namespace IFramework.UI
{
    /// <summary>
    /// 字段
    /// </summary>
    partial class UIModule : IUIModule
    {
        #region ui层级
        public Canvas canvas { get; private set; }
        public RectTransform belowBackground { get; private set; }
        public RectTransform background { get; private set; }
        public RectTransform belowAnimation { get; private set; }
        public RectTransform common { get; private set; }
        public RectTransform aboveAnimation { get; private set; }
        public RectTransform pop { get; private set; }
        public RectTransform guide { get; private set; }
        public RectTransform toast { get; private set; }
        public RectTransform top { get; private set; }
        public RectTransform aboveTop { get; private set; }
        public RectTransform camera { get; private set; }
        #endregion
        private IGroups _groups;
        private List<IPanelLoader> _loaders;
        private Dictionary<UILayer, List<WeakReference<UIPanel>>> _panelOrders;
        private Stack<UIPanel> stack;
        private Stack<UIPanel> memory;
        /// <summary>
        /// 加载器个数
        /// </summary>
        public int loaderCount { get { return _loaders.Count; } }
        /// <summary>
        /// stack 数量
        /// </summary>
        public int stackCount { get { return stack.Count; } }
        /// <summary>
        /// memory 数量
        /// </summary>
        public int memoryCount { get { return memory.Count; } }
        /// <summary>
        /// stack top
        /// </summary>
        public UIPanel current
        {
            get
            {
                if (stack.Count == 0)
                    return null;
                return stack.Peek();
            }
        }

    }
    /// <summary>
    /// 私有方法
    /// </summary>
    partial class UIModule
    {
        private RectTransform CreateLayer(string name)
        {
            GameObject go = new GameObject(name);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetParent(canvas.transform);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.localPosition = Vector3.zero;
            rect.sizeDelta = Vector3.zero;
            return rect;
        }
        private void CreateLayers()
        {
            belowBackground = CreateLayer("belowBackground");
            background = CreateLayer("background");
            belowAnimation = CreateLayer("belowAnimation");
            common = CreateLayer("common");
            aboveAnimation = CreateLayer("aboveAnimation");
            pop = CreateLayer("pop");
            guide = CreateLayer("Guide");
            toast = CreateLayer("Toast");
            top = CreateLayer("Top");
            aboveTop = CreateLayer("aboveTop");
            camera = CreateLayer("UICamera");
        }
        private RectTransform GetLayerParent(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.BelowBackground: return belowBackground;
                case UILayer.Background: return background;
                case UILayer.BelowAnimation: return belowAnimation;
                case UILayer.Common: return common;
                case UILayer.AboveAnimation: return aboveAnimation;
                case UILayer.Pop: return pop;
                case UILayer.Guide: return guide;
                case UILayer.Toast: return toast;
                case UILayer.Top: return top;
                case UILayer.AboveTop: return aboveTop;
                default: return null;
            }
        }
        private void InvokeViewState(UIEventArgs arg)
        {
            if (arg.pressPanel != null)
                _groups.OnPress(arg.pressPanel.name,arg);
            if (arg.popPanel != null)
                _groups.OnPop(arg.popPanel.name, arg);
            if (arg.curPanel != null)
                _groups.OnTop(arg.curPanel.name,arg);
            arg.SetDirty();
            arg.Recyle();
        }
        private List<UIPanel> _orderHelp = new List<UIPanel>();
        private void SetOrder(UIPanel panel)
        {
            UILayer layer = panel.layer;
            int order = panel.layerOrder;
            if (!_panelOrders.ContainsKey(layer))
                _panelOrders.Add(layer, new List<WeakReference<UIPanel>>());
            var list = _panelOrders[layer];
            _orderHelp.Clear();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                UIPanel _tmp;
                if (!list[i].TryGetTarget(out _tmp))
                {
                    list.Remove(list[i]);
                }
                else
                {
                    _orderHelp.Add(_tmp);
                }
            }
            if (_orderHelp.Contains(panel)) return;
            _orderHelp.Sort((a, b) => { return a.layerOrder - b.layerOrder; });
            int sbindex = 0;
            bool bigExist = false;
            for (int i = 0; i < _orderHelp.Count; i++)
            {
                if (_orderHelp[i].layerOrder > order)
                {
                    sbindex = _orderHelp[i].transform.GetSiblingIndex();
                    bigExist = true;
                    break;
                }
            }
            if (bigExist)
            {
                panel.transform.SetSiblingIndex(sbindex);
            }
            list.Add(new WeakReference<UIPanel>(panel));
        }
    }
    /// <summary>
    /// 常用
    /// </summary>
    partial class UIModule
    {
        /// <summary>
        /// 创建 画布
        /// </summary>
        public void CreateCanvas()
        {
            var root = new GameObject(name);
            root.AddComponent<RectTransform>();
            canvas = root.AddComponent<Canvas>();
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CreateLayers();
        }
        /// <summary>
        /// 设置画布
        /// </summary>
        /// <param name="canvas"></param>
        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            canvas.name = name;
            CreateLayers();
        }


        /// <summary>
        /// 设置加载器
        /// </summary>
        /// <param name="loader"></param>
        public void AddLoader(IPanelLoader loader)
        {
            _loaders.Add(loader);
        }
        /// <summary>
        /// 设置ui组管理器
        /// </summary>
        /// <param name="groups"></param>
        public void SetGroups(IGroups groups)
        {
            this._groups = groups;
        }

        /// <summary>
        /// 获取 ui
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIPanel Get(string name)
        {
            //if (UICache.Count > 0) ClearCache(arg);
            if (current != null && current.name == name)
                return current;
            var panel = _groups.FindPanel(name);
            if (panel == null)
                panel = Load(name);
            Push(panel);
            return panel;
        }
        /// <summary>
        /// 前进
        /// </summary>
        public bool GoForWard()
        {
            if (memoryCount <= 0) return false;
            UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
            arg.code = UIEventArgs.Code.GoForward;
            if (stackCount > 0)
                arg.pressPanel = current;
            var ui = memory.Pop();
            arg.curPanel = ui;
            stack.Push(ui);
            InvokeViewState(arg);
            //arg.Recyle();
            return true;
        }
        /// <summary>
        /// 后退
        /// </summary>
        public bool GoBack()
        {
            if (stackCount <= 0) return false;
            UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
            arg.code = UIEventArgs.Code.GoBack;
            var ui = stack.Pop();
            arg.popPanel = ui;

            memory.Push(ui);
            if (stackCount > 0)
                arg.curPanel = current;

            InvokeViewState(arg);
            //arg.Recyle();
            return true;
        }
        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearMemory()
        {
            while (memory.Count != 0)
            {
                UIPanel p = memory.Pop();
                if (p != null && !ExistInStack(p))
                {
                    _groups.UnSubscribe(p);
                }
            }
        }
    }
    /// <summary>
    /// 额外
    /// </summary>
    partial class UIModule
    {
        /// <summary>
        /// 展示一个界面
        /// </summary>
        /// <param name="name"></param>
        /// <param name="addtoStack"></param>
        public void Show(string name)
        {
            if (current != null && current.name == name) return;
            var panel = FindPanel(name);
            if (panel == null)
                panel = Load(name);
            this._groups.OnShow(name);
        }
        /// <summary>
        /// 藏一个界面
        /// </summary>
        /// <param name="name"></param>
        public void Hide(string name)
        {
            var panel = FindPanel(name);
            if (panel != null)
            {
                this._groups.OnHide(name);
            }
        }
        /// <summary>
        /// 挂起一个界面
        /// </summary>
        /// <param name="name"></param>
        public void Pause(string name)
        {
            var panel = FindPanel(name);
            if (panel != null)
            {
                this._groups.OnPause(name);
            }
        }
        /// <summary>
        /// 重新启用一个界面
        /// </summary>
        /// <param name="name"></param>
        public void UnPause(string name)
        {
            var panel = FindPanel(name);
            if (panel != null)
            {
                this._groups.OnUnPause(name);
            }
        }
        /// <summary>
        /// 彻底关闭一个界面
        /// </summary>
        /// <param name="name"></param>
        public void Close(string name)
        {
            var panel = FindPanel(name);

            if (panel != null)
            {
                if (!ExistInStack(panel))
                {
                    this._groups.OnClose(name);
                    _groups.UnSubscribe(panel);
                }
            }
        }

        /// <summary>
        /// 放置相机到 ui模块的 camera 层级
        /// </summary>
        /// <param name="camera"></param>
        public void PutCamera(Camera camera)
        {
            camera.transform.SetParent(this.camera);
        }


        /// <summary>
        /// stack 中是否存在 ui
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public bool ExistInStack(UIPanel panel) { return stack.Contains(panel); }
        /// <summary>
        /// memory 中是否存在 ui
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public bool ExistInMemory(UIPanel panel) { return memory.Contains(panel); }
        /// <summary>
        /// 是否 存在 ui
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public bool Exist(UIPanel panel) { return ExistInMemory(panel) || ExistInStack(panel); }
        /// <summary>
        /// 获取 memory 的第一个 ui
        /// </summary>
        /// <returns></returns>
        public UIPanel MemoryPeek()
        {
            if (memory.Count == 0)
                return null;
            return memory.Peek();
        }



        /// <summary>
        /// 加载 ui 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIPanel Load(string name)
        {
            if (_groups == null)
                throw new Exception("Please Set ModuleType First");
            UIPanel ui = default(UIPanel);
            if (_loaders == null || loaderCount == 0)
            {
                Log.E("NO UILoader");
                return ui;
            }
            for (int i = 0; i < _loaders.Count; i++)
            {
                var result = _loaders[i].Load(ref name);
                if (result == null) continue;
                ui = result;
                ui = GameObject.Instantiate(ui, GetLayerParent(ui.layer));
                ui.name = name;
                ui.module = this;
                SetOrder(ui);
                _groups.Subscribe(ui);
                return ui;
            }
            Log.E(string.Format("Can't load ui with Name: {0}", name));
            return ui;
        }

        /// <summary>
        /// 查找 ui
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIPanel FindPanel(string name)
        {
            return _groups.FindPanel(name);
        }
        /// <summary>
        /// 推 ui 入 stack
        /// </summary>
        /// <param name="ui"></param>
        public void Push(UIPanel ui)
        {
            if (ui.addToStack)
            {
                UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
                arg.code = UIEventArgs.Code.Push;
                if (stackCount > 0)
                    arg.pressPanel = current;
                arg.curPanel = ui;
                stack.Push(ui);
                InvokeViewState(arg);
                if (memory.Count > 0) ClearMemory();
            }
        }
    }
    /// <summary>
    /// 基础内容
    /// </summary>
    public partial class UIModule : UpdateFrameworkModule, IUIModule
    {
        public override int priority { get { return 80; } }

        protected override void Awake()
        {
            stack = new Stack<UIPanel>();
            memory = new Stack<UIPanel>();
            _loaders = new List<IPanelLoader>();
            _panelOrders = new Dictionary<UILayer, List<WeakReference<UIPanel>>>();
        }
        protected override void OnDispose()
        {
            stack.Clear();
            memory.Clear();
            _loaders.Clear();
            if (_groups != null)
                _groups.Dispose();
            if (canvas != null)
                GameObject.Destroy(canvas.gameObject);
        }
        protected override void OnUpdate()
        {
            if (_groups != null)
            {
                _groups.Update();
            }
        }
    }
}
