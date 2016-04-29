/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
                    logger.Info("payload aquired: {0}", payload.Id);
                    var environmentVariables = payload.Environment;

                    var commandFailureEncountered = false;

                    foreach(var command in payload.Commands)
                    {
                        if (_interrupt)
                        {
                            logger.Debug("command skipped due to interrupt or service stopping.");
                            logger.Trace("minion decrees an amnesty for the executioner and the condemned because he is also going to bed. no one knows the scouts whereabouts but minion will find and kill paeon, eventually.");
                            break;
                        }
                        else
                        {
                            if (commandFailureEncountered)
                            {
                                // skip running the command
                                logger.Debug("command skipped due to earlier failures: {0}", command);
                                logger.Trace("paeon {0} fate, {1}.", (Guid.NewGuid().ToByteArray().First() % 2 == 0) ? "accepts" : "considers", (ImaginaryFriend.MagicNumberThinkerUpper.Next(0, 9) % 2 == 0) ? "frightfully" : "dejectedly");
                            }
                            else
                            {
                                // run the command
                                logger.Debug("command execution underway: {0}", command);
                                var executioner = new Executioner(paeon, environmentVariables, command);
                                var seenStatusUpdates = new List<string>();
                                while (!executioner.HasDoneTheDeed)
                                {
                                    var availableStatusUpdatesCount = executioner.StatusUpdates.Count;
                                    var seenStatusUpdatesCount = seenStatusUpdates.Count;
                                    for (int i = seenStatusUpdatesCount; i < availableStatusUpdatesCount; i++)
                                    {
                                        // todo: broadcast via livelog
                                        seenStatusUpdates.Add(executioner.StatusUpdates[i]);
                                        logger.Trace("paeon {0}.", (ImaginaryFriend.MagicNumberThinkerUpper.Next(0, 9) % 2 == 0) ? "shrugs" : "twiddles thumbs");
                                    }
                                }
                                environmentVariables = executioner.TheMessAfterTheDeed;
                                if (executioner.IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask)
                                {
                                    logger.Trace("paeon {0}.", (ImaginaryFriend.MagicNumberThinkerUpper.Next(0, 9) % 2 == 0) ? "giggles" : "squirms delightedly");
                                }
                                else
                                {
                                    commandFailureEncountered = true;
                                }
                            }
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

        private void HandleExecutionerPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StatusUpdates":
                    logger.Trace("paeon {0}.", (ImaginaryFriend.MagicNumberThinkerUpper.Next(0, 9) % 2 == 0 ) ? "shrugs" : "twiddles thumbs");
                    break;
                case "IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask":
                    logger.Trace("paeon {0}.", (ImaginaryFriend.MagicNumberThinkerUpper.Next(0, 9) % 2 == 0 ) ? "is vaguely impressed" : "is relieved");
                    break;
                case "HasDoneTheDeed":
                    logger.Trace("paeon {0}.", (ImaginaryFriend.MagicNumberThinkerUpper.Next(0, 9) % 2 == 0 ) ? "nods" : "dozes");
                    break;
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
