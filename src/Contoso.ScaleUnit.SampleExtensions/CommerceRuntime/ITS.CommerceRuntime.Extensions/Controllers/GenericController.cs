namespace ITS
{
    namespace CommerceRuntime
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using ITS.CommerceRuntime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        [RoutePrefix("GenericController")]
        [BindEntity(typeof(CustomAPIEntity))]
        public class GenericController : IController
        {
            [HttpPost]
            [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Application, CommerceRoles.Storefront)]
            public async Task<GenericStringDataModel> ExecuteGenericRequest(IEndpointContext context,string RequestVal,string ObjStrVal)
            {
                GenericStringDataModel result = new GenericStringDataModel();
                GenericRequest request = new GenericRequest(RequestVal, ObjStrVal);
                GenericResponse response = await context.ExecuteAsync<GenericResponse>(request).ConfigureAwait(false);
                result.DataResult = response.ResponseVal;
                return result;
            }
        }
    }
}