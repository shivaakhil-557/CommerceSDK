namespace Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// A class representing the request to get the gas pumps data.
    /// </summary>
    [DataContract]
    public sealed class GetGasPumpsDataRequest : Request
    {
        public GetGasPumpsDataRequest(string storeNumber)
        {
            StoreNumber = storeNumber;
        }

        /// <summary>
        /// The store number.
        /// </summary>
        public string StoreNumber { get; private set; }
    }
}