using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.Extensions;

namespace Contos.Diagnostic
{
    public static class ContosLogger
    {
        private static readonly object lockObject = new object();
        private static TelemetryClient client = null;
        public static TelemetryClient GetLogger(RequestContext context)
        {
            if (client == null)
            {
                lock (lockObject)
                {
                    if (client == null)
                    {
                        string key = context.Runtime.Configuration.GetSettingValue("ext.AppInsightsKey") ?? string.Empty;
                        client = new TelemetryClient(new TelemetryConfiguration(key));
                    }
                }
            }
            return client;
        }
    }
}