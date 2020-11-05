﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockCheck
{
    /// <summary>
    /// Provides information about a process that is holding a lock on a file.
    /// </summary>
    public abstract class ProcessInfo
    {
        protected ProcessInfo(int processId, DateTime startTime)
        {
            ProcessId = processId;
            StartTime = startTime;
        }

        /// <summary>
        /// The process identifier of the process holding a lock on a file.
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        /// The start time (local) of the process holding a lock o a file.
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// The executable name of the process holding a lock.
        /// </summary>
        public string ExecutableName { get; protected set; }

        /// <summary>
        /// The descriptive application name, if available. Otherwise
        /// the same as the executable name or another informative
        /// string.
        /// </summary>
        public string ApplicationName { get; protected set; }

        /// <summary>
        /// The owner of the process.
        /// </summary>
        public string Owner { get; protected set; }

        /// <summary>
        /// The full path to the process' executable, if available.
        /// </summary>
        public string ExecutableFullPath { get; protected set; }

        /// <summary>
        /// The platform specific session ID of the process.
        /// </summary>
        /// <value>
        /// On Windows, the Terminal Services ID. On Linux
        /// the process' session ID.
        /// </value>
        public int SessionId { get; protected set; }

        /// <summary>
        /// A platform specific string that specifies the type of lock, if available.
        /// </summary>
        public string LockType { get; protected set; }

        /// <summary>
        /// A platform specific string that specifies the mode of the lock, if available.
        /// </summary>
        public string LockMode { get; protected set; }

        /// <summary>
        /// A platform specific string that specifies the access lock requested, if available.
        /// </summary>
        public string LockAccess { get; protected set; }

        public override int GetHashCode()
        {
            var h1 = ProcessId.GetHashCode();
            var h2 = StartTime.GetHashCode();
            return ((h1 << 5) + h1) ^ h2;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ProcessInfo;
            if (other != null)
            {
                return other.ProcessId == ProcessId && other.StartTime == StartTime;
            }
            return false;
        }

        public override string ToString()
        {
            return ProcessId + "@" + StartTime.ToString("s");
        }

        public string ToString(string format)
        {
            if (format == null)
            {
                return ToString();
            }

            if (format == "F")
            {
                return ToString() + "/" + ApplicationName;
            }

            if (format == "A")
            {
                var sb = new StringBuilder();
                sb.Append(nameof(ProcessId)).Append(": ").Append(ProcessId).AppendLine();
                sb.Append(nameof(StartTime)).Append(": ").Append(StartTime).AppendLine();
                sb.Append(nameof(ExecutableName)).Append(": ").Append(ExecutableName).AppendLine();
                sb.Append(nameof(ApplicationName)).Append(": ").Append(ApplicationName).AppendLine();
                sb.Append(nameof(Owner)).Append(": ").Append(Owner).AppendLine();
                sb.Append(nameof(ExecutableFullPath)).Append(": ").Append(ExecutableFullPath).AppendLine();
                sb.Append(nameof(SessionId)).Append(": ").Append(SessionId).AppendLine();
                sb.Append(nameof(LockType)).Append(": ").Append(LockType).AppendLine();
                sb.Append(nameof(LockMode)).Append(": ").Append(LockMode).AppendLine();
                sb.Append(nameof(LockAccess)).Append(": ").Append(LockAccess).AppendLine();
                return sb.ToString();
            }

            return ToString();
        }

        public static void Format(StringBuilder sb, IEnumerable<ProcessInfo> lockers, IEnumerable<string> fileNames, int? max = null)
        {
            if (lockers == null || !lockers.Any())
                return;

            int count = lockers.Count();
            sb.AppendFormat("File {0} locked by: ", string.Join(", ", fileNames));
            foreach (var locker in lockers.Take(max ?? Int32.MaxValue))
            {
                sb.AppendLine($"[{locker.ApplicationName}, pid={locker.ProcessId}, owner={locker.Owner}, started={locker.StartTime:yyyy-MM-dd HH:mm:ss.fff}]");
            }

            if (count > max)
            {
                sb.AppendLine($"[{count - max} more processes...]");
            }
        }
    }
}