/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using IFramework.Serialization.DataTable;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
namespace IFramework.Language
{
    public static class LanguageModuleEx
    {
        public static void LoadCsv(this ILanguageModule module, string path, bool rewrite = true)
        {
            var dw = DataTableTool.CreateReader(new StreamReader(path, Encoding.UTF8), new DataRow(), new DataExplainer());
            var pairs = dw.Get<LanPair>();
            module.Load(pairs, rewrite);
        }
        public static void LoadXml(this ILanguageModule module, string xml, bool rewrite = true)
        {
            var pairs = Xml.FromXml<List<LanPair>>(xml);
            module.Load(pairs, rewrite);
        }
        public static void LoadJson(this ILanguageModule module, string json, bool rewrite = true)
        {
            var pairs = JsonUtility.FromJson<List<LanPair>>(json);
            module.Load(pairs, rewrite);
        }
        public static void LoadScriptableObject(this ILanguageModule module, LanGroup group, bool rewrite = true)
        {
            var pairs = group.pairs;
            module.Load(pairs, rewrite);
        }
    }
}
