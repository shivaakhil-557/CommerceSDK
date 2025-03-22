using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Threading.Tasks;

namespace Contoso.CommerceRuntime.RequestHandlers
{
    public class ExtendNotificationService : SingleAsyncRequestHandler<GetNotificationsExtensionServiceRequest>
    {
        protected override async Task<Response> Process(GetNotificationsExtensionServiceRequest request)
        {
            ThrowIf.Null(request, "request");
            NotificationDetailCollection details = new NotificationDetailCollection();
            DateTimeOffset lastNotificationDateTime = DateTimeOffset.Now;
            string myOperationId = "5057";
            
            // do the actual work here
            if (request.SubscribedOperation.ToString() == myOperationId)
            {
                NotificationDetail detail = new NotificationDetail()
                {
                    // Text which will display for the notification detail in the POS notification center
                    DisplayText = "Check the customers existing in Azure B2C",
                    // Number of notifications found
                    ItemCount = 1,
                    // Timestamp of creation of latest notification item (Used to determine whether notification is new)
                    LastUpdatedDateTime = lastNotificationDateTime,
                    // Boolean value representing whether the attempt to get notifications for the given operation was successful
                    IsSuccess = true,
                    // If you would like POS to navigate to a specific action property for the given operation
                    // when the notification tile is selected, define the action property as well.
                    // This property can be configured using the Operation parameter field of the button grid designer and passed to the CRT code.
                    ActionProperty = "57"
                };
                details.Add(detail);
            }
            var serviceResponse = new GetNotificationsExtensionServiceResponse(details);
            return await Task.FromResult(serviceResponse).ConfigureAwait(false);
        }
    }

}