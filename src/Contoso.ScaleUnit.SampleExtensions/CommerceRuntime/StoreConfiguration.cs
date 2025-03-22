namespace ITS
{
    namespace CommerceRuntime
    {
		using System;
		using System.Threading.Tasks;
		using Microsoft.Dynamics.Commerce.Runtime;
		using Microsoft.Dynamics.Commerce.Runtime.DataModel;
		using Microsoft.Dynamics.Commerce.Runtime.Messages;
		using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

		public class StoreConfiguration
        {
            public StoreConfiguration()
            {
                this.Channel = 0;
                this.DataAreaId = "";
                this.LanguageID = "";
                this.StaffId = "";
                this.Store = "";
                this.TerminalId = "";
                this.Currency = "";
                this.StoreName = "";
                this.FuncProfileId = "";
                this.StaffName = "";
                //this.CurrentStore = null;
            }
            public string TerminalId { get; set; }
            public long Channel { get; set; }
            public string Store { get; set; }
            public string StoreName { get; set; }
            public string DataAreaId { get; set; }
            public string StaffId { get; set; }
            public string StaffName { get; set; }
            public string LanguageID { get; set; }
            public string Currency { get; set; }
            public DateTime CurrentDate { get; set; }
            public string FuncProfileId { get; set; }

			public static async Task<StoreConfiguration> GetConfigurationAsync(Request request)
			{
				GetEmployeesServiceRequest extensionRequest = new GetEmployeesServiceRequest(request.RequestContext.GetPrincipal().UserId, QueryResultSettings.FirstRecord);
				Employee loggedInEmp = (await request.RequestContext.ExecuteAsync<GetEmployeesServiceResponse>(extensionRequest).ConfigureAwait(false)).Employees.Results[0];

				TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                StoreConfiguration config = new StoreConfiguration
                {
                    Store = request.RequestContext.GetOrgUnit().OrgUnitNumber,
                    DataAreaId = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId,
                    Channel = request.RequestContext.GetPrincipal().ChannelId,
                    TerminalId = request.RequestContext.GetTerminalId(),
                    StaffId = request.RequestContext.GetPrincipal().UserId,
                    StaffName = loggedInEmp.Name,
                    LanguageID = request.RequestContext.GetDeviceConfiguration().CultureName,
                    Currency = request.RequestContext.GetDeviceConfiguration().Currency,
                    CurrentDate = TimeZoneInfo.ConvertTime(request.RequestContext.GetNowInUtc().DateTime, TimeZoneInfo.Local, tst),
                    StoreName = request.RequestContext.GetOrgUnit().OrgUnitName,
                    FuncProfileId = request.RequestContext.GetDeviceConfiguration().ProfileId
                };
                return config;
			}

			public static async Task<StoreConfiguration> GetConfigurationAsync(RequestContext context)
			{
				GetEmployeesServiceRequest extensionRequest = new GetEmployeesServiceRequest(context.GetPrincipal().UserId, QueryResultSettings.FirstRecord);
				Employee loggedInEmp = (await context.ExecuteAsync<GetEmployeesServiceResponse>(extensionRequest).ConfigureAwait(false)).Employees.Results[0];

				TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                StoreConfiguration config = new StoreConfiguration
                {
                    Store = context.GetOrgUnit().OrgUnitNumber,
                    DataAreaId = context.GetChannelConfiguration().InventLocationDataAreaId,
                    Channel = context.GetPrincipal().ChannelId,
                    TerminalId = context.GetTerminalId(),
                    StaffId = context.GetPrincipal().UserId,
                    LanguageID = context.GetDeviceConfiguration().CultureName,
                    Currency = context.GetDeviceConfiguration().Currency,
                    CurrentDate = TimeZoneInfo.ConvertTime(context.GetNowInUtc().DateTime, TimeZoneInfo.Local, tst),
                    StoreName = context.GetOrgUnit().OrgUnitName,
                    FuncProfileId = context.GetDeviceConfiguration().ProfileId
                };
                return config;
			}
		}
    }
}
