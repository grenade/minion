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
            logger.Trace("minion is poised.");
        }

        public void Run()
        {
            logger.Trace("minion seeks purpose.");
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
                logger.Trace("minion dutifully completes unappreciated labours.");
                Thread.Sleep(5000);
            }
            Paeon.KillAll();
            logger.Trace("minion is at peace.");
        }

        /// <summary>
        /// - scout for payload quests
        /// - claim quest for the glory of england, assure taskmaster of capable hands
        /// - done: spawn a paeon to do all the work or take the blame for failure
        /// - watch mercillesly while paeon executes payload tasks, criticise often, tally working time
        /// - report completion, take credit, bask in smug satisfaction
        /// </summary>
        private void Work()
        {
            logger.Trace("minion finds fullfilment in service.");
            _working = true;

            if (Scout.HasFoundAnHonourableQuest())
            {
                logger.Trace("minion cracks the whip.");
                var quest = Scout.AnHonourableQuest();
                try
                {
                    var paeon = new Paeon();
                    var payload = Scout.AnHonourableQuest();
                    var environmentVariables = payload.Environment;

                    foreach(var command in payload.Commands)
                    {
                        var executioner = new Executioner(paeon, environmentVariables, command);
                        while (!executioner.HasDoneTheDeed)
                        {
                            logger.Trace("paeon {0} admires the executioners axe blade.", (DateTime.Now.Ticks % 2 == 0) ? "gleefully" : "somberly");
                            Thread.Sleep(2000);
                        }
                        environmentVariables = executioner.TheMessAfterTheDeed;
                        if (executioner.IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask)
                        {
                            logger.Trace("paeon {0}.", (DateTime.Now.Ticks % 2 == 0)? "giggles" : "squirms delightedly");
                        }
                        else
                        {
                            logger.Trace("paeon considers fate, {0}.", (DateTime.Now.Ticks % 2 == 0) ? "frightfully" : "dejectedly");
                        }
                    }
                    paeon.Kill();
                }
                catch (Exception ex)
                {
                    /*
                    - todo: report failure to taskmaster, blame paeon
                    */
                    logger.Error(ex, "{0} failed miserably and is looking forward to summary execution.");
                    Paeon.KillAll();
                }
                finally
                {
                    _working = false;
                }
            }
            else
            {
                logger.Trace("minion fondly remembers purposeful days of lore.");
                Thread.Sleep(1000);
            }
        }

        private void Cleanup()
        {
            try
            {
                logger.Trace("minion mops up.");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion
    }
}
