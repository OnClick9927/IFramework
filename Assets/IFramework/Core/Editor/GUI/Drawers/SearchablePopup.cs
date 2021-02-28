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
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool
{
    class SearchablePopup : PopupWindowContent
    {
        private class FilteredList
        {
            public struct Entry
            {
                public int index;
                public string value;
            }

            private string[] items;
            public string fitter { get; private set; }
            public List<Entry> entries { get; private set; }
            public int count
            {
                get
                {
                    return items.Length;
                }
            }

            public FilteredList(string[] items)
            {
                this.items = items;
                entries = new List<Entry>();
                UpdateFilter("");
            }

            public bool UpdateFilter(string filter)
            {
                if (fitter == filter)
                    return false;

                fitter = filter;
                entries.Clear();

                for (int i = 0; i < items.Length; i++)
                {
                    if (string.IsNullOrEmpty(fitter) || items[i].ToLower().Contains(fitter.ToLower()))
                    {
                        Entry entry = new Entry
                        {
                            index = i,
                            value = items[i]
                        };
                        if (string.Equals(items[i], fitter, StringComparison.CurrentCultureIgnoreCase))
                            entries.Insert(0, entry);
                        else
                            entries.Add(entry);
                    }
                }
                return true;
            }
        }

        private const float ROW_HEIGHT = 16.0f;
        private const float ROW_INDENT = 8.0f;

        public static void Show(Rect position, string[] options, int current, Action<int, string> onVakueChange)
        {
            SearchablePopup win = new SearchablePopup(options, current, onVakueChange);
            PopupWindow.Show(position, win);
        }

        private static void DrawBox(Rect rect, Color tint)
        {
            Color c = GUI.color;
            GUI.color = tint;
            GUI.Box(rect, "", Selection);
            GUI.color = c;
        }

        private readonly Action<int, string> onSelectionMade;
        private readonly int currentIndex;
        private readonly FilteredList list;
        private Vector2 scroll;
        private int hoverIndex;
        private int scrollToIndex;
        private float scrollOffset;
        private static GUIStyle Selection = "SelectionRect";
        private SearchField searchField;

        private SearchablePopup(string[] names, int currentIndex, Action<int, string> onSelectionMade)
        {
            list = new FilteredList(names);
            this.currentIndex = currentIndex;
            this.onSelectionMade = onSelectionMade;

            hoverIndex = currentIndex;
            scrollToIndex = currentIndex;
            scrollOffset = GetWindowSize().y - ROW_HEIGHT * 2;
            searchField = new SearchField(list.fitter, null, 0);
        }
        public override Vector2 GetWindowSize()
        {
            return new Vector2(base.GetWindowSize().x*2,Mathf.Min(600, list.count * ROW_HEIGHT +EditorStyles.toolbar.fixedHeight));
        }
        public override void OnGUI(Rect rect)
        {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }

        private void DrawSearch(Rect rect)
        {
            GUI.Label(rect, "", EditorStyles.toolbar);
            searchField.OnGUI(rect.Zoom(AnchorType.MiddleCenter, -2));
            if (list.UpdateFilter(searchField.value))
            {
                hoverIndex = 0;
                scroll = Vector2.zero;
            }
        }
        private void DrawSelectionArea(Rect scrollRect)
        {
            Rect contentRect = new Rect(0, 0,
                scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                list.entries.Count * ROW_HEIGHT);

            scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

            Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);

            Event e = Event.current;
            for (int i = 0; i < list.entries.Count; i++)
            {
                if (scrollToIndex == i &&
                    (e.type == EventType.Repaint || e.type == EventType.Layout))
                {
                    Rect r = new Rect(rowRect);
                    r.y += scrollOffset;
                    GUI.ScrollTo(r);
                    scrollToIndex = -1;
                    scroll.x = 0;
                }

                if (rowRect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseMove)
                    {
                        hoverIndex = i;
                        e.Use();
                    }
                    if (e.type == EventType.MouseDown)
                    {
                        onSelectionMade(list.entries[i].index, list.entries[i].value);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                DrawRow(rowRect, i);

                rowRect.y = rowRect.yMax;
            }

            GUI.EndScrollView();
        }
        private void DrawRow(Rect rowRect, int i)
        {
            if (list.entries[i].index == currentIndex)
                DrawBox(rowRect, Color.cyan);
            else if (i == hoverIndex)
                DrawBox(rowRect, Color.white);
            Rect labelRect = new Rect(rowRect);
            labelRect.xMin += ROW_INDENT;
            GUI.Label(labelRect, list.entries[i].value);
        }
        private void HandleKeyboard()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.DownArrow)
                {
                    hoverIndex = Mathf.Min(list.entries.Count - 1, hoverIndex + 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = ROW_HEIGHT;
                    e.Use();

                }
                if (e.keyCode == KeyCode.UpArrow)
                {
                    hoverIndex = Mathf.Max(0, hoverIndex - 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = -ROW_HEIGHT;
                    e.Use();

                }
                if (e.keyCode == KeyCode.Return || e.character == '\n')
                {
                    if (hoverIndex >= 0 && hoverIndex < list.entries.Count)
                    {
                        onSelectionMade(list.entries[hoverIndex].index, list.entries[hoverIndex].value);
                        EditorWindow.focusedWindow.Close();
                        e.Use();

                    }
                }
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    EditorWindow.focusedWindow.Close();
                    e.Use();

                }
            }
        }
    }
}
