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

namespace IFramework.UI
{
    public class UIEventArgs : RecyclableObject
    {
        public enum Code
        {
            GoBack, GoForward, Push
        }
        public Code code;
        public UIPanel popPanel;
        public UIPanel curPanel;
        public UIPanel pressPanel;
        protected override void OnDataReset()
        {
            popPanel = null;
            curPanel = null;
            pressPanel = null;
        }
    }

    public interface IGroups : IDisposable
    {
        void Update();
        UIPanel FindPanel(string name);
        void InvokeViewState(UIEventArgs arg);
        void Subscribe(UIPanel panel);
        void UnSubscribe(UIPanel panel);
    }
    public interface IPanelLoader
    {
        UIPanel Load(Type type, string name);
    }
    public partial class UIModule : IUIModule
    {
        public Canvas canvas { get; private set; }
        private RectTransform root;
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
        private RectTransform InitTransform(string name)
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




        public void CreateCanvas()
        {
            root = new GameObject(name).AddComponent<RectTransform>();
            canvas = root.gameObject.AddComponent<Canvas>();
            root.gameObject.AddComponent<CanvasScaler>();
            root.gameObject.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            belowBackground = InitTransform("belowBackground");
            background = InitTransform("background");
            belowAnimation = InitTransform("belowAnimation");
            common = InitTransform("common");
            aboveAnimation = InitTransform("aboveAnimation");
            pop = InitTransform("pop");
            guide = InitTransform("Guide");
            toast = InitTransform("Toast");
            top = InitTransform("Top");
            aboveTop = InitTransform("aboveTop");
            camera = InitTransform("UICamera");
        }
        public void SetCanvas(Canvas canvas)
        {
            root = canvas.GetComponent<RectTransform>();
            this.canvas = canvas;
            belowBackground = InitTransform("BGBG");
            background = InitTransform("Background");
            belowAnimation = InitTransform("AnimationUnderPage");
            common = InitTransform("Common");
            aboveAnimation = InitTransform("AnimationOnPage");
            pop = InitTransform("PopUp");
            guide = InitTransform("Guide");
            toast = InitTransform("Toast");
            top = InitTransform("Top");
            aboveTop = InitTransform("TopTop");
            camera = InitTransform("UICamera");
        }
        public void SetCamera(Camera ca, bool isLast = true, int index = -1)
        {
            camera.SetChildWithIndex(ca.transform, !isLast ? index : camera.childCount);
        }
        public void SetLayer(UIPanel ui, bool isLast = true, int index = -1)
        {
            switch (ui.layer)
            {
                case UILayer.BelowBackground:
                    belowBackground.SetChildWithIndex(ui.transform, !isLast ? index : belowBackground.childCount);
                    break;
                case UILayer.Background:
                    background.SetChildWithIndex(ui.transform, !isLast ? index : background.childCount);
                    break;
                case UILayer.BelowAnimation:
                    belowAnimation.SetChildWithIndex(ui.transform, !isLast ? index : belowAnimation.childCount);
                    break;
                case UILayer.Common:
                    common.SetChildWithIndex(ui.transform, !isLast ? index : common.childCount);
                    break;
                case UILayer.AboveAnimation:
                    aboveAnimation.SetChildWithIndex(ui.transform, !isLast ? index : aboveAnimation.childCount);
                    break;
                case UILayer.Pop:
                    pop.SetChildWithIndex(ui.transform, !isLast ? index : pop.childCount);
                    break;
                case UILayer.Guide:
                    guide.SetChildWithIndex(ui.transform, !isLast ? index : guide.childCount);
                    break;
                case UILayer.Toast:
                    toast.SetChildWithIndex(ui.transform, !isLast ? index : toast.childCount);
                    break;
                case UILayer.Top:
                    top.SetChildWithIndex(ui.transform, !isLast ? index : top.childCount);
                    break;
                case UILayer.AboveTop:
                    aboveTop.SetChildWithIndex(ui.transform, !isLast ? index : aboveTop.childCount);
                    break;
                default:
                    break;
            }
            ui.transform.LocalIdentity();
        }
    }
    public partial class UIModule
    {
        private List<IPanelLoader> _loaders;
        public int loaderCount { get { return _loaders.Count; } }

        public Stack<UIPanel> stack;
        public Stack<UIPanel> memory;
        public int stackCount { get { return stack.Count; } }
        public int memoryCount { get { return memory.Count; } }
        public bool ExistInStack(UIPanel panel) { return stack.Contains(panel); }
        public bool ExistInMemory(UIPanel panel) { return memory.Contains(panel); }
        public bool Exist(UIPanel panel) { return ExistInMemory(panel) || ExistInStack(panel); }
        public UIPanel current
        {
            get
            {
                if (stack.Count == 0)
                    return null;
                return stack.Peek();
            }
        }
        public UIPanel MemoryPeek()
        {
            if (memory.Count == 0)
                return null;
            return memory.Peek();
        }
    }
    public partial class UIModule : UpdateFrameworkModule
    {
        private IGroups _groups;
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

        public void AddLoader(IPanelLoader loader)
        {
            _loaders.Add(loader);
        }
        public void SetGroups(IGroups groups)
        {
            this._groups = groups;
        }

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
                ui.layer = layer;
                SetLayer(ui);
                ui.name = name;
                ui.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                _groups.Subscribe(ui);
                return ui;
            }
            Log.E(string.Format("NO ui Type: {0}    Layer: {1}  Name: {2}", type, layer, name));
            return ui;
        }
        public T Load<T>(string name, UILayer layer = UILayer.Common) where T : UIPanel
        {
            return (T)Load(typeof(T), name, layer);
        }
        public bool HaveLoad(string panelName)
        {
            return _groups.FindPanel(panelName) != null;
        }

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
            if (memory.Count > 0) ClearCache();
        }
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
        public void ClearCache()
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
        private void InvokeViewState(UIEventArgs arg)
        {
            _groups.InvokeViewState(arg);
        }

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
        public T Get<T>(string name, UILayer layer = UILayer.Common) where T : UIPanel
        {
            return (T)Get(typeof(T), name, layer);
        }
    }
}
