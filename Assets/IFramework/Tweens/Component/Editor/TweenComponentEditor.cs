/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.151
 *UnityVersion:   2018.4.24f1
 *Date:           2021-07-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using UnityEditor;
using UnityEngine;
namespace IFramework.Tweens
{
    public class TweenComponentEditor<T, Target> : Editor where T : struct where Target : Object
    {
        public virtual bool drawTargets { get { return false; } }
        public TweenComponent<T, Target> componet { get { return this.target.As<TweenComponent<T, Target>>(); } }
        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            using (new EditorGUI.DisabledGroupScope(Application.isPlaying))
            {
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                {
                    componet.autoPlay = GUILayout.Toggle(componet.autoPlay, "autoPlay", GUIStyles.toolbarButton);
                    componet.autoRcyle = GUILayout.Toggle(componet.autoRcyle, "autoRcyle", GUIStyles.toolbarButton);
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("duration"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("snap"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("curve"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("LoopType"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("loop"));
                if (drawTargets)
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("targets"), true);
                }
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("type"));
                switch (componet.type)
                {
                    case global::IFramework.Tweens.TweenComponent<T, Target>.TweenType.Single:
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("start"));
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("end"));

                        break;
                    case global::IFramework.Tweens.TweenComponent<T, Target>.TweenType.Array:
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("array"), true);

                        break;
                    default:
                        break;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                }
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Play"))
                    {
                        componet.Play();
                    }
                    if (GUILayout.Button("Rewind"))
                    {
                        componet.tween.Rewind(1);
                    }
                    if (GUILayout.Button("Complete"))
                    {
                        componet.tween.Complete(false);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
