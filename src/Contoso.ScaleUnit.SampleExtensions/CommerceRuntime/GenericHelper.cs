using ITS.CommerceRuntime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.Data;
using Microsoft.Dynamics.Commerce.Runtime.Data.Types;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITS
{
	namespace CommerceRuntime
	{
		public class GenericHelpers
		{
            public static string GenerateNewPassword(int lowercase, int uppercase, int numerics)
            {
                string lowers = "abcdefghijklmnopqrstuvwxyz";
                string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string number = "0123456789";

                Random random = new Random();

                string generated = "!";
                for (int i = 1; i <= lowercase; i++)
                    generated = generated.Insert(
                        random.Next(generated.Length),
                        lowers[random.Next(lowers.Length - 1)].ToString()
                    );

                for (int i = 1; i <= uppercase; i++)
                    generated = generated.Insert(
                        random.Next(generated.Length),
                        uppers[random.Next(uppers.Length - 1)].ToString()
                    );

                for (int i = 1; i <= numerics; i++)
                    generated = generated.Insert(
                        random.Next(generated.Length),
                        number[random.Next(number.Length - 1)].ToString()
                    );

                return generated.Replace("!", string.Empty);
            }

            public async Task<ITSRetailParametes> GetRetailParametesAsync(Request request,StoreConfiguration config)
			{
				ITSRetailParametes retailParam = new ITSRetailParametes();
				DataSet ds = new DataSet();
				string newLine = Environment.NewLine;
				StringBuilder sb = new StringBuilder();
				sb.Append("DECLARE @DATAAREAID nvarchar(4) = '"+config.DataAreaId+"'" + newLine);
				sb.Append(@"SELECT ITSSUPPORTUSERID,ITSB2CEXTENSIONAPPCLIENTID,ITSCLIENTSECRET,
							ITSAPPLICATIONCLIENTID,ITSB2CTENANTID FROM ext.ITSRETAILPARAMETERS 
							WHERE DATAAREAID = @DATAAREAID");
				using (DatabaseContext db = new DatabaseContext(request.RequestContext))
				{
					ds = await db.ExecuteQueryDataSetAsync(sb.ToString(), new ParameterSet()).ConfigureAwait(false);
					if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
					{
						retailParam = DataHelpers.ConvertDataTable<ITSRetailParametes>(ds.Tables[0])[0];
					}
				}
				return retailParam;
			}

            public async Task<BasicResponse> GetCustomersByEmailIdAsync (Request request,StoreConfiguration config,string emailId,BasicResponse resp)
            {
                DataSet ds = new DataSet();
                List<CustomerExt> lstCustomers = new List<CustomerExt>();
                StringBuilder sb = new StringBuilder();
                sb.Append("DECLARE @DATAAREAID NVARCHAR(4) = '"+config.DataAreaId+"'");
                sb.Append(Environment.NewLine);
                sb.Append(@"SELECT A.ACCOUNTNUM AS CustomerId, B.NAME AS CustomerName, D.LOCATOR AS EmailId
                            FROM ax.CUSTTABLE A
                            INNER JOIN ax.DIRPARTYTABLE B ON A.PARTY = B.RECID 
                            INNER JOIN ax.DIRPARTYLOCATION C ON B.RECID = C.PARTY
                            INNER JOIN ax.LOGISTICSELECTRONICADDRESS D ON C.LOCATION = D.LOCATION
                            WHERE D.TYPE = 2 AND DATAAREAID = @DATAAREAID AND D.LOCATOR LIKE '%" + emailId+"%'");
                try
                {
                    using (DatabaseContext db = new DatabaseContext(request.RequestContext))
                    {
                        ds = await db.ExecuteQueryDataSetAsync(sb.ToString(), new ParameterSet()).ConfigureAwait(false);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            lstCustomers = DataHelpers.ConvertDataTable<CustomerExt>(ds.Tables[0]);
                        }
                    }
                    resp.Payload = lstCustomers;
                }
                catch (Exception ex)
                {
                    resp.Status = 201;
                    resp.Message = ex.Message;
                    resp.ErrorDetails = ex.StackTrace + "\n" + ex.Message;
                    resp.Remarks = ex.Message;
                    resp.Payload = ex.InnerException;
                }
                return resp;
            }
            
            public async Task<ICustomerSearchResult> IsCustomersByEmailIdPhoneAsync (Request request,StoreConfiguration config,string emailId,string phone)
            {
                ICustomerSearchResult result = new ICustomerSearchResult();
                int status = 0;
                string searchText = string.Empty;
                try
                {
                    InvokeExtensionMethodRealtimeRequest rtsRequest = new InvokeExtensionMethodRealtimeRequest("isCustomerExistsByEmailPhone", emailId,phone);
                    ReadOnlyCollection<object> containerArray = (await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(rtsRequest).ConfigureAwait(false)).Result;
                    if (Convert.ToString(containerArray[0]) != "")
                    {
                        status = Convert.ToInt32(containerArray[0]);
                        searchText = Convert.ToString(containerArray[1]);
                    }
                } 
                catch 
                {
                    DataSet ds = new DataSet();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("DECLARE @DATAAREAID NVARCHAR(4) = '"+config.DataAreaId+"'");
                    sb.Append("DECLARE @EMAILID NVARCHAR(250) = '%" + emailId+"%'");
                    sb.Append("DECLARE @PHONE NVARCHAR(20) = '%" + phone+"%'");
                    sb.Append(Environment.NewLine);
                    sb.Append(@"IF @EMAILID <> '%%' AND @PHONE <> '%%'
	                                IF EXISTS (SELECT 1
				                                FROM ax.CUSTTABLE A
				                                INNER JOIN ax.DIRPARTYTABLE B ON A.PARTY = B.RECID 
				                                INNER JOIN ax.DIRPARTYLOCATION C ON B.RECID = C.PARTY
				                                INNER JOIN ax.LOGISTICSELECTRONICADDRESS D ON C.LOCATION = D.LOCATION
				                                WHERE DATAAREAID = @DATAAREAID AND D.LOCATOR LIKE @EMAILID) 
					                                SELECT 1 AS FLAG, REPLACE(@EMAILID,'%','') AS SEARCHTEXT
	                                ELSE
		                                IF EXISTS (SELECT 1
				                                FROM ax.CUSTTABLE A
				                                INNER JOIN ax.DIRPARTYTABLE B ON A.PARTY = B.RECID 
				                                INNER JOIN ax.DIRPARTYLOCATION C ON B.RECID = C.PARTY
				                                INNER JOIN ax.LOGISTICSELECTRONICADDRESS D ON C.LOCATION = D.LOCATION
				                                WHERE DATAAREAID = 'usrt' AND D.LOCATOR LIKE @PHONE) 
					                                SELECT 1 AS FLAG, REPLACE(@PHONE,'%','') AS SEARCHTEXT
		                                ELSE
					                                SELECT 0 AS FLAG, '' AS SEARCHTEXT
                                ELSE
	                                IF @PHONE <> '%%'
		                                IF EXISTS (SELECT 1
					                                FROM ax.CUSTTABLE A
					                                INNER JOIN ax.DIRPARTYTABLE B ON A.PARTY = B.RECID 
					                                INNER JOIN ax.DIRPARTYLOCATION C ON B.RECID = C.PARTY
					                                INNER JOIN ax.LOGISTICSELECTRONICADDRESS D ON C.LOCATION = D.LOCATION
					                                WHERE DATAAREAID = @DATAAREAID AND D.LOCATOR LIKE @PHONE) 
						                                SELECT 1 AS FLAG, REPLACE(@PHONE,'%','') AS SEARCHTEXT
		                                ELSE
						                                SELECT 0 AS FLAG, '' AS SEARCHTEXT
	                                ELSE
		                                SELECT 0 AS FLAG , '' AS SEARCHTEXT");
                    using (DatabaseContext db = new DatabaseContext(request.RequestContext))
                    {
                        ds = await db.ExecuteQueryDataSetAsync(sb.ToString(),new ParameterSet()).ConfigureAwait(false);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            status = Convert.ToInt32(ds.Tables[0].Rows[0]["FLAG"]);
                            searchText = Convert.ToString(ds.Tables[0].Rows[0]["SEARCHTEXT"]);
                        }
                    }
                }
                result.Status = status;
                result.SearchText = searchText;
                return result;
            }
		}
	}
}