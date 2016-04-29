/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using minion.taskmaster;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;

namespace minion.powers
{
    enum StatusUpdate
    {
        [Description("i have considered taking up rabbit herding")]
        a,

        [Description("i have looked at my axe")]
        b,

        [Description("i have looked at my sharpening stone")]
        c,

        [Description("i have sharpened my axe")]
        d,

        [Description("i have resolved to move to fiji and become a publican")]
        e,

        [Description("i have rememebered the joy of axe swinging")]
        f,

        [Description("i have donned my cloak")]
        g,

        [Description("i have presented myself at my post")]
        h,

        [Description("i have hooded the unfortunate evildoer")]
        i,

        [Description("i have completed my duties")]
        j
    };

    /// <summary>
    /// despite her grizzly name, the executioner has never hurt a fly.
    /// she is a peaceful instigator of cpu cycles and only performs
    /// executions in the context of process spawning.
    /// </summary>
    internal class Executioner
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Random randomNumberGenerator = new Random();

        public Executioner(Paeon paeon, IEnumerable<EnvironmentVariable> environmentVariables, Command command)
        {
            StatusUpdates = new ObservableCollection<string>();

            // todo: replace with actual work
            var shouldFail = randomNumberGenerator.Next(0, 100) < 25;
            for (int s = 0; s < (shouldFail ? randomNumberGenerator.Next(0, 9) : 10); s ++)
            {
                StatusUpdates.Add(Enumerations.GetDescription((StatusUpdate)s));
                logger.Trace(Enumerations.GetDescription((StatusUpdate)s).Replace(" my", " her").Replace("i have", "executioner has"));
                Thread.Sleep(randomNumberGenerator.Next(500, 4000));
            }
            if(StatusUpdates.Count == 10)
            TheMessAfterTheDeed = environmentVariables;
            IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask = (StatusUpdates.Count == 10);
            HasDoneTheDeed = true;
        }

        public ObservableCollection<string> StatusUpdates { get; private set; }
        public bool HasDoneTheDeed { get; private set; }
        public bool IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask { get; private set; }
        public IEnumerable<EnvironmentVariable> TheMessAfterTheDeed { get; private set; }
        
    }

    class Enumerations
    {
        public static string GetDescription(Enum value)
        {
            var attributes = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes != null && attributes.Length > 0)
                ? attributes[0].Description
                : value.ToString();
        }
    }
}
