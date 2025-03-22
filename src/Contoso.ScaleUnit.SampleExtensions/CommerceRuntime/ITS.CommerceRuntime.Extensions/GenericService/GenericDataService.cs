namespace ITS
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using System.Collections.Generic;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Newtonsoft.Json;

    namespace CommerceRuntime
    {
        public class GenericDataService : IRequestHandlerAsync
        {
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GenericRequest)
                    };
                }
            }

            public async Task<Response> Execute (Request request)
            {
                if (request == (GenericRequest)request)
                {
                    return await ExecuteGenericDataServiceAsync((GenericRequest)request).ConfigureAwait(false);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            private async Task<GenericResponse> ExecuteGenericDataServiceAsync(GenericRequest request)
            {
                StoreConfiguration config = await StoreConfiguration.GetConfigurationAsync(request).ConfigureAwait(false);
                dynamic requestObj = JsonConvert.DeserializeObject<dynamic>(request.RequestVal);
                string _Operation = requestObj.Operation;
                BasicResponse resp = new BasicResponse();
                GenericHelpers generic = new GenericHelpers();
                try
                {
                    switch (_Operation)
                    {
                        case "GETEXISTINGCUSTOMERS":
                            string emailId = requestObj.EMAILID;
                            resp = await generic.GetCustomersByEmailIdAsync(request, config, emailId, resp).ConfigureAwait(false);
                            break;

                        case "ISEXISTINGCUSTOMERS":
                            resp.Payload = await generic.IsCustomersByEmailIdPhoneAsync(request, config, (string)requestObj.EMAILID,(string)requestObj.PHONE).ConfigureAwait(false);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    resp.Status = 201;
                    resp.Message = ex.Message;
                    resp.ErrorDetails = ex.StackTrace + "\n" + ex.Message;
                    resp.Remarks = ex.Message;
                    resp.Payload = ex.InnerException;
                }
                return new GenericResponse(JsonConvert.SerializeObject(resp));
            }
        }
    }
}