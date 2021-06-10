/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    Description
 *History:        2020-02-28--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using IFramework;
using IFramework.Modules.Message;
using IFramework.UI;

namespace IFramework_Demo
{
    public class Panel01ViewModel : UIViewModel<Panel01Model>
    {
        private Int32 _count;
        public Int32 count
        {
            get { return GetProperty(ref _count); }
            private set
            {
                Tmodel.count = value;
                SetProperty(ref _count, value);
            }
        }
        protected override void SubscribeMessage()
        {
            base.SubscribeMessage();
            this.message.Subscribe<Panel01View>(Listen);

            Launcher.env.modules.Message.Subscribe<UIExample>(Listen);

        }
        protected override void UnSubscribeMessage()
        {
            base.UnSubscribeMessage();
            this.message.UnSubscribe<Panel01View>(Listen);
            Launcher.env.modules.Message. UnSubscribe<UIExample>(Listen);

        }
        private void Listen(IMessage message)
        {
            if (message.args.Is<MathEvent>())
            {
                var eve = message. args.As<MathEvent>();
                switch (eve.type)
                {
                    case MathType.Sub:
                        count--;
                        break;
                    case MathType.Add:
                        count++;
                        break;
                    default:
                        break;
                }
            }

        }

        protected override void SyncModelValue()
        {
            this.count = Tmodel.count;

        }

    }
}
