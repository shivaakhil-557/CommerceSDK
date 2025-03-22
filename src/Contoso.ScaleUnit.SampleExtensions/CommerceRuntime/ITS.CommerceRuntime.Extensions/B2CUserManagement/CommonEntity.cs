namespace ITS { 

    namespace CommerceRuntime
    {
        public class UpdatedCustomersToSearchInB2C : CustomersToSearchInB2C
        {
            public UpdatedCustomersToSearchInB2C()
            {
                IsExistsInB2C = 0;
            }
            public int IsExistsInB2C { get; set; }
        }

        public class CustomersToSearchInB2C
        {
            public CustomersToSearchInB2C()
            {
                CustomerId = string.Empty;
                CustomerName = string.Empty;
                PersonalEmailId = string.Empty;
                CustomerGroup = string.Empty;
                Currency = string.Empty;
                InventorySite = string.Empty;
                InventorySite = string.Empty;
                CreditLimit = 0.00m;

            }
            public string CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string PersonalEmailId { get; set; }
            public string CustomerGroup { get; set; }
            public string Currency { get; set; }
            public string InventoryLocation { get; set; }
            public string InventorySite { get; set; }
            public decimal CreditLimit { get; set; }
        }

        public class PostAPIResponse
        {
            public int Status { get; set; }

            public object Result { get; set; }
        }

        public class FuncSearchB2CResult
        {
            public FuncSearchB2CResult()
            {
                Status = 200;
                Payload = string.Empty;
                Message = "Success";
                ErrorDetails = string.Empty;
                Remarks = string.Empty;
            }
            public string Payload { get; set; }
            public int Status { get; set; }
            public string Message { get; set; }
            public string Remarks { get; set; }
            public string ErrorDetails { get; set; }

        }
        public class ITSRetailParametes
        {
            public ITSRetailParametes()
            {
                DATAAREAID = "dat";
                ITSAPPLICATIONCLIENTID = string.Empty;
                ITSB2CEXTENSIONAPPCLIENTID = string.Empty;
                ITSB2CTENANTID = string.Empty;
                ITSCLIENTSECRET = string.Empty;
                ITSSUPPORTUSERID = string.Empty;
            }
            public string DATAAREAID { get; set; }
            public string ITSSUPPORTUSERID { get; set; }
            public string ITSB2CEXTENSIONAPPCLIENTID { get; set; }
            public string ITSCLIENTSECRET { get; set; }
            public string ITSAPPLICATIONCLIENTID { get; set; }
            public string ITSB2CTENANTID { get; set; }

        }

        interface IBasicResponse
        {
            int Status { get; set; }
            string Message { get; set; }
            string Remarks { get; set; }
            string ErrorDetails { get; set; }
            dynamic Payload { get; set; }
        }

        public class BasicResponse : IBasicResponse
        {
            public BasicResponse()
            {
                Status = 200;
                Message = "Success";
                ErrorDetails = string.Empty;
                Remarks = string.Empty;
                Payload = string.Empty;
            }
            public int Status { get; set; }
            public string Message { get; set; }
            public string Remarks { get; set; }
            public string ErrorDetails { get; set; }
            public dynamic Payload { get; set; }
        }

        public class CustomerExt
        {
            public CustomerExt()
            {
                CustomerId = string.Empty;
                CustomerName = string.Empty;
                EmailId = string.Empty;
            }
            public string CustomerId { get; set; }
            public string CustomerName{ get; set; }
            public string EmailId { get; set; }
        }
        
    }
}
