/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using minion.taskmaster;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace minion.powers
{
    /// <summary>
    /// despite her grizzly name, the executioner has never hurt a fly.
    /// she is a peaceful instigator of cpu cycles and only performs
    /// executions in the context of process spawning.
    /// </summary>
    internal class Executioner
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Executioner(Paeon paeon, IEnumerable<EnvironmentVariable> environmentVariables, Command command)
        {
            StatusUpdates = new List<string>();
            TheMessBeforeTheDeed = environmentVariables;
            ExecuteCommand();
        }

        public List<string> StatusUpdates { get; private set; }
        public bool HasDoneTheDeed { get; private set; }
        public bool IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask { get; private set; }
        public IEnumerable<EnvironmentVariable> TheMessBeforeTheDeed { get; private set; }
        public IEnumerable<EnvironmentVariable> TheMessAfterTheDeed { get; private set; }

        private void ExecuteCommand()
        {
            // todo: replace with actual work
            var shouldFail = ImaginaryFriend.MagicNumberThinkerUpper.Next(0, 100) < 25;
            var updates = new[] {
                string.Format("i have {0} taking up {1} {2}.", ImaginaryFriend.InconclusiveAction, ImaginaryFriend.FarmActivity, ImaginaryFriend.NotFarmAnimals),
                string.Format("i have {0} my {1} {2}.", ImaginaryFriend.WeaponPreparation, ImaginaryFriend.WeaponAdjective, ImaginaryFriend.Weapon),
                string.Format("i have {0} my {1} {2}.", ImaginaryFriend.WeaponPreparation, ImaginaryFriend.WeaponAdjective, ImaginaryFriend.Weapon),
                string.Format("i have {0} my {1} {2}.", ImaginaryFriend.WeaponPreparation, ImaginaryFriend.WeaponAdjective, ImaginaryFriend.Weapon),
                string.Format("i have {0} to move to {1} and become a {2}.", ImaginaryFriend.ConclusiveAction, ImaginaryFriend.Place, ImaginaryFriend.Occupation),
                string.Format("i have donned my {0}.", ImaginaryFriend.Wearable),
                string.Format("i have {0} myself at {1}.", ImaginaryFriend.Appearance, ImaginaryFriend.Position),
                string.Format("i have {0} the {1} {2}.", ImaginaryFriend.LastRite, ImaginaryFriend.CondemnedAdjective, ImaginaryFriend.Condemned),
                string.Format("i have completed my duty.", ImaginaryFriend.LastRite, ImaginaryFriend.CondemnedAdjective, ImaginaryFriend.Condemned)
            };

            for (int i = 0; i < (shouldFail ? ImaginaryFriend.MagicNumberThinkerUpper.Next(0, updates.Length-1) : updates.Length); i++)
            {
                StatusUpdates.Add(updates[i]);
                logger.Trace(updates[i].Replace(" my", " her").Replace("i have ", "executioner has "));
                Thread.Sleep(ImaginaryFriend.MagicNumberThinkerUpper.Next(500, 4000));
            }

            TheMessAfterTheDeed = TheMessBeforeTheDeed;

            IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask = (StatusUpdates.Count == updates.Length);
            if (!IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask)
            {
                logger.Error("command execution failed."); // todo: include execution failure
            }
            HasDoneTheDeed = true;
        }
    }
}
