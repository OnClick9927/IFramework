/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.GUITool
{
    [Serializable]
    public class TableViewCalculator
    {
        public TableViewCalculator() { _listView = new ListViewCalculator(); }
        private ListViewCalculator _listView;
        public Rect position { get; private set; }
        public Rect view { get { return _listView.view; } }
        public Rect content { get { return _listView.content; } }
        public ListViewCalculator.Row titleRow { get; private set; }
        public List<ListViewCalculator.Row> rows { get { return _listView.rows; } }
        public int firstVisibleRow { get { return _listView.firstVisibleRow; } }
        public int lastVisibleRow { get { return _listView.lastVisibleRow; } }
        public float titleRowOffsetY = 0.2f;
        public List<ListViewCalculator.Row> selectedRows { get { return _listView.selectedRows; } }


        public void Calc(Rect position, Vector2 contentOffset, Vector2 scroll, float rowHeight, int rowCount, ListViewCalculator.ColumnSetting[] setting)
        {
            this.position = position;
            float Mutl = titleRowOffsetY + 1;
            titleRow = new ListViewCalculator.Row()
            {
                height = rowHeight,
                rowID = 0,
                position = new Rect(position.x,
                                        position.y,
                                        position.width,
                                        rowHeight * Mutl)
            };
            ListViewCalculator.ColumnSetting[] Tsetting = new ListViewCalculator.ColumnSetting[setting.Length];
            for (int i = 0; i < Tsetting.Length; i++)
            {
                Tsetting[i] = new ListViewCalculator.ColumnSetting()
                {
                    width = setting[i].width,
                    name = setting[i].name,
                    offsetX = setting[i].titleoffsetX,
                    offSetY = -titleRowOffsetY * rowHeight,
                    anchorType = setting[i].titleAnchorType,
                    titleAnchorType = setting[i].titleAnchorType
                };
            }
            titleRow.Calc(Tsetting);
            titleRow.CalacLocal(new Rect(position.x,
                                        position.y,
                                        position.width,
                                        rowHeight * Mutl));

            if (view.width > titleRow.width)
            {
                float offset = view.width - titleRow.width;
                titleRow.width += offset;
                titleRow.localPostion.width += offset;
                titleRow.columns[titleRow.ColumnCount - 1].position.width += offset;
                titleRow.columns[titleRow.ColumnCount - 1].localPostion.width += offset;
            }

            for (int i = 0; i < titleRow.ColumnCount; i++)
            {
                ListViewCalculator.Column item = titleRow.columns[i];
                item.position.x -= scroll.x;
                item.localPostion.x -= scroll.x;
            }

            titleRow.position.x = view.x;
            titleRow.position.width = view.width;

            _listView.Calc(new Rect(position.x,
                                position.y + rowHeight * Mutl,
                                position.width,
                                position.height - rowHeight), contentOffset, scroll,
                                    rowHeight,
                                    rowCount, setting);

            _listView.view.height -= titleRowOffsetY * rowHeight;



        }




        public void ShiftSelectRow(int index)
        {
            _listView.ShiftSelectRow(index);
        }
        public void ControlSelectRow(int index)
        {
            _listView.ControlSelectRow(index);
        }
        public void SelectRow(int index)
        {
            _listView.SelectRow(index);
        }
        public void SelectNone()
        {
            _listView.SelectNone();
        }
        public void SelectAll()
        {
            _listView.SelectAll();
        }

        public void Calc(Rect rect, Vector2 vector2, object scroll, float lineHeight, int v, ListViewCalculator.ColumnSetting[] setting)
        {
            throw new NotImplementedException();
        }
    }
}
