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
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Linq;

namespace IFramework.GUITool
{
    public class MenuTree : GUIBase
    {
        private class InnerTree : TreeView
        {
            public event Action<string> onCurrentChange;

            private IList<string> _paths;
            private class Item: TreeViewItem
            {
                public string path;
            }
            private List<Item> _items;

            public void ReadTree(List<string> paths, bool sort = true)
            {
                if (sort) paths.Sort();
                _paths = paths;
                this.Reload();
            }
            public InnerTree(TreeViewState state) : base(state)
            {
                this.rowHeight = 25;
                showAlternatingRowBackgrounds = true;
            }

            protected override TreeViewItem BuildRoot()
            {
                var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
                return root;
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
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                if (_paths == null) return new List<TreeViewItem>();
                _items = new List<Item>();
                for (int i = 0; i < _paths.Count; i++)
                {
                    string path = _paths[i];
                    var items = path.Split('/');
                    string last = items.Last();
                    bool ok = true;
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        if (!last.ToLower().Contains(this.searchString.ToLower())) ok = false;
                    }
                    if (ok)
                    {
                        var list2 = new List<Item>();
                        for (int j = 0; j < items.Length; j++)
                        {
                            int index = 0;
                            int depth = j;
                            string name = items[j];
                            string __path = ToString(items, j+1);
                            var exist = _items.Find((it) =>
                            {
                                return it.displayName == name && it.depth == depth;
                            });
                            if (exist==null)
                            {
                                index++;
                                exist = new Item() { id = index + _items.Count-1, depth = depth, displayName = name  ,path=__path};
                            }
                            list2.Add(exist);
                        }
                        for (int j = 0; j < list2.Count; j++)
                        {
                            Item item = list2[j];
                            Item next = null;
                            if (j+1<list2.Count)
                            {
                                next = list2[j + 1];
                            }
                            if (!_items.Contains(item))
                            {
                                _items.Add(item);
                            }
                            if (!IsExpanded(item.id) || next ==null)
                            {
                                item.children = next == null? null: CreateChildListForCollapsedParent();
                                break;
                            }
                            else
                            {
                                item.AddChild(next);
                            }
                        }
                    }
                }
                return _items.ConvertAll(it=> {
                    return new TreeViewItem()
                    {
                        id = it.id,
                        depth = it.depth,
                        displayName = it.displayName,
                        children = it.children,
                        icon = it.icon,
                        parent = it.parent
                    };
                });
            }

            protected override bool CanMultiSelect(TreeViewItem item)
            {
                return false;
            }
            protected override void SelectionChanged(IList<int> selectedIds)
            {
                string name = _items[selectedIds.First()].path;
                onCurrentChange?.Invoke(name);
                base.SelectionChanged(selectedIds);
            }
            protected override void SearchChanged(string newSearch)
            {
                Reload();
            }

            public void Clear()
            {
                _paths = null;
               // _items.Clear();
                Reload();
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                base.RowGUI(args);
            }
        }
        private InnerTree _tree;
        public event Action<string> onCurrentChange;

        public MenuTree()
        {
            _tree = new InnerTree(new TreeViewState());
            _tree.onCurrentChange += _tree_onCurrentChange;
        }
        public void Fitter(string fit)
        {
            _tree.searchString = fit;
        }
        private void _tree_onCurrentChange(string obj)
        {
            onCurrentChange?.Invoke(obj);
        }

        public void ReadTree(List<string> paths, bool sort = true)
        {
            _tree.ReadTree(paths, sort);
        }

        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            _tree.OnGUI(position);
        }

        public void Clear()
        {
            _tree.Clear();
        }

        protected override void OnDispose()
        {
            Clear();
        }
    }
}
