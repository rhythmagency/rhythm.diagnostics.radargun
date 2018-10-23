using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Rhythm.Diagnostics.RadarGun {

    /// <summary>
    /// Provides precision measurement of task timing and duration.
    /// </summary>
    public class RadarGun {

        protected RadarGun() {
            Timer = new Stopwatch();
            Milestones = new List<Tuple<TimeSpan, string>>();
        }

        public DateTimeOffset StartTime { get; protected set; }
        public Stopwatch Timer { get; protected set; }
        public List<Tuple<TimeSpan, string>> Milestones { get; protected set; }

        public void Start(string message = null) {
            if (!Timer.IsRunning) {
                StartTime = DateTimeOffset.Now;
                this.AddMilestone(message ?? "RadarGun started");
                Timer.Start();
            }
        }

        public void Stop(string message = null) {
            if (Timer.IsRunning) {
                Timer.Stop();
                this.AddMilestone(message ?? "RadarGun stopped");
            }
        }

        public void AddMilestone(string message) {
            Milestones.Add(new Tuple<TimeSpan, string>(Timer.Elapsed, message));
        }

        public void AddTimedMilestone(string message, Action task) {
            this.AddMilestone($"Started {message}");
            var subTimer = Stopwatch.StartNew();
            try {
                task();
            }
            finally {
                subTimer.Stop();
                this.AddMilestone($"Finished {message} | time taken: {subTimer.ElapsedMilliseconds} ms");
            }
        }

        public T AddTimedMilestone<T>(string message, Func<T> task) {
            this.AddMilestone($"Started {message}");
            var subTimer = Stopwatch.StartNew();
            try {
                return task();
            }
            finally {
                subTimer.Stop();
                this.AddMilestone($"Finished {message} | time taken: {subTimer.ElapsedMilliseconds} ms");
            }
        }

        public override string ToString() {
            return JsonConvert.SerializeObject(Milestones.Select(m => new { timeMark = m.Item1.TotalMilliseconds, message = m.Item2 }).ToArray());
        }

    #region static members

        public static RadarGun StartNew(string message = null) {
            var result = new RadarGun();
            result.Start(message);
            return result;
        }

    #endregion

    }
}
