/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.247
 *UnityVersion:   2018.4.24f1
 *Date:           2021-05-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using IFramework.GUITool;
using IFramework;
namespace IFramework_Demo
{
    [EditorWindowCache]
	public class MenuTreeExp:EditorWindow
	{
        private MenuTree tree;
        private void OnEnable()
        {
            tree = new MenuTree();
            tree.ReadTree(new System.Collections.Generic.List<string>()
            {
                "77",
                "77/88",
                 "77/88/99",

                "99",
                "88"
            });
        }
        private void OnGUI()
        {
            tree.OnGUI(this.LocalPosition());
        }
    }
}
