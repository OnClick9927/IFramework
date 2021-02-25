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

//ui 模块存储示意图                                 push/get
//                                                  |
//    |--------------------------------------       |                     --------------------------------------|
//    |    stack                               <----|                                                    memory |
//    |                                        <----------- ui ---------- go forward                            |
//    |                                        go back ---- ui ----------------->                               |
//    |--------------------------------------                             --------------------------------------|
namespace IFramework.UI
{
    /// <summary>
    /// 字段
    /// </summary>
    partial class UIModule 
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
    /// 常用
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
        /// <param name="layer"></param>
        /// <returns></returns>
        public UIPanel Get(Type type, string name, UILayer layer = UILayer.Common)
        {
            //if (UICache.Count > 0) ClearCache(arg);
            if (current != null && current.name == name && current.GetType() == type)
                return current;
            var panel = _groups.FindPanel(name);
            if (panel == null)
                panel = Load(type, name, layer);
            Push(panel);
            return panel;
        }
        /// <summary>
        /// 获取ui
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public T Get<T>(string name, UILayer layer = UILayer.Common) where T : UIPanel
        {
            return (T)Get(typeof(T), name, layer);
        }
        /// <summary>
        /// 前进
        /// </summary>
        public void GoForWard()
        {
            UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
            arg.code = UIEventArgs.Code.GoForward;
            if (memoryCount <= 0) return;
            if (stackCount > 0)
                arg.pressPanel = current;
            var ui = memory.Pop();
            arg.curPanel = ui;
            stack.Push(ui);
            InvokeViewState(arg);
            arg.Recyle();
        }
        /// <summary>
        /// 后退
        /// </summary>
        public void GoBack()
        {
            UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
            arg.code = UIEventArgs.Code.GoBack;
            if (stackCount <= 0) return;
            var ui = stack.Pop();
            arg.popPanel = ui;
            memory.Push(ui);
            if (stackCount > 0)
                arg.curPanel = current;
            InvokeViewState(arg);
            arg.Recyle();
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
        /// 放置相机到 ui模块的 camera 层级
        /// </summary>
        /// <param name="camera"></param>
        public void PutCamera(Camera camera)
        {
            camera.transform.SetParent(this.camera);
        }
        /// <summary>
        /// 放置ui 到对应的层级
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="layer"></param>
        public void PutPanel(UIPanel panel,UILayer layer)
        {
            RectTransform tmp = null;
            switch (layer)
            {
                case UILayer.BelowBackground: tmp = belowBackground; break;
                case UILayer.Background: tmp = background; break;
                case UILayer.BelowAnimation: tmp = belowAnimation; break;
                case UILayer.Common: tmp = common; break;
                case UILayer.AboveAnimation: tmp = aboveAnimation; break;
                case UILayer.Pop: tmp = pop; break;
                case UILayer.Guide: tmp = guide; break;
                case UILayer.Toast: tmp = toast; break;
                case UILayer.Top: tmp = top; break;
                case UILayer.AboveTop: tmp = aboveTop; break;
                default: break;
            }
            panel.transform.SetParent(tmp);
            panel.transform.LocalIdentity();
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
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public UIPanel Load(Type type, string name, UILayer layer = UILayer.Common)
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
                var result = _loaders[i].Load(type, name);
                if (result == null) continue;
                ui = result;
                ui = GameObject.Instantiate(ui);
                PutPanel(ui, layer);
                ui.name = name;
                ui.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                _groups.Subscribe(ui);
                return ui;
            }
            Log.E(string.Format("NO ui Type: {0}    Layer: {1}  Name: {2}", type, layer, name));
            return ui;
        }
        /// <summary>
        /// 加载 ui
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public T Load<T>(string name, UILayer layer = UILayer.Common) where T : UIPanel
        {
            return (T)Load(typeof(T), name, layer);
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
            UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
            arg.code = UIEventArgs.Code.Push;
            if (stackCount > 0)
                arg.pressPanel = current;
            arg.curPanel = ui;
            stack.Push(ui);
            InvokeViewState(arg);
            arg.Recyle();
            if (memory.Count > 0) ClearMemory();
        }
        private void InvokeViewState(UIEventArgs arg)
        {
            _groups.InvokeViewState(arg);
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
            if (_groups!=null)
            {
                 _groups.Update();
            }
        }
    }
}
