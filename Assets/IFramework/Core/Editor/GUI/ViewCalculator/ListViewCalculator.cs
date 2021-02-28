/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;

namespace IFramework.GUITool
{
    [Serializable]
    public class ListViewCalculator
    {
        [Serializable]
        public class ColumnSetting
        {
            public float width;
            public string name;
            public float offsetX = -2;
            public float offSetY = -2;
            public float titleoffsetX = -8;
            public AnchorType anchorType = AnchorType.MiddleCenter;
            public AnchorType titleAnchorType = AnchorType.LowerCenter;
        }
        [Serializable]
        public class Column
        {
            public string name;
            public int rowID;
            public int columnID;
            public Rect position;
            public Rect localPostion;
        }
        [Serializable]
        public class Row : IComparable
        {
            public Column this[int Key] { get { return columns[Key]; } }
            public Column this[string Key]
            {
                get
                {
                    for (int i = 0; i < columns.Count; i++)
                        if (columns[i].name == Key) return columns[i];
                    return default(Column);
                }
            }
            public float width;
            public float height;
            public bool selected;
            public int rowID;
            public Rect position;
            public Rect localPostion;
            public Row() { columns = new List<Column>(); }
            public int ColumnCount { get { return columns.Count; } }
            public readonly List<Column> columns;
            internal void Calc(ColumnSetting[] settings)
            {
                while (columns.Count > settings.Length) columns.RemoveAt(columns.Count - 1);
                while (columns.Count < settings.Length) columns.Add(new Column());
                width = 0;
                for (int i = 0; i < columns.Count; i++)
                {
                    Column item = columns[i];
                    item.rowID = rowID;
                    item.columnID = i;
                    item.name = settings[i].name;
                    item.position = new Rect(position.x + width,
                                            position.y,
                                            settings[i].width,
                                            position.height).
                                            Zoom(settings[i].anchorType, new Vector2(settings[i].offsetX, settings[i].offSetY));
                    width += settings[i].width;
                }
                position.width = width;
            }
            internal void CalacLocal(Rect parent)
            {
                localPostion = position;
                localPostion.x -= parent.x;
                localPostion.y -= parent.y;
                for (int i = 0; i < columns.Count; i++)
                {
                    columns[i].localPostion = columns[i].position;
                    columns[i].localPostion.x -= position.x;
                    columns[i].localPostion.y -= position.y;
                }
            }

            public int CompareTo(object obj)
            {
                Row row = obj as Row;
                return this.rowID.CompareTo(row.rowID);
            }
        }
        public ListViewCalculator() {
            selectedRows = new List<Row>();
            rows = new List<Row>();
        }
        public readonly List<Row> selectedRows;
        public readonly List<Row> rows;
        private Rect _content;
        public Rect content { get { return _content; } }
        public Rect view;
        public float rowHeight { get; set; }
        public int rowCount { get; private set; }
        public int firstVisibleRow { get; private set; }
        public int lastVisibleRow { get; private set; }

        public void Calc(Rect view, Vector2 contentOffset, Vector2 scroll, float rowHeight, int rowCount, ColumnSetting[] setting)
        {
            firstVisibleRow = int.MaxValue;
            lastVisibleRow = int.MinValue;
            this.rowHeight = rowHeight;
            this.rowCount = rowCount;

            this.view = view;
            if (this.view.height <= rowHeight) this.view.height = rowHeight;

            this._content.position = contentOffset;
            _content.height = rowHeight * rowCount;

            while (rows.Count > rowCount) rows.RemoveAt(rows.Count - 1);
            while (rows.Count < rowCount) rows.Add(new Row());

            for (int i = 0; i < rows.Count; i++)
            {
                if (content.yMin - scroll.y + (i + 1) * rowHeight > this.view.yMax) break;
                if (content.yMin - scroll.y + (i + 1) * rowHeight < this.view.yMin) continue;
                firstVisibleRow = firstVisibleRow > i ? i : firstVisibleRow;
                lastVisibleRow = lastVisibleRow > i ? lastVisibleRow : i;

                Row row = rows[i];
                row.height = rowHeight;
                row.rowID = i;
                row.position = new Rect(view.x + 2,
                                        view.y + i * rowHeight,
                                        view.width - 4,
                                        rowHeight).Zoom(AnchorType.MiddleCenter, new Vector2(0, -1));
                row.Calc(setting);
                if (this.view.width > row.width)
                {
                    float offset = this.view.width - 4 - row.width;
                    row.width += offset;
                    row.columns[row.ColumnCount - 1].position.width += offset;
                }
                row.position.width = row.width;
                _content.width = row.width - 10;
                row.CalacLocal(content);

             
       
            }
        }


        private void Sort()
        {
            selectedRows.Sort();
        }
        public void SelectRow(int index)
        {
            if (index < 0 || index >= rows.Count) return;
            selectedRows.Clear();

            for (int i = 0; i < rows.Count; i++)
            {
                if (i == index)
                {
                    rows[i].selected = true;
                    if (!selectedRows.Contains(rows[i]))
                    {
                        selectedRows.Add(rows[i]);
                    }
                }
                else
                    rows[i].selected = false;
            }
        }
        public void ControlSelectRow(int index)
        {
            if (index < 0 || index >= rows.Count) return;
            rows[index].selected = !rows[index].selected;

            if (rows[index].selected)
            {
                if (!selectedRows.Contains(rows[index]))
                {
                    selectedRows.Add(rows[index]);
                }
            }
            else
            {
                if (selectedRows.Contains(rows[index]))
                {
                    selectedRows.Remove(rows[index]);
                }
            }
            Sort();

        }
        public void ShiftSelectRow(int index)
        {
            if (index < 0 || index >= rows.Count) return;
            int temp = -1;
            for (int j = 0; j < rows.Count; j++)
                if (rows[j].selected)
                {
                    temp = j; break;
                }
            if (temp == -1 || temp == index) rows[index].selected = true;
            else if (temp < index)
                for (int j = temp; j <= index; j++)
                {
                    rows[j].selected = true;
                    if (!selectedRows.Contains(rows[j]))
                    {
                        selectedRows.Add(rows[j]);
                    }

                }
            else if (temp > index)
                for (int j = index; j < temp; j++)
                {
                    rows[j].selected = true;
                    if (!selectedRows.Contains(rows[j]))
                    {
                        selectedRows.Add(rows[j]);
                    }
                }

            Sort();


        }
        public void SelectNone()
        {
            for (int j = 0; j < rows.Count; j++)
                rows[j].selected = false;
            selectedRows.Clear();
        }
        public void SelectAll()
        {
            selectedRows.Clear();
            for (int j = 0; j < rows.Count; j++)
            {

                rows[j].selected = true;
                selectedRows.Add(rows[j]);
            }
        }


    }

}
