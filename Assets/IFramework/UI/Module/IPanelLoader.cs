/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace IFramework.UI
{
    /// <summary>
    /// ui加载器
    /// </summary>
    public interface IPanelLoader
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        UIPanel Load(ref string name);
    }
}
