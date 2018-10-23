using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Rhythm.Diagnostics.RadarGun {

    /// <summary>
    /// Attaches a <seealso cref="RadarGun"/> instance to the current HttpContext.
    /// Provides failure protection if a RadarGun instance is not running on the current request.
    /// </summary>
    public static class HttpContextRadarGun {

        public static RadarGun Current {
            get { return (RadarGun)HttpContext.Current?.Items["RadarGun_Current"]; }
            set {
                if (HttpContext.Current == null)
                    throw new Exception("Cannot set RadarGun.Current because HttpContext is null.");
                HttpContext.Current.Items["RadarGun_Current"] = value;
            }
        }

        public static void Start(string message = null) {
            Current = RadarGun.StartNew(message);
        }

        public static void Stop(string message = null) {
            Current?.Stop();
        }

        public static void AddMilestone(string message) {
            Current?.AddMilestone(message);
        }

        public static void AddTimedMilestone(string message, Action task) {
            if (Current != null)
                Current.AddTimedMilestone(message, task);
            else
                task();
        }

        public static T AddTimedMilestone<T>(string message, Func<T> task) {
            if (Current != null)
                return Current.AddTimedMilestone(message, task);
            else
                return task();
        }

        public static string GetMessages() {
            if (Current != null)
                return Current.ToString();
            else
                return String.Empty;
        }

    }
}
