/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso.CommerceRuntime.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.ApplicationInsights.DataContracts;
    using Contos.Diagnostic;

    public class AddExtensionPropertiesToProductSearchResults : IRequestTriggerAsync
    {
        // This key must match the name of the setting included in the CommerceRuntimeExtensionSettings.
        private const string IncludeExtensionPropertiesInProductSearchKey = "ext.Contoso.IncludeExtensionPropertiesInProductSearch";
        private const string ExtensionPropertyName = "CONTOSO_PRODUCT_VERSION";

        /// <summary>
        /// Gets the supported requests for this trigger.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(SearchProductsServiceRequest) };
            }
        }

        /// <summary>
        /// Post trigger code to retrieve extension properties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(response, nameof(response));

            var scaleUnitId = request.RequestContext.GetChannelConfiguration();

            var trace = new TraceTelemetry($"Executed request of type {request.GetType().Name}", SeverityLevel.Information);
            trace.Properties.Add("Step", "Execution started");
            ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);

            SearchProductsServiceResponse searchProductsResponse = response as SearchProductsServiceResponse;
            if (searchProductsResponse != null)
            {
                trace = new TraceTelemetry("AddExtensionPropertiesToProductSearchResults Trigger : Found SearchProductsServiceResponse", SeverityLevel.Information);
                trace.Properties.Add("Step", "SearchProductsServiceResponse identified");
                ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);

                // Only include the version number in the extension properties if it is configured in the CommerceRuntime config file.
                if (request.RequestContext.Runtime.Configuration.Settings.TryGetValue(IncludeExtensionPropertiesInProductSearchKey, out string settingValue))
                {
                    trace = new TraceTelemetry("AddExtensionPropertiesToProductSearchResults Trigger :Configuration setting found", SeverityLevel.Information);
                    trace.Properties.Add("Step", "Configuration setting retrieved");
                    trace.Properties.Add("SettingValue", settingValue);
                    ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);

                    if (bool.TryParse(settingValue, out bool shouldAddExtensionProperties) && shouldAddExtensionProperties)
                    {
                        trace = new TraceTelemetry("AddExtensionPropertiesToProductSearchResults Trigger : Should add extension properties", SeverityLevel.Information);
                        trace.Properties.Add("Step", "Parsed configuration setting");
                        trace.Properties.Add("ShouldAddExtensionProperties", shouldAddExtensionProperties.ToString());
                        ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);

                        var random = new Random();
                        foreach (ProductSearchResult productSearchResult in searchProductsResponse.ProductSearchResults)
                        {
                            var version = Math.Round((random.NextDouble() + 0.1) * random.Next(1, 10), 2); // Generate a random version number.
                            productSearchResult.SetProperty(ExtensionPropertyName, version.ToString());

                            trace = new TraceTelemetry("AddExtensionPropertiesToProductSearchResults Trigger : Set property on product search result", SeverityLevel.Information);
                            trace.Properties.Add("Step", "Set property on product search result");
                            trace.Properties.Add("ProductId", productSearchResult.ItemId.ToString());
                            trace.Properties.Add("Version", version.ToString());
                            ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);
                        }
                    }
                }
            }

            trace = new TraceTelemetry("AddExtensionPropertiesToProductSearchResults Trigger : OnExecuted method execution completed", SeverityLevel.Information);
            trace.Properties.Add("Step", "Execution completed");
            ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);
            return Task.CompletedTask;
        }


        /// <summary>
        /// Pre trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        public Task OnExecuting(Request request)
        {
            return Task.CompletedTask;
        }
    }
}
