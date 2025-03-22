using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System.Runtime.Serialization;

namespace ITS
{
	namespace CommerceRuntime
	{
		[DataContract]
		public sealed class GenericRequest : Request
        {
            public GenericRequest(string _requestVal,string _objStrVal)
            {
                RequestVal = _requestVal;
                ObjStrVal = _objStrVal;
            }
            [DataMember]
            public string RequestVal { get; private set; }
			
			[DataMember]
            public string ObjStrVal { get; private set; }
        }
	}
}