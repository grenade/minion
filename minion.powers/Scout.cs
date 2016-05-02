/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using minion.taskmaster;
using NLog;
using System.Collections.Generic;

namespace minion.powers
{
    /// <summary>
    /// The scout seeks out quests in the form of TaskCluster task payloads.
    /// Scouts are responsible for determining and returning to the minion a single payload for processing.
    /// Scouts manage all dialogue with the taskmaster on the minions behalf.
    /// </summary>
    internal static class Scout
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool HasFoundAnHonourableQuest()
        {
            // be 25% successful at finding quests.
            //var success = ImaginaryFriend.MagicNumberThinkerUpper.Next(0, 100) < 25;
            //logger.Trace("scout has {0}.", success ? "made triumphant noises" : "been found to be useless, yet again");
            //return success;

            // todo: implement
            //return GitHub.RecentCommitsExist();
            return true;
        }

        public static Payload AnHonourableQuest()
        {
            logger.Trace("scout has returned with an honourable quest.");
            return GitHub.GetPayload();
        }

        public static void LogResults(IEnumerable<CommandResult> results)
        {
            logger.Trace("scout is dispatched with a somewhat accurate tale of fear and loathing.");
            GitHub.Update("mozharness-firefox-windows", results);
        }
    }

}
