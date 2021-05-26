/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace IFramework.Language
{
    [CreateAssetMenu(fileName ="NewLanGoup",menuName = "IFramework/NewLanGoup")]
    public class LanGroup:ScriptableObject
	{
#if UNITY_EDITOR
        public const string assetPath = "Language/Editor/LanGroup.asset";
#endif
        public List<LanPair> pairs = new List<LanPair>();

        public void DeletePairsByLan(SystemLanguage lan)
        {
            pairs.RemoveAll((pair) => { return pair.lan == lan; });
        }

        public void DeletePairsByKey(string key)
        {
            pairs.RemoveAll((pair) => { return pair.key == key; });
        }

        public void DeletePair(LanPair pair)
        {
            pairs.Remove(pair);
        }
    }
}
