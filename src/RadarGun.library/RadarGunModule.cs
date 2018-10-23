using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Rhythm.Diagnostics.RadarGun {

    /// <summary>
    /// Uses a <seealso cref="RadarGun"/> instance to automatically measure each request's timeline.
    /// Provides the resulting information on each request in a custom response header value.
    /// Only runs when the site is running in debug mode.
    /// </summary>
    public class RadarGunModule : IHttpModule {

        public void Init(HttpApplication context) {
            context.BeginRequest += BeginRequest;
            context.PreSendRequestHeaders += Context_PreSendRequestHeaders;
        }

        private void BeginRequest(object sender, EventArgs e) {
            if (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled)
                HttpContextRadarGun.Start();
        }

        private void Context_PreSendRequestHeaders(object sender, EventArgs e) {
            HttpContextRadarGun.Stop();
            if (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled) {
                HttpContext.Current.Response.Headers.Add("RadarGun", HttpContextRadarGun.GetMessages());
            }
        }

        public void Dispose() { }

    }
}
