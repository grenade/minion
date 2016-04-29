/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using NLog;
using System;
using System.ServiceProcess;

using minion.powers;
using System.Threading.Tasks;
using System.Threading;

namespace minion.daemon
{
    static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                logger.Trace("minion is attentive.");
                logger.Trace("minion's life will be cut short by a key press.");
                Attend.Instance.Init();
                Task.Factory.StartNew(() => Attend.Instance.Run());
                while (!Console.KeyAvailable)
                    Thread.Sleep(500);
                Console.ReadKey();
                Console.WriteLine();
                Task.Factory.StartNew(() => Attend.Instance.Stop());
                logger.Trace("minion is mortally wounded. another key press would be merciful.");
                Console.ReadKey();
            }
            else
            {
                try
                {
                    ServiceBase.Run(new[] { new Dispatcher() });
                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }
            }
        }
    }
}
