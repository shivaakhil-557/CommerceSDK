namespace ITS
{
    namespace CommerceRuntime
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using ITS.CommerceRuntime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        [RoutePrefix("CustomAPI")]
        [BindEntity(typeof(CustomAPIEntity))]
        public class ITSCustomizedAPIController : IController
        {
            [HttpPost]
            [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Application, CommerceRoles.Storefront)]
            public async Task<Cart> CreateCart(IEndpointContext context, string _CustomerId)
            {
                try
                {
                    GetAvailableShiftsRequest getShifts = new GetAvailableShiftsRequest();
                    List<Shift> Shifts = (await context.ExecuteAsync<GetAvailableShiftsResponse>(getShifts).ConfigureAwait(false)).Shifts.ToList();
                    List<Shift> ActiveShifts = Shifts.Where(w => w.Status == ShiftStatus.Open).ToList();
                    if (ActiveShifts.Count == 0)
                    {
                        CreateShiftRequest newShiftRequest = new CreateShiftRequest();
                        Shift NewShift = (await context.ExecuteAsync<CreateShiftResponse>(newShiftRequest).ConfigureAwait(false)).Shift;
                    }
                    else
                    {
                        ResumeShiftRequest resumeShiftRequest = new ResumeShiftRequest
                        {
                            ShiftTerminalId = "HOUSTON-14"
                        };
                        await context.ExecuteAsync<ResumeShiftResponse>(resumeShiftRequest).ConfigureAwait(false);
                    }
                    GetCartRequest getCartsRequest = new GetCartRequest(new CartSearchCriteria(), QueryResultSettings.AllRecords);
                    List<Cart> carts = (await context.ExecuteAsync<GetCartResponse>(getCartsRequest).ConfigureAwait(false)).Carts.ToList();
                    if (carts.Count > 0)
                    {
                        Cart supendedCart = (await context.ExecuteAsync<SuspendCartResponse>(new SuspendCartRequest()).ConfigureAwait(false)).Cart;
                        ReasonCodeLine reasonToVoidSuspendedCart = new ReasonCodeLine
                        {
                            ReasonCodeId = "VOID",
                            Information = "Voiding a active suspended cart",
                            TransactionId = supendedCart.Id,
                        };
                        List<string> suspendedCartIds = new List<string> { supendedCart.Id };
                        IEnumerable<ReasonCodeLine> lstReasonCodeLines = new List<ReasonCodeLine> { reasonToVoidSuspendedCart };
                        await context.ExecuteAsync<VoidSuspendedCartsResponse>(new VoidSuspendedCartsRequest(suspendedCartIds, lstReasonCodeLines)).ConfigureAwait(false);
                    }
                    Cart newCart = new Cart
                    {
                        CustomerId = _CustomerId
                    };
                    CreateCartRequest createCartRequest = new CreateCartRequest(newCart);
                    Response createCartReponse = await context.ExecuteAsync<Response>(createCartRequest).ConfigureAwait(false);
                    //SaveCartRequest createNewCartRequest = new SaveCartRequest(newCart,TransactionOperationType.Create);
                    //Cart createdCart = (await context.ExecuteAsync<SaveCartResponse>(createNewCartRequest).ConfigureAwait(false)).Cart;
                    return ((SaveCartResponse)createCartReponse).Cart;
                }
                catch (Exception ex)
                {
                    
                    return new Cart();
                }

            }

            [HttpPost]
            [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Application, CommerceRoles.Storefront)]
            public async Task<GenericStringDataModel> GetExistingB2C_Customers(IEndpointContext context,string RequestVal,string ObjStrVal)
            {
                GenericStringDataModel result = new GenericStringDataModel();
                B2CUserManagementRequest request = new B2CUserManagementRequest(RequestVal, ObjStrVal);
                B2CUserManagementResponse response = await context.ExecuteAsync<B2CUserManagementResponse>(request).ConfigureAwait(false);
                result.DataResult = response.ResponseVal;
                return result;
            }
        }
    }
}