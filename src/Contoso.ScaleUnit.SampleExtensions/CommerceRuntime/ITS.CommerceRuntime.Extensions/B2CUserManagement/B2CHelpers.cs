namespace ITS
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System;
    using Newtonsoft.Json;
    using CRT = Microsoft.Dynamics.Commerce.Runtime.Messages;
    using System.Text;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.Data.Types;
    using Microsoft.Dynamics.Commerce.Runtime;
    using System.Net;

    namespace CommerceRuntime
	{
		public class B2CHelpers
		{
            static readonly string baseUrl = "http://localhost:7075/api/AzureFuncB2C";
            static readonly string PublicBaseUrl = "https://searchusersinb2c.azurewebsites.net/api/AzureFuncB2C";
            public async Task<string> CheckCustomersExistInB2C (CRT.Request request,StoreConfiguration config)
            {
                string base64ExcelStr = string.Empty;
                GenericHelpers generic = new GenericHelpers();
                List<UpdatedCustomersToSearchInB2C> lstUpdatedCustomersInFNO = new List<UpdatedCustomersToSearchInB2C>();
                List<CustomersToSearchInB2C> lstCustomersInFNO = new List<CustomersToSearchInB2C>();
                ITSRetailParametes retailParam = new ITSRetailParametes();
                retailParam = await generic.GetRetailParametesAsync(request, config).ConfigureAwait(false);
                lstCustomersInFNO = await GetCustomersToSearchAsync(request, config).ConfigureAwait(false);
                WebHeaderCollection headerCollection = new WebHeaderCollection();
                headerCollection["TenantId"] = retailParam.ITSB2CTENANTID;
                headerCollection["AppId"] = retailParam.ITSAPPLICATIONCLIENTID;
                headerCollection["ClientSecret"] = retailParam.ITSCLIENTSECRET;
                headerCollection["B2cExtensionAppClientId"] = retailParam.ITSB2CEXTENSIONAPPCLIENTID;
                string PostData = JsonConvert.SerializeObject(lstCustomersInFNO);
                PostAPIResponse postResponse = await ExecutePostApiAsync(PublicBaseUrl, headerCollection, PostData, "POST").ConfigureAwait(false);
                if (postResponse.Status == (int)HttpStatusCode.OK && !string.IsNullOrEmpty(postResponse.Result.ToString()))
                {
                    FuncSearchB2CResult result = JsonConvert.DeserializeObject<FuncSearchB2CResult>(postResponse.Result.ToString());
                    lstUpdatedCustomersInFNO = JsonConvert.DeserializeObject<List<UpdatedCustomersToSearchInB2C>>(result.Payload);
                    base64ExcelStr = DataHelpers<UpdatedCustomersToSearchInB2C>.GenerateExcelFileAsBase64Str(lstUpdatedCustomersInFNO, "Customers_Searched_In_B2C");
                }
                return base64ExcelStr;
            }

            public async Task<List<CustomersToSearchInB2C>> GetCustomersToSearchAsync(CRT.Request request,StoreConfiguration config)
            {
                string newLine = Environment.NewLine;
                List<CustomersToSearchInB2C> lstCustomers = new List<CustomersToSearchInB2C>();
                StringBuilder sb = new StringBuilder();
                DataSet ds = new DataSet();
                sb.Append("DECLARE @DATAAREAID nvarchar(4) = '"+config.DataAreaId+"'" + newLine);
                sb.Append(@"SELECT A.ACCOUNTNUM AS CustomerId ,B.NAME AS CustomerName, LOWER(D.LOCATOR) As PersonalEmailId,A.CUSTGROUP AS CustomerGroup
                            ,A.CURRENCY AS Currency ,A.INVENTLOCATION As InventoryLocation ,A.INVENTSITEID AS InventorySite,A.TAXGROUP AS TaxGroup
                            ,CAST(A.CREDITMAX AS NUMERIC(28,2)) AS CreditLimit 
                            FROM ax.CUSTTABLE A 
                            INNER JOIN ax.DIRPARTYTABLE B ON A.PARTY = B.RECID 
                            INNER JOIN ax.DIRPARTYLOCATION C ON B.RECID = C.PARTY 
                            INNER JOIN ax.LOGISTICSELECTRONICADDRESS D ON C.LOCATION = D.LOCATION
                            WHERE D.TYPE = 2 AND D.ISPRIMARY = 1 AND A.DATAAREAID = @DATAAREAID");
                using (DatabaseContext db = new DatabaseContext(request.RequestContext))
                {
                    ds = await db.ExecuteQueryDataSetAsync(sb.ToString(), new ParameterSet()).ConfigureAwait(false);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        lstCustomers = DataHelpers.ConvertDataTable<CustomersToSearchInB2C>(ds.Tables[0]);
                    }
                }
                return lstCustomers;
            }

            private async Task<PostAPIResponse> ExecutePostApiAsync(string requestUrl,WebHeaderCollection headerCollection, string postData, string requestMethod)
            {
                int StatusCode;
                try
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = requestMethod.ToUpper();
                    httpWebRequest.Headers = headerCollection;
                    if (requestMethod == "GET")
                    {
                        var httpResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync().ConfigureAwait(false));
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            StatusCode = (int)httpResponse.StatusCode;
                            var result = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                            return new PostAPIResponse
                            {
                                Status = StatusCode,
                                Result = result
                            };
                        }
                    }
                    else
                    {
                        using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync().ConfigureAwait(false)))
                        {
                            await streamWriter.WriteAsync(postData).ConfigureAwait(false);
                            await streamWriter.FlushAsync().ConfigureAwait(false);
                            streamWriter.Close();

                            var httpResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync().ConfigureAwait(false));
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                StatusCode = (int)httpResponse.StatusCode;
                                var result = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                                return new PostAPIResponse
                                {
                                    Status = StatusCode,
                                    Result = result
                                };
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    using (StreamReader r = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                        var result = await r.ReadToEndAsync().ConfigureAwait(false);
                        return new PostAPIResponse
                        {
                            Status = StatusCode,
                            Result = result
                        };
                    }
                }
            }

        }
    }
}