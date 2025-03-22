namespace ITS
{
    namespace CommerceRuntime.DataModel
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using SystemAnnotations = System.ComponentModel.DataAnnotations;
        
        [DataContract]
        public class GenericStringDataModel : CommerceEntity
        {
            public GenericStringDataModel()
                : base("GenericStringDataModel")
            {}

            private const string DataResultColumn = "DATARESULT";

            [SystemAnnotations.Key]
            [DataMember]
            [Column(DataResultColumn)]
            public string DataResult
            {
                get { return (string)this[DataResultColumn]; }
                set { this[DataResultColumn] = value; }
            }
        }

        [DataContract]
        public class GenericIntDataModel : CommerceEntity
        {
            public GenericIntDataModel()
                : base("GenericIntDataModel")
            { }

            private const string DataResultColumn = "DATARESULT";

            [SystemAnnotations.Key]
            [DataMember]
            [Column(DataResultColumn)]
            public int DataResult
            {
                get { return (int)this[DataResultColumn]; }
                set { this[DataResultColumn] = value; }
            }
        }

        public class GenericDecimalDataModel : CommerceEntity
        {
            public GenericDecimalDataModel()
                : base("GenericDecimalDataModel")
            { }

            private const string DataResultColumn = "DATARESULT";

            [SystemAnnotations.Key]
            [DataMember]
            [Column(DataResultColumn)]
            public decimal DataResult
            {
                get { return (decimal)this[DataResultColumn]; }
                set { this[DataResultColumn] = value; }
            }
        }

    }
}