﻿namespace Contoso.CommerceRuntime.Messages
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// A class representing the response to the get gas pumps data request.
    /// </summary>
    [DataContract]
    public sealed class GetGasPumpsDataResponse : Response
    {
        public GetGasPumpsDataResponse(IEnumerable<GasPump> pumps)
        {
            GasPumps = pumps;
        }

        /// <summary>
        /// The gas pumps.
        /// </summary>
        public IEnumerable<GasPump> GasPumps { get; private set; }
    }
}