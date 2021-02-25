/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1.440
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Modules.MVVM;

namespace IFramework.UI
{
    /// <summary>
    /// VM 基类
    /// </summary>
    public abstract class UIViewModel : ViewModel { }
    public abstract class UIViewModel<M> : UIViewModel where M : IDataModel
    {
        protected M Tmodel { get { return (M)model; } }
    }

}
