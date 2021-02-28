/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.271
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool
{
    public class MenuTree:GUIBase
    {
        public class MenuTrunk
        {
            public string name { get; private set; }
            public float height { get {return  fit ? 30 : 0; } }
            public bool isOn;
            private bool __fit=true;
            public bool fit
            {
                get
                {
                    for (int i = 0; i < childs.Count; i++)
                    {
                        if (childs[i].fit)
                        {
                            return true;
                        }
                    }
                    return __fit;
                }
            }





            public virtual int depth { get { return parent.depth + 1; } }
            public MenuTrunk parent { get; private set; }
            public MenuTree tree { get; private set; }

            protected List<MenuTrunk> childs { get { return tree.GetChild(this); } }
            public string path
            {
                get
                {
                    if (parent == null)
                        return name;
                    return parent.path + "/" + name;
                }
            }
            public float totalHeight
            {
                get
                {
                    float tmp = height;
                    if (isOn)
                    {
                        childs.ForEach((node) => {
                            tmp += node.totalHeight;
                        });
                    }
                    return tmp;

                }
            }
            public MenuTrunk(string name,MenuTrunk parent, MenuTree tree)
            {
                this.name = name;
                this.parent = parent;
                this.tree = tree;
                isOn = true;
            }

            public virtual void OnGUI(Rect rect)
            {
                if (!fit) return;
                if (childs == null || childs.Count == 0 || !isOn)
                {
                    SelfGUI(rect);
                }
                else
                {
                    var rs = rect.HorizontalSplit(height);
                    SelfGUI(rs[0]);

                    DrawChild(rs[1], 0);
                }
            }
            private void SelfGUI(Rect rect)
            {
                if (Event.current.type== EventType.Repaint)
                {
                    GUIStyles.Get("ObjectPickerResultsOdd").Draw(rect, false, false, tree.current == this, false);
                    GUIStyles.Get("IN Title").Draw(rect, false, false, false, false);
                }

                Event e = Event.current;
                var _r = rect.Zoom(AnchorType.MiddleRight, new Vector2(-50, 0));

                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (_r.Contains(e.mousePosition) && e.clickCount == 1)
                        {
                            trySelect = true;
                        }
                        break;
                    case EventType.MouseUp:
                        if (_r.Contains(e.mousePosition) && e.clickCount == 1 && trySelect)
                        {
                            tree.current = this;
                            e.Use();
                        }
                        trySelect = false;
                        break;
                }

                var r = rect.Zoom(AnchorType.MiddleRight, new Vector2(-depth * 10, -15));
                if (childs == null || childs.Count == 0)
                    GUI.Label(r, name);
                else
                {
                    GUI.Box(r, "", "IN MinMaxStateDropDown");
                    var rs = r.VerticalSplit(15);

                    isOn = EditorGUI.Foldout(rs[0],isOn, "");
                    GUI.Label(rs[1],name);
                }
            }
            bool trySelect;

            protected void DrawChild(Rect rect, int index)
            {
                if (index >= childs.Count) return;
                var rs = rect.HorizontalSplit(childs[index].totalHeight);
                childs[index].OnGUI(rs[0]);
                DrawChild(rs[1], ++index);
            }


            public void Fitter(string fit)
            {
                if (string.IsNullOrEmpty(fit))
                {
                    __fit = true;
                }
                else
                {
                    if (name.ToLower().Contains(fit.ToLower()))
                    {
                        __fit = true;
                    }
                    else
                    {
                        __fit = false;
                    }
                }
                for (int i = 0; i < childs.Count; i++)
                {
                    childs[i].Fitter(fit);
                }

            }
        }
        public class MenuRoot : MenuTrunk
        {
            public override int depth { get { return 0; } }
            public MenuRoot(string name, MenuTrunk parent, MenuTree tree):base(name,parent,tree)
            {   }
            public override void OnGUI(Rect rect)
            {
                if (childs != null && childs.Count > 0)
                {
                    DrawChild(rect, 0);
                }
            }
        }
        private MenuRoot _root;
        private List<MenuTrunk> _nodes;
        private MenuTrunk _current;
        private Vector2 _scroll;
        public MenuTrunk current
        {
            get { return _current; }
            set
            {
                if (_current != value)
                {
                    _current = value;
                    if (_current != null && onCurrentChange != null)
                    {
                        onCurrentChange(_current.path.Substring(_root.path.Length + 1));
                    }
                }
            }
        }

        public event Action<string> onCurrentChange;
        public float height { get { return _root.totalHeight; } }
        public MenuTree()
        {
            _root = new MenuRoot("root",null,this);
            _nodes = new List<MenuTrunk>();
            _nodes.Add(_root);
        }


        private List<MenuTrunk> GetChild(MenuTrunk trunk)
        {
            return _nodes.FindAll((_node) => { return _node.parent == trunk; });
        }
        private MenuTrunk CreateTrunk(string content, MenuTrunk parent)
        {
            MenuTrunk leaf = new MenuTrunk(content,parent,this);
            _nodes.Add(leaf);
            return leaf;
        }
        public void Select(string path)
        {
            current = Find(path);
        }
        public void Clear()
        {
            for (int i = _nodes.Count-1; i >=0 ; i--)
            {
                if (_nodes[i]!=_root)
                {
                    _nodes.Remove(_nodes[i]);
                }
            }
        }
        public void ReadTree(List<string> paths,bool sort=true)
        {
          //  _nodes.Clear();
            if (sort) paths.Sort();
            for (int i = 0; i < paths.Count; i++)
            {
                string path = _root.path + "/" + paths[i];
                if (ContainsNode(path)) continue;
                var items = path.Split('/');
                for (int j = 1; j < items.Length; j++)
                {
                    string tmpPath = ToString(items, j);
                    CreateNode(tmpPath, items[j]);
                }
            }

        }

        public MenuTrunk Find(string path)
        {
            return _nodes.Find((_node) => { return path == _node.path; });
        }
        private string lastfit;

        public void Fitter(string fit)
        {
            if (fit!=lastfit)
            {
                _root.Fitter(fit);
                lastfit = fit;
            }
        }

        private string ToString(string[] strs, int count)
        {
            string tmp = "";
            for (int i = 0; i < count; i++)
            {
                tmp += "/" + strs[i];
            }
            return tmp.Substring(1);
        }
        private void CreateNode(string path, string content)
        {
            if (ContainsNode(path + "/" + content)) return;
            var trunk = Find(path);
            var t = CreateTrunk(content, trunk);


        }
        private bool ContainsNode(string path)
        {
            return Find(path) != null;
        }

        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            Event e = Event.current;
            var rs = position.HorizontalSplit(_root.totalHeight);
            _scroll = GUI.BeginScrollView(position, _scroll, rs[0]);
            _root.OnGUI(rs[0]);
            EmptyEve(e, rs[1]);
            GUI.EndScrollView();
        }
        private void EmptyEve(Event e, Rect r)
        {
          //  r.DrawOutLine(2, Color.red);
            if (r.height > 0 && r.Contains(e.mousePosition) && e.type == EventType.MouseUp)
            {
                current = null;
                e.Use();
            }
        }

    
    }

}
