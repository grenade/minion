/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using NLog;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace minion.powers
{
    internal class Paeon
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Paeon()
        {
            Name = InventUnsulliedName();
            Password = InventSecurePassword();
            HomeDrive = ChooseDrive();
            HomePath = Path.Combine(string.Concat(HomeDrive, Path.DirectorySeparatorChar), Name);
            UserPrincipal = CreateUserAccount();
            JoinGroups(new[] { "Users" });
            CreateHomePaths();
        }

        public string Name { get; private set; }

        public string Password { get; private set; }

        public UserPrincipal UserPrincipal { get; private set; }

        public string HomeDrive { get; private set; }

        public string HomePath { get; private set; }

        private bool female = (DateTime.Now.Ticks % 2 == 0);

        #region private helper methods

        private string InventUnsulliedName()
        {
            // todo: ensure no paeons before us have sullied (soiled?) this good name
            // (check for home directories or registry entries).
            var foundUnsulliedName = false;
            string name = null;
            while (!foundUnsulliedName)
            {
                name = string.Concat("paeon-", Guid.NewGuid().ToString().Substring(0, 8));
                foundUnsulliedName = !Enumerable.Range('x', 3).Concat(Enumerable.Range('c', 1)).AsParallel().Any(d=>Directory.Exists(string.Concat(d, @":\")));
            }
            return name;
        }

        private string InventSecurePassword()
        {
            // todo: fix this
            return Guid.NewGuid().ToString().Substring(0, 13);
        }

        private string ChooseDrive()
        {
            // in the absence of science, what happens here is:
            // - if the x: drive exists, use that. it's fast (x: is a striped raid 0 drive by convention).
            // - if y: and z: exist, use either, randomly (they're fast-ish ssd's by convention).
            // - fall back to the system drive (probably c:).
            return Directory.Exists(@"x:\")
                ? "x:"
                : Directory.Exists(@"y:\") && Directory.Exists(@"z:\")
                    ? (female ? "y:" : "z:")
                    : Environment.GetEnvironmentVariable("SystemDrive").ToLower();
        }

        private UserPrincipal CreateUserAccount()
        {
            UserPrincipal up;
            using (var mc = new PrincipalContext(ContextType.Machine))
            {
                up = new UserPrincipal(mc, Name, Password, true)
                {
                    PasswordNeverExpires = true,
                    HomeDirectory = HomePath
                };
                up.Save();
            }
            logger.Debug("minion spawned a paeon and named {0}: {1}.", (female ? "her" : "him"), Name);
            Console.WriteLine();
            return up;
        }

        private void JoinGroups(IEnumerable<string> groups)
        {
            using (var mc = new PrincipalContext(ContextType.Machine))
            {
                groups.AsParallel()
                    .Select(group => GroupPrincipal.FindByIdentity(mc, group))
                    .Where(group => group != null)
                    .ForAll(group =>
                    {
                        group.Members.Add(UserPrincipal);
                        group.Save();
                    });
            }
            logger.Debug("minion commanded: {0} to treat {1} as their own.", string.Join(", ", groups), Name);
        }

        private void CreateHomePaths()
        {
            var paths = new[] {
                HomePath,
                Path.Combine(HomePath, "Temp"),
                Path.Combine(HomePath, "AppData", "Roaming"),
                Path.Combine(HomePath, "AppData", "Local")
            };
            var sids = new[] {
                GroupPrincipal.FindByIdentity(new PrincipalContext(ContextType.Machine), "Administrators").Sid,
                UserPrincipal.Sid
            };
            var ds = new DirectorySecurity();
            sids.AsParallel().ForAll(sid => ds.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, AccessControlType.Allow)));
            paths.AsParallel().ForAll(path => Directory.CreateDirectory(path, ds));
            logger.Debug("minion created paths: {0} and granted: Administrators and {1} dominion over them.", string.Join(", ", paths), Name);
        }

        public void Kill()
        {
            UserPrincipal.Delete();
            logger.Debug("minion {0} {1} and felt {2} remorse.", (Guid.NewGuid().ToByteArray().First() % 2 == 0) ? "slew" : "killed", Name, (Guid.NewGuid().ToByteArray().First() % 2 == 0) ? "no" : "some");
        }

        #endregion private helper methods
    }
}
