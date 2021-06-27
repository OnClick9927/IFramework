/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IFramework.GUITool.ToorbarMenu
{
    public abstract class ToolbarNode 
    {
        public ToolBarTree tree;
        protected GUIContent content;
        protected float width;
        protected Func<bool> canshowFunc;
        public bool canshow
        {
            get
            {
                if (canshowFunc == null)
                {
                    return true;
                }
                return canshowFunc();
            }
        }
        public ToolbarNode(GUIContent content, int width = 100, Func<bool> canshow=null) { this.width = width; this.content = content; this.canshowFunc = canshow; }
        public abstract void OnGUI();
    }

    class ToolBarSpace : ToolbarNode
    {
        public ToolBarSpace(int width=100, Func<bool> canshow=null) : base(null,width,canshow) { }
        public override void OnGUI()
        {
            GUILayout.Space(width);
        }
    }
    class ToolBarFlexibleSpace : ToolbarNode
    {
        public ToolBarFlexibleSpace(Func<bool> canshow=null) : base(null, 0,canshow) { }
        public override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }
    class DelegateLabel : ToolbarNode
    {
        public event Action<Rect> panDel;
        public DelegateLabel(Action<Rect> panDel, int width=100, Func<bool> canshow=null) : base(null, width,canshow) { this.panDel = panDel; }

        public override void OnGUI()
        {
            switch (tree.type)
            {
                case BarType.Horzontal:
                    GUILayout.Label("", GUILayout.Width(width));
                    break;
                case BarType.Vertical:
                    GUILayout.Label("", GUILayout.Height(width));
                    break;
                default:
                    break;
            }
            if (panDel != null)
                panDel(GUILayoutUtility.GetLastRect());
        }
    }
    class ToolBarLabel : ToolbarNode
    {
        public ToolBarLabel(GUIContent content, int width=100, Func<bool> canshow=null) : base(content, width,canshow) { }

        public override void OnGUI()
        {
            switch (tree.type)
            {
                case BarType.Horzontal:
                    GUILayout.Label(content, GUILayout.Width(width));
                    break;
                case BarType.Vertical:
                    GUILayout.Label(content, GUILayout.Height(width));

                    break;
                default:
                    break;
            }
        }
    }
    class ToolBarToolTip : ToolbarNode
    {
        public ToolBarToolTip(GUIContent content, int width=100, Func<bool> canshow=null) : base(content, width,canshow) { }

        public override void OnGUI()
        {
            switch (tree.type)
            {
                case BarType.Horzontal:
                    GUILayout.Label(content, GUIStyles.Tooltip, GUILayout.Width(width));
                    break;
                case BarType.Vertical:
                    GUILayout.Label(content, GUIStyles.Tooltip, GUILayout.Height(width));

                    break;
                default:
                    break;
            }
        }
    }
    class ToolBarSearchField : ToolbarNode
    {

        private event Action<string> onValueChange;
        public ToolBarSearchField(Action<string> onValueChange, string value, int width=100 ,Func<bool> canshow=null) : base(null, width, canshow) {

            this.onValueChange = onValueChange;
            s = new SearchField("", null, 0);

            s.onValueChange += (str) => {
                if (this.onValueChange != null)
                    this.onValueChange(str);
                value = str;
            };
        }

        private SearchField s;
        public override void OnGUI()
        {
            switch (tree.type)
            {
                case BarType.Horzontal:
                    GUILayout.Label("", GUILayout.Width(width));
                    break;
                case BarType.Vertical:
                    GUILayout.Label("", GUILayout.Height(width));
                    break;
                default:
                    break;
            }
            s.OnGUI(GUILayoutUtility.GetLastRect());
        }
    }
    class ToolBarButton : ToolbarNode
    {
        private event Action<Rect> onClick;
        public ToolBarButton(GUIContent content, Action<Rect> onClick, int width=100 ,Func<bool> canshow=null) : base(content, width, canshow) { this.onClick = onClick; }

        public override void OnGUI()
        {
            switch (tree.type)
            {
                case BarType.Horzontal:
                    GUILayout.Label("", GUILayout.Width(width));
                    break;
                case BarType.Vertical:
                    GUILayout.Label("", GUILayout.Height(width));
                    break;
                default:
                    break;
            }
            Rect r = GUILayoutUtility.GetLastRect();
            if (GUI.Button(r, content, GUIStyles.toolbarbutton))
            {
                if (onClick != null) onClick(r);
            }
        }
    }
    class ToolBarDropDownButton : ToolbarNode
    {
        private event Action<Rect> onClick;
        public ToolBarDropDownButton(GUIContent content, Action<Rect> onClick, int width=100,Func<bool> canshow=null) : base(content, width, canshow) { this.onClick = onClick; }

        public override void OnGUI()
        {
            switch (tree.type)
            {
                case BarType.Horzontal:
                    GUILayout.Label("", GUILayout.Width(width));
                    break;
                case BarType.Vertical:
                    GUILayout.Label("", GUILayout.Height(width));
                    break;
                default:
                    break;
            }
            Rect r = GUILayoutUtility.GetLastRect();
            if (GUI.Button(r, content, GUIStyles.ToolbarDropDown))
            {
                if (onClick != null) onClick(r);
            }
        }
    }
    class ToolBarToggle : ToolbarNode
    {
        private bool value;
        private event Action<bool> onValueChange;

        public ToolBarToggle(GUIContent content, Action<bool> onValueChange, bool value = false , int width=100 , Func<bool> canshow=null) : base(content, width,canshow)
        {
            this.onValueChange = onValueChange;
            this.value = value;
        }

        public override void OnGUI()
        {
                    bool val = value;
            switch (tree.type)
            {
                case BarType.Horzontal:
                    val= GUILayout.Toggle( val, content, GUIStyles.toolbarbutton, GUILayout.Width(width));
                    break;
                case BarType.Vertical:
                    val= GUILayout.Toggle( val, content, GUIStyles.toolbarbutton, GUILayout.Height(width));
                    break;
                default:
                    break;
            }
            if (val != value)
            {
                value = val;
                if (onValueChange != null) onValueChange(value);
            }
        }
    }


    class ToolBarPopup : ToolbarNode
    {
        private int value = 0;
        private string[] ops;
        private Action<int> onValueChange;

        public ToolBarPopup(Action<int> onValueChange,  string[] ops, int value = 0, int width=100, Func<bool> canshow=null) : base(null, width, canshow)
        {
            this.value = value;
            this.ops = ops;
            this.onValueChange = onValueChange;
        }

        public override void OnGUI()
        {
            int tmp = 0;
            switch (tree.type)
            {
                case BarType.Horzontal:
                    tmp = EditorGUILayout.Popup(value, ops, GUIStyles.ToolbarDropDown, GUILayout.Width(width));
                    break;
                case BarType.Vertical:
                    tmp = EditorGUILayout.Popup(value, ops, GUIStyles.ToolbarDropDown, GUILayout.Height(width));
                    break;
                default:
                    break;
            }
            if (tmp != value)
            {
                value = tmp;
                if (onValueChange != null)
                {
                    onValueChange(value);
                }
            }
        }
    }
    public enum BarType { Horzontal,Vertical}
    public class ToolBarTree:GUIBase
    {
        public BarType type = BarType.Horzontal;
        private List<ToolbarNode> _nodes = new List<ToolbarNode>();
        public override void OnGUI(Rect position)
        {
            GUIStyles.ToolBar.fixedHeight = position.height;
            GUILayout.BeginArea(position);
            switch (type)
            {
                case BarType.Horzontal:
                    GUILayout.BeginHorizontal(GUIStyles.ToolBar, GUILayout.Width(position.width));
                    _nodes.ForEach((n) =>
                    {
                        if (n.canshow)
                        {
                            n.OnGUI();
                        }
                    });
                    GUILayout.EndHorizontal();
                    break;
                case BarType.Vertical:
                    GUILayout.BeginVertical(GUIStyles.ToolBar, GUILayout.Width(position.width));
                    _nodes.ForEach((n) =>
                    {
                        if (n.canshow)
                        {
                            n.OnGUI();
                        }
                    });
                    GUILayout.EndVertical();
                    break;
                default:
                    break;
            }
            GUILayout.EndArea();
        }
        protected override void OnDispose()
        {
            _nodes.Clear();
        }
        public ToolBarTree DockNode(ToolbarNode node)
        {
            node.tree = this;
            _nodes.Add(node);
            return this;
        }
    }


    public static class ToolBarTreeEx
    {
        public static ToolBarTree Popup(this ToolBarTree t, Action<int> onValueChange, string[] ops, int value = 0, int width = 100, Func<bool> canshow = null)
        {
            ToolBarPopup btn = new ToolBarPopup(onValueChange, ops, value, width,canshow);
           t. DockNode(btn);
            return t;
        }
        public static ToolBarTree Button(this ToolBarTree t, GUIContent content, Action<Rect> onClick, int width = 100, Func<bool> canshow = null)
        {
            ToolBarButton btn = new ToolBarButton(content, onClick, width,canshow);
            t.DockNode(btn);
            return t;
        }
        public static ToolBarTree DropDownButton(this ToolBarTree t, GUIContent content, Action<Rect> onClick, int width = 100, Func<bool> canshow = null)
        {
            ToolBarDropDownButton btn = new ToolBarDropDownButton(content, onClick, width,canshow);
            t.DockNode(btn);
            return t;
        }
        public static ToolBarTree Toggle(this ToolBarTree t, GUIContent content, Action<bool> onValueChange, bool value = false, int width = 100, Func<bool> canshow = null)
        {
            ToolBarToggle tog = new ToolBarToggle(content, onValueChange, value, width,canshow);
            t.DockNode(tog);
            return t;
        }
        public static ToolBarTree SearchField(this ToolBarTree t, Action<string> onValueChange, string value, int width = 100, Func<bool> canshow = null)
        {
            ToolBarSearchField tog = new ToolBarSearchField(onValueChange, value, width,canshow);
            t.DockNode(tog);
            return t;
        }
        public static ToolBarTree Label(this ToolBarTree t, GUIContent content, int width = 100, Func<bool> canshow = null)
        {
            ToolBarLabel la = new ToolBarLabel(content, width,canshow);
            t.DockNode(la);
            return t;
        }
        public static ToolBarTree ToolTip(this ToolBarTree t, GUIContent content, int width = 100, Func<bool> canshow = null)
        {
            ToolBarToolTip la = new ToolBarToolTip(content, width,canshow);
            t.DockNode(la);
            return t;
        }
        public static ToolBarTree Space(this ToolBarTree t, int width = 100, Func<bool> canshow = null)
        {
            ToolBarSpace sp = new ToolBarSpace(width,canshow);
            t.DockNode(sp);
            return t;
        }
        public static ToolBarTree Delegate(this ToolBarTree t, Action<Rect> panDel, int width = 100, Func<bool> canshow = null)
        {
            DelegateLabel sp = new DelegateLabel(panDel, width,canshow);
            t.DockNode(sp);
            return t;
        }

        public static ToolBarTree FlexibleSpace(this ToolBarTree t, Func<bool> canshow = null)
        {
            ToolBarFlexibleSpace sp = new ToolBarFlexibleSpace(canshow);
            t.DockNode(sp);
            return t;
        }

    }
}
