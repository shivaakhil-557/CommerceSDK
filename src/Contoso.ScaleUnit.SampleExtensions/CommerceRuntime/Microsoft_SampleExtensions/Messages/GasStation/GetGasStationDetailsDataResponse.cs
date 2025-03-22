namespace Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// A class representing the response to the get gas station details request.
    /// </summary>
    [DataContract]
    public sealed class GetGasStationDetailsDataResponse : Response
    {
        public GetGasStationDetailsDataResponse(GasStationDetails details)
        {
            Details = details;
        }

        /// <summary>
        /// The gas station details.
        /// </summary>
        public GasStationDetails Details { get; private set; }
    }
}