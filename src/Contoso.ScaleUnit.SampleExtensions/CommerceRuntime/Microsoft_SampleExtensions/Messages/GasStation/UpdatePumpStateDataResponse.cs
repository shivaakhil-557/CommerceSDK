namespace Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// A response class representing the result of updating the gas pump state.
    /// </summary>
    [DataContract]
    public sealed class UpdatePumpStateDataResponse : Response
    {
        public UpdatePumpStateDataResponse(GasPump pump)
        {
            Pump = pump;
        }

        /// <summary>
        /// The gas pump.
        /// </summary>
        public GasPump Pump { get; private set; }
    }
}