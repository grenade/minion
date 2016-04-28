﻿/*
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
                Thread.Sleep(5000);
            }
            Paeon.KillAll();
            logger.Debug("minion is at peace.");
        }

        private void Work()
        {
            logger.Debug("minion finds fullfilment in service.");
            _working = true;
            try
            {
                /*
                - scout for payload quests
                - claim quest for the glory of england, assure taskmaster of capable hands
                - done: spawn a paeon to do all the work or take the blame for failure
                - watch mercillesly while paeon executes payload tasks, criticise often, tally working time
                - report completion, take credit, bask in smug satisfaction
                */
                var paeon = new Paeon();
                // todo: execute tasks
                paeon.Kill();
            }
            catch (Exception ex)
            {
                /*
                - report failure to taskmaster, blame paeon
                */
                logger.Error(ex, "{0} failed miserably and is looking forward to summary execution.");
                Paeon.KillAll();
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
