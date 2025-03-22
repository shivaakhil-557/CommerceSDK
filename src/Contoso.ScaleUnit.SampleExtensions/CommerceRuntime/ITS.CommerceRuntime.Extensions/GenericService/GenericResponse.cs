using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System.Runtime.Serialization;

namespace ITS
{
	namespace CommerceRuntime
	{
		[DataContract]
		public sealed class GenericResponse : Response
		{
            public GenericResponse(string _ResponseVal)
            {
                ResponseVal = _ResponseVal;
            }
            public string ResponseVal { get; set; }
        }
    }
}