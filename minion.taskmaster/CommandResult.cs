using System;
using System.Collections.Generic;

namespace minion.taskmaster
{
    public class CommandResult
    {
        public Command CommandBefore { get; set; }
        public Command CommandAfter { get; set; }
        public IEnumerable<EnvironmentVariable> EnvironmentBefore { get; set; }
        public IEnumerable<EnvironmentVariable> EnvironmentAfter { get; set; }
        public int ExitCode { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ExitTime { get; set; }
        public TimeSpan TotalProcessorTime { get; set; }
        public TimeSpan UserProcessorTime { get; set; }
        public TimeSpan PrivilegedProcessorTime { get; set; }
    }
}
