/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using NLog;
using System;
using System.Threading;

namespace minion.powers
{
    public class Attend
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region singleton

        static readonly object DiscLock = new object();
        static Attend _instance;
        public static Attend Instance
        {
            get
            {
                lock (DiscLock)
                    return _instance ?? (_instance = new Attend());
            }
        }

        #endregion

        #region schedule checking and service init, run, work, stop

        private static bool _interrupt;
        private static bool _working;

        public void Init()
        {
            logger.Debug("minion is poised.");
        }

        public void Run()
        {
            logger.Debug("minion seeks purpose.");
            while (!_interrupt)
            {
                Work();
            }
            Cleanup();
        }

        public void Stop()
        {
            _interrupt = true;
            while (_working)
            {
                logger.Debug("minion dutifully completes unappreciated labours.");
            }
            logger.Debug("minion is at peace.");
        }

        private void Work()
        {
            _working = true;
            try
            {
                logger.Debug("minion finds fullfilment in service.");
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                _working = false;
            }
        }

        private void Cleanup()
        {
            try
            {
                logger.Debug("minion mops up.");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion
    }
}
