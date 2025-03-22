

namespace Contoso.CommerceRuntime.Entities.DataModel
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using SystemAnnotations = System.ComponentModel.DataAnnotations;
    using System.Text;

    public class GasStationDetails:CommerceEntity
    {
        public GasStationDetails(string storeNumber, string itemId) : base ("GasStationDetails")
        {
            this.StoreNumber = storeNumber;
            this.GasolineItemId = itemId;
        }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [SystemAnnotations.Key]
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string StoreNumber { get; set; }

        [RequiredToBeUtc(true)]
        [DataMember]
        public DateTimeOffset LastGasDeliveryTime { get; set; }

        [RequiredToBeUtc(true)]
        [DataMember]
        public DateTimeOffset NextGasDeliveryTime { get; set; }

        [DataMember]
        public decimal GasBasePrice { get; set; }

        [DataMember]
        public int GasPumpCount { get; set; }

        [DataMember]
        public decimal GasTankCapacity { get; set; }

        [DataMember]
        public decimal GasTankLevel { get; set; }

        [DataMember]
        public string GasolineItemId { get; set; }
    }
}
