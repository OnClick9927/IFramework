/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.109
 *UnityVersion:   2018.4.24f1
 *Date:           2020-12-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace IFramework.GUITool
{
    public class SearchablePopup : PopupWindowContent
    {
        public static void Show(Rect position, string[] options, int current, Action<int, string> onVakueChange, int width = 400)
        {
            SearchablePopup win = new SearchablePopup(options, current, onVakueChange,width);
            PopupWindow.Show(position, win);
        }
        private readonly string[] names;
        private readonly int width;
        private SearchField searchField;
        private SelectTree _tree;
        private SearchablePopup(string[] names, int currentIndex, Action<int, string> onSelectionMade,int width=400)
        {
            this.names = names;
            this.width = width;
            searchField = new SearchField("", null, 0);
            _tree = new SelectTree(new TreeViewState(),this, currentIndex, names, onSelectionMade);
            searchField.onValueChange += (str) =>
            {
                _tree.searchString = str;
            };

        }
        public override Vector2 GetWindowSize()
        {
            return new Vector2(width, Mathf.Min(600, (names.Length+1) * EditorStyles.toolbar.fixedHeight+10));
        }
        public override void OnGUI(Rect rect)
        {
            var rs = rect.HorizontalSplit(EditorStyles.toolbar.fixedHeight+5);
            DrawSearch(rs[0].Zoom(AnchorType.LowerCenter,-5));
            _tree.OnGUI(rs[1].Zoom(AnchorType.UpperCenter, -5));
        }

        private void DrawSearch(Rect rect)
        {
            searchField.OnGUI(rect.Zoom(AnchorType.MiddleCenter, -2));
        }

        private class SelectTree : TreeView
        {
            private static GUIStyle Selection = "SelectionRect";

            private readonly SearchablePopup _pop;
            private readonly int _current;
            private string[] _names;
            private readonly Action<int, string> onSelectionMade;
            private struct Index
            {
                public int id;
                public string value;
            }
            private List<Index> _show;
            public SelectTree(TreeViewState state, SearchablePopup pop,int current,string[] names, Action<int, string> onSelectionMade) : base(state)
            {
                this._pop = pop;
                this._current = current;
                this._names = names;
                this._show = new List<Index>();
                for (int i = 0; i < names.Length; i++)
                {
                    _show.Add(new Index()
                    {
                        id = names.ToList().IndexOf(names[i]),
                        value = names[i]
                    });
                }
                this.onSelectionMade = onSelectionMade;
                showAlternatingRowBackgrounds = true;
                Reload();
            }

            protected override TreeViewItem BuildRoot()
            {
                var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
      
                return root;
            }
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                var list = new List<TreeViewItem>();
                for (int i = 0; i < _show.Count; i++)
                {
                    list.Add(new TreeViewItem() { id = _show[i].id, depth = 1, displayName = _show[i].value });
                }
                return list;
            }
            protected override void SingleClickedItem(int id)
            {
                base.SingleClickedItem(id);
                onSelectionMade(id, _names[id]);
                _pop.editorWindow.Close();
                GUIUtility.ExitGUI();
            }
            private void DrawBox(Rect rect, Color tint)
            {
                Color c = GUI.color;
                GUI.color = tint;
                GUI.Box(rect, "", Selection);
                GUI.color = c;
            }
            protected override void SearchChanged(string newSearch)
            {
                _show.Clear();
                for (int i = 0; i < _names.Length; i++)
                {
                    if (_names[i].ToLower().Contains(searchString.ToLower()))
                    {
                        _show.Add(new Index()
                        {
                            id = _names.ToList().IndexOf(_names[i]),
                            value = _names[i]
                        });
                    }
                }
                Reload();
            }
           
            protected override void RowGUI(RowGUIArgs args)
            {
                base.RowGUI(args);
                if (args.item.id==_current)
                    DrawBox(args.rowRect, Color.white);
            }
            protected override bool CanMultiSelect(TreeViewItem item)
            {
                return false;
            }
            protected override bool CanBeParent(TreeViewItem item)
            {
                return false;
            }
        }
    }
}
