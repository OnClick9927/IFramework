/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.279
 *UnityVersion:   2018.4.24f1
 *Date:           2020-12-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using IFramework.GUITool.ToorbarMenu;
using IFramework.GUITool;
using System.Reflection;
using System;

namespace IFramework.Tweens
{
    partial class TweenWindow
    {

        public enum ShowType
        {
            Ease,
        }
        [Serializable]
        class EsseGUI: GUIBase
        {
            public Ease ease { get { return _ease; } set { _ease = value;
                    _converter.Config(value);
                } }
            EaseCoverter _converter = new EaseCoverter();
            public TweenWindow window;
            private Ease _ease;
            private float _percent;
            private ITween tween;
            private const float cellsize = 80;
            private const float delta = 0.2f;

            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                GUI.BeginGroup(position);
                Rect pos = new Rect(new Vector2(50, 10), new Vector2(400, 400));
                GUI.Box(pos, "");

                Handles.color = window._backgroundColor;
                Vector2 end = new Vector2(pos.width, pos.height) + pos.position;
                Handles.DrawLine(new Vector2(0, pos.height) + pos.position, end);
                end = new Vector2(0, pos.height) + pos.position;
                Handles.DrawLine(new Vector2(0, 0) + pos.position, end);
                float hCount = pos.width / cellsize;
                float vCount = pos.height / cellsize;
                hCount = Math.Min(hCount, vCount);

                Handles.DrawLine(pos.BottomLeft(), new Vector2(hCount * cellsize, pos.height - hCount * cellsize) + pos.position);


                int _index = 0;
                while (pos.height >= _index * cellsize)
                {
                    float y = pos.height - _index * cellsize;
                    Vector2 left = new Vector2(0, y) + pos.position;
                    Vector2 right = new Vector2(pos.width, y) + pos.position;
                    Handles.DrawLine(left, right);
                    GUI.Label(new Rect(left + new Vector2(-20, -6), Vector2.one * 80), (_index * delta).ToString("0.0"));
                    _index++;
                }
                _index = 0;
                while (pos.width >= _index * cellsize)
                {
                    float x = _index * cellsize;
                    Vector2 top = new Vector2(x, 0) + pos.position;
                    Vector2 buttom = new Vector2(x, pos.height) + pos.position;
                    Handles.DrawLine(top, buttom);
                    GUI.Label(new Rect(buttom + new Vector2(-8, 0), Vector2.one * 80), (_index * delta).ToString("0.0"));
                    _index++;
                }

                float per = 0;
                float _p = 0.002f;
                float pixels = cellsize * 1 / delta;
                while (per<1)
                {
                    var p1 = _converter.Convert(per / 1, per, 1);
                    per += _p;
                    var p2 = _converter.Convert(per / 1, per, 1);
                    Handles.color = Color.Lerp(window._curveHeadColor, window._curveTrailColor, per);
                    Handles.DrawLine(new Vector2((per-_p)*pixels,pos.height- p1 * pixels)+pos.position, new Vector2(per*pixels, pos.height - p2 * pixels)+pos.position);
                }
                var rect = new Rect(pos.BottomLeft() + new Vector2(0, 30), new Vector2(pos.width, 20));
                var rs = rect.VerticalSplit(pos.width / 4 * 3, 20);
                _percent = EditorGUI.Slider(rs[0], "Percent", _percent, 0, 1);
                if (GUI.Button(rs[1], "Watch Curve"))
                {
                    _percent = 0;
                    if (tween != null)
                    {
                        tween.Complete(false);
                        tween = null;
                    }
                    tween = TweenEx.DoGoto(0, 1, 5f, () => { return _percent; }, (value) =>
                    {
                        _percent = value;
                        window.Repaint();
                    }, false, EditorEnv.envType)
                            .SetEase(_ease)
                            .OnCompelete(() =>
                            {
                                _percent = 1;
                            })
                            .SetDeltaTime((float)EditorEnv.env.deltaTime.TotalMilliseconds);
                }
                {
                    var point = _converter.Convert(_percent,_percent,1);
                    Rect cone = new Rect(Vector2.zero, Vector2.one * 10);

                    cone.center = new Vector2(_percent * pixels, pos.height - point * pixels) + pos.position;
                    GUI.Box(cone, new GUIContent("", point.ToString()), GUIStyles.Get("U2D.createRect"));
                    var content = new GUIContent(point.ToString());
                    var size = GUIStyles.Get("label").CalcSize(content);
                    GUI.Label(new Rect(cone.position + Vector2.one * 10, size), content);
                }
                GUI.EndGroup();
            }

            protected override void OnDispose()
            {
               
            }
        }
    }
    [EditorWindowCache]
    partial class TweenWindow:EditorWindow
	{
        private ShowType _show;
        private Color _backgroundColor=new Color32(47,83,168,255);
        private Color _curveHeadColor=new Color32(86,215,162,255);
        private Color _curveTrailColor = new Color32(222, 100, 174, 255);


        private ToolBarTree tree = new ToolBarTree();
        [SerializeField] private EsseGUI _ease = new EsseGUI();

        private void OnEnable()
        {
            _ease.window = this;
            tree
                .Delegate((position) => {

                    _show = (ShowType)EditorGUI.EnumPopup(position, _show, GUIStyles.Get("ToolbarDropdown"));
                })
                .Delegate((position) => {

                    _ease.ease = (Ease)EditorGUI.EnumPopup(position, _ease.ease , GUIStyles.Get("ToolbarDropdown"));
                },100,()=> { return _show == ShowType.Ease; })

                .FlexibleSpace()

                .Label(new GUIContent("Background"), 80)
                .Delegate((position) =>
                {
                    _backgroundColor = EditorGUI.ColorField(position, "", _backgroundColor);
                },60)
                .Label(new GUIContent("CurveHead"), 70)
                .Delegate((position) =>
                {
                    _curveHeadColor = EditorGUI.ColorField(position, "", _curveHeadColor);
                }, 60)
                .Label(new GUIContent("CurveTrial"), 70)
                .Delegate((position) =>
                {
                    _curveTrailColor = EditorGUI.ColorField(position, "", _curveTrailColor);
                }, 60);
        }
        private void OnDisable()
        {
        }
        private void OnGUI()
        {
            var rs0= this.LocalPosition().HorizontalSplit(20);
            tree.OnGUI(rs0[0]);
            switch (_show)
            {

                case ShowType.Ease:
                    _ease.OnGUI(rs0[1]);

                    break;
                default:
                    break;
            }
        }
    }
}
