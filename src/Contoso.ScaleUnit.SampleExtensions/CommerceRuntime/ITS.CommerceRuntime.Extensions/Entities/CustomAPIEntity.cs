namespace ITS
{
    namespace CommerceRuntime.DataModel
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using SystemAnnotations = System.ComponentModel.DataAnnotations;

        /// <summary>
        /// Defines a simple class that holds information about custom api entries in the controller available to expose.
        /// </summary>
        public class CustomAPIEntity : CommerceEntity
        {
            private const string IdColumn = "ID";

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomAPIEntity"/> class.
            /// </summary>
            public CustomAPIEntity()
                : base("CustomAPIEntity")
            {
            }

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <remarks>
            /// Fields named "Id" are automatically treated as the entity key.
            /// If a name other than Id is preferred, <see cref="System.ComponentModel.DataAnnotations.KeyAttribute"/>
            /// can be used like it is here to annotate a given field as the entity key.
            /// </remarks>
            [SystemAnnotations.Key]
            [DataMember]
            [Column(IdColumn)]
            public long Id
            {
                get { return (long)this[IdColumn]; }
                set { this[IdColumn] = value; }
            }
        }

        interface ICustomerSearch
        {
            int Status { get; set; }
            string SearchText { get; set; }
        }

        public class ICustomerSearchResult : ICustomerSearch
        {
            public ICustomerSearchResult()
            {
                Status = 0;
                SearchText = string.Empty;
            }
            public int Status { get; set; }
            public string SearchText { get; set; }
        }
    }

}