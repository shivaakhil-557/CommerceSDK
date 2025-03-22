namespace ITS
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using System.Collections.Generic;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contos.Diagnostic;
    using Microsoft.ApplicationInsights.DataContracts;
    using System.Net.Http.Headers;

    namespace CommerceRuntime
    {
        public class B2CUserManagementDataService : SingleAsyncRequestHandler<B2CUserManagementRequest>
        {
            protected override async Task<Response> Process (B2CUserManagementRequest request)
            {
                var trace = new TraceTelemetry("CRT executing request", SeverityLevel.Information);
                ThrowIf.Null(request, nameof(request));
                StoreConfiguration config = await StoreConfiguration.GetConfigurationAsync(request).ConfigureAwait(false);
                ITSRetailParametes retailParam = new ITSRetailParametes();
                GenericHelpers generic = new GenericHelpers();
                retailParam = await generic.GetRetailParametesAsync(request,config).ConfigureAwait(false);
                B2CHelpers b2CHelpers = new B2CHelpers();
                string response;
                try
                {
                    trace.Properties.Add("CustomDimensionColumn1", request.RequestContext.GetTerminalId().ToString());
                    trace.Properties.Add("CustomDimensionColumn2", "CRT demo - Save Cart request");
                    trace.Properties.Add(new KeyValuePair<string, string>("B2CUserManagementDataService","Executing the process"));
                 

                    response = await b2CHelpers.CheckCustomersExistInB2C(request, config).ConfigureAwait(false);
                    trace.Properties.Add(new KeyValuePair<string, string>("B2CUserManagementDataService", "Completed the process execution succesfully."));
                    ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);
                }
                catch (Exception ex)
                {
                    trace.Properties.Add(new KeyValuePair<string, string>("B2CUserManagementDataService", "Failed the process execution succesfully. "+ ex.Message));
                    ContosLogger.GetLogger(request.RequestContext).TrackTrace(trace);
                    response = ex.Message;
                }

                return new B2CUserManagementResponse(response);
            }
        }
    }
}