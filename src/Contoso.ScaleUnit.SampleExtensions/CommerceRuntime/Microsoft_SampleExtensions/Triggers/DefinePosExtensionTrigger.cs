﻿/**
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
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contos.Diagnostic;
    using Microsoft.ApplicationInsights.DataContracts;


    /// <summary>
    /// Class that implements a post trigger for the GetCustomerDataRequest request type.
    /// </summary>
    public class DefinePosExtensionTrigger : IRequestTriggerAsync
    {
        /// <summary>
        /// Gets the supported requests for this trigger.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(GetExtensionPackageDefinitionsRequest) };
            }
        }

        /// <summary>
        /// Post trigger code to retrieve extension properties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");


            var trace = new TraceTelemetry("CRT executing request", SeverityLevel.Information);
            trace.Properties.Add("CustomDimensionColumn1", request.RequestContext.GetTerminalId().ToString());
            trace.Properties.Add("CustomDimensionColumn2", "CRT demo - Save Cart request");
            ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);

            var getExtensionsResponse = (GetExtensionPackageDefinitionsResponse)response;
            var extensionPackageDefinition = new ExtensionPackageDefinition();
            extensionPackageDefinition.Name = "Contoso.Commerce.Extensions.Full";
            extensionPackageDefinition.Publisher = "ITS";
            extensionPackageDefinition.IsEnabled = true;

            getExtensionsResponse.ExtensionPackageDefinitions.Add(extensionPackageDefinition);

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