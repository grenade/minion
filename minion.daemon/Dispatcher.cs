/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using minion.powers;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace minion.daemon
{
    public partial class Dispatcher : ServiceBase
    {
        public Dispatcher()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Attend.Instance.Init();
            Task.Factory.StartNew(() => Attend.Instance.Run());
        }

        protected override void OnStop()
        {
            Task.Factory.StartNew(() => Attend.Instance.Stop());
        }
    }
}
