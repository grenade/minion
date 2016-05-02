/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using minion.taskmaster;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
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
            this.paeon = paeon;
            this.command = command;
            StatusUpdates = new List<string>();
            TheMessBeforeTheDeed = environmentVariables;
            ExecuteCommand();
        }

        public List<string> StatusUpdates { get; private set; }
        public bool HasDoneTheDeed { get; private set; }
        public bool IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask { get; private set; }
        public IEnumerable<EnvironmentVariable> TheMessBeforeTheDeed { get; private set; }
        public IEnumerable<EnvironmentVariable> TheMessAfterTheDeed { get; private set; }

        private Paeon paeon;
        private Command command;

        private void ExecuteCommand()
        {
            var environmentVariables = new List<EnvironmentVariable>(MergeProtectedEnvironmentVariables(TheMessBeforeTheDeed.Select(v => new EnvironmentVariable(v.Name, ExpandEnvironmentVariablesWithSubstitution(ExpandEnvironmentVariables(v.Value, TheMessBeforeTheDeed), paeon.HomePath, paeon.Name, paeon.Password)))));

            var impossibleSubstitutions = new Dictionary<string, string>();
            while (environmentVariables.Any(v => Regex.IsMatch(v.Value, "%[^%^;]+%") && !Regex.IsMatch(v.Value, string.Format("%{0}%", v.Name), RegexOptions.IgnoreCase) && !Regex.Matches(v.Value, "%([^%^;]+)%").Cast<Match>().Select(r => r.Groups[1].Value).Any(r => impossibleSubstitutions.ContainsKey(r.ToUpperInvariant()))))
            {
                foreach (var v in environmentVariables.Where(v => Regex.IsMatch(v.Value, "%[^%^;]+%") && !Regex.IsMatch(v.Value, string.Format("%{0}%", v.Name), RegexOptions.IgnoreCase)))
                {
                    foreach (Match m in Regex.Matches(v.Value, "%([^%^;]+)%"))
                    {
                        if (!environmentVariables.Select(x => x.Name).Any(name => string.Equals(name, m.Groups[1].Value)) && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(m.Groups[1].Value)))
                        {
                            if (!impossibleSubstitutions.ContainsKey(m.Groups[1].Value.ToUpperInvariant()))
                                impossibleSubstitutions.Add(m.Groups[1].Value.ToUpperInvariant(), m.Groups[1].Value);
                            v.Value = v.Value.Replace(string.Format("%{0}%", m.Groups[1].Value), string.Empty);
                        }
                    }
                }
                environmentVariables = new List<EnvironmentVariable>(environmentVariables.Select(v => new EnvironmentVariable(v.Name, ExpandEnvironmentVariablesWithSubstitution(ExpandEnvironmentVariables(v.Value, environmentVariables), paeon.HomePath, paeon.Name, paeon.Password))));
            }

            try
            {
                command.File = ExpandEnvironmentVariablesWithSubstitution(ExpandEnvironmentVariables(command.File, environmentVariables), paeon.HomePath, paeon.Name, paeon.Password);
                command.Arguments = command.Arguments.Select(a => ExpandEnvironmentVariablesWithSubstitution(ExpandEnvironmentVariables(a, environmentVariables), paeon.HomePath, paeon.Name, paeon.Password));

                var errors = new List<string>();
                var output = new List<string>();
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command.File,
                        Arguments = command.Arguments.Any() ? string.Join(" ", command.Arguments) : null,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = paeon.HomePath,
                        UserName = paeon.Name,
                        Password = paeon.Password.ToSecureString()
                    }
                };
                try
                {
                    // pass environment changes to the command
                    environmentVariables.ForEach(e => {
                        if (process.StartInfo.EnvironmentVariables.ContainsKey(e.Name))
                            process.StartInfo.EnvironmentVariables[e.Name] = e.Value;
                        else
                            process.StartInfo.EnvironmentVariables.Add(e.Name, e.Value);
                    });
                    // many moz processes use stderr to communicate output. there's no benefit in differentiating between stderr and stdout
                    process.ErrorDataReceived += (s, ea) => { if (!string.IsNullOrWhiteSpace(ea.Data)) StatusUpdates.Add(ea.Data); };
                    process.OutputDataReceived += (s, ea) => { if (!string.IsNullOrWhiteSpace(ea.Data)) StatusUpdates.Add(ea.Data); };
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    if (process.ExitCode != 0 && !process.HasExited)
                        process.Kill();

                    // propagate environment changes from the command
                    environmentVariables.Clear();
                    process.StartInfo.EnvironmentVariables.Cast<DictionaryEntry>().AsParallel().ForAll(v => environmentVariables.Add(new EnvironmentVariable((string)v.Key, (string)v.Value)));
                    TheMessAfterTheDeed = environmentVariables;

                    IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask = (process.ExitCode == 0 && process.HasExited);

                    // record some stats
                    results.Add(new CommandResult()
                    {
                        StartTime = process.StartTime,
                        ExitTime = process.ExitTime,
                        TotalProcessorTime = process.TotalProcessorTime,
                        ExitCode = process.ExitCode,
                        EnvironmentBefore = environmentBefore,
                        EnvironmentAfter = environmentVariables
                    });
                }
                catch (Win32Exception win32Exception)
                {
                    // the payload command itself failed. we have no control over that, its business as usual while we record the failure.
                    logger.Warn(win32Exception, "a payload command was a failure");
                    IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask = false;
                }
                finally
                {
                    process.CancelOutputRead();
                    process.CancelErrorRead();
                    if (!process.HasExited)
                        process.Kill();
                    HasDoneTheDeed = true;
                }
            }
            catch (Exception exception)
            {
                //we failed to execute the payload command. we care about this.
                logger.Error(exception, "an attempt to run a payload command was a failure");
                IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask = false;
            }
            finally
            {
                var completed = results.Count(r => r.ExitCode == 0);
                var skipped = results.Count(r => r.ExitCode == -100000);
                var failed = results.Count(r => r.ExitCode != -100000 && r.ExitCode != 0);

                DeleteUser(user);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("minion killed paeon {0} after she completed {1} tasks and failed at {2} in {3}.", username, completed, failed, new TimeSpan(results.Sum(r => r.TotalProcessorTime.Ticks)));
                Console.ResetColor();
            }
            
            
            if (!IsDutifullySatisfiedIfSlightlyMoroseConsideringHerBurdensomeTask)
            {
                logger.Error("command execution failed."); // todo: include execution failure
            }
            HasDoneTheDeed = true;
        }

        private IEnumerable<EnvironmentVariable> MergeProtectedEnvironmentVariables(IEnumerable<EnvironmentVariable> environmentVariables)
        {
            var protectedEnvironmentVariables = new List<EnvironmentVariable>()
            {
                new EnvironmentVariable("USERNAME", paeon.Name),
                new EnvironmentVariable("HOME", paeon.HomePath),
                new EnvironmentVariable("HOMEDRIVE", paeon.HomeDrive),
                new EnvironmentVariable("HOMEPATH", paeon.HomePath.Split(':').Last()),
                new EnvironmentVariable("USERPROFILE", paeon.HomePath),
                new EnvironmentVariable("TEMP", Path.Combine(paeon.HomePath, "Temp")),
                new EnvironmentVariable("TMP", Path.Combine(paeon.HomePath, "Temp")),
                new EnvironmentVariable("APPDATA", Path.Combine(paeon.HomePath, "AppData", "Roaming")),
                new EnvironmentVariable("LOCALAPPDATA", Path.Combine(paeon.HomePath, "AppData", "Local"))
            };
            var mergedEnvironmentVariables = new List<EnvironmentVariable>(protectedEnvironmentVariables);
            foreach (var ev in environmentVariables)
                if (!protectedEnvironmentVariables.Any(pev => Regex.IsMatch(ev.Name, pev.Name, RegexOptions.IgnoreCase)))
                    mergedEnvironmentVariables.Add(ev);
            return mergedEnvironmentVariables;
        }

        private static string ExpandEnvironmentVariables(string value, IEnumerable<EnvironmentVariable> environmentVariables)
        {
            foreach (var ev in environmentVariables)
                if (value != ev.Value)
                    value = Regex.Replace(value, string.Format("%{0}%", ev.Name), ev.Value, RegexOptions.IgnoreCase);
            return value;
        }

        private static string ExpandEnvironmentVariablesWithSubstitution(string value, string workingDirectory = null, string username = null, string password = null)
        {
            string result = string.Empty;
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = string.Concat("/c echo ", value),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = workingDirectory,
                    UserName = username,
                    Password = password.ToSecureString()
                }
            };
            process.OutputDataReceived += (s, e) => result = string.IsNullOrWhiteSpace(e.Data) ? result : e.Data;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();
            if (!process.HasExited)
                process.Kill();
            return result;
        }
    }

    static class Extensions
    {
        public static SecureString ToSecureString(this string s)
        {
            var ss = new SecureString();
            if (!string.IsNullOrEmpty(s))
                foreach (var c in s.ToCharArray())
                    ss.AppendChar(c);
            return ss;
        }
    }
}
