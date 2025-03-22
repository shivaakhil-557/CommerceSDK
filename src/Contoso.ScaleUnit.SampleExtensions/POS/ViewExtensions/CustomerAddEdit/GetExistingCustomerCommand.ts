/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import ko from "knockout";
import * as CustomerAddEditView from "PosApi/Extend/Views/CustomerAddEditView";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { StringExtensions } from "PosApi/TypeExtensions";
import { GenericController } from "../../DataService/DataServiceRequests.g";
import GenericStaticClass, { IBasicResponse, ICustomerSearchResult } from "../../Entities/Common";
import MessageHelpers from "../../Utilities/MessageHelpers";
import ConfirmDialogModule from "../../Controls/Dialogs/Display/ConfirmDialogModule";
import { Icons } from "PosApi/Create/Views";

export default class GetExistingCustomerCommand extends CustomerAddEditView.CustomerAddEditExtensionCommandBase {
    public currentCustomer: ko.Observable<ProxyEntities.Customer>;
    public receiptEmail: ko.Observable<string>;
    /**
     * Creates a new instance of the GetExternalCustomerCommand class.
     * @param {IExtensionCommandContext<CustomerAddEditView.ICustomerAddEditToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<CustomerAddEditView.ICustomerAddEditToExtensionCommandMessageTypeMap>) {
        super(context);
        this.receiptEmail = ko.observable("");

        this.id = "getExistingCustomerCommand";
        this.label = "Search Customer";
        this.extraClass = Icons.Search;

        this.customerUpdatedHandler = (data: CustomerAddEditView.CustomerAddEditCustomerUpdatedData) => {
            this.currentCustomer = ko.observable(data.customer);
            this.receiptEmail(data.customer.Email);
        };
    }

    /**
     * Initializes the command.
     * @param {CustomerAddEditView.ICustomerAddEditExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: CustomerAddEditView.ICustomerAddEditExtensionCommandState): void {
        if (state.isNewCustomer) {
            // We don't want to override a customer if it already exists.
            this.isVisible = true;
            this.canExecute = true;
        }
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        let customer: ProxyEntities.Customer = this.customer;
        GenericStaticClass.CustomerDetails = this.customer;
        GenericStaticClass.CustomerFirstName = this.customer.FirstName;
        if (!(StringExtensions.isNullOrWhitespace(customer.ReceiptEmail) && StringExtensions.isNullOrWhitespace(customer.Email)) || !StringExtensions.isNullOrWhitespace(customer.Phone)) {
            let requestVal: string = JSON.stringify({
                Operation: "ISEXISTINGCUSTOMERS",
                EMAILID: customer.Email ?? '',
                PHONE: customer.Phone ?? ''
            });
            let isExistingCustomerSearch: GenericController.ExecuteGenericRequestRequest<GenericController.ExecuteGenericRequestResponse> = new GenericController.ExecuteGenericRequestRequest(requestVal, "");
            this.context.runtime.executeAsync(isExistingCustomerSearch)
                .then((response: ClientEntities.ICancelableDataResult<GenericController.ExecuteGenericRequestResponse>) => {
                    if (!response.canceled && !StringExtensions.isNullOrWhitespace(response.data.result.DataResult)) {
                        let resp: IBasicResponse = JSON.parse(response.data.result.DataResult) as IBasicResponse;
                        let result: ICustomerSearchResult = <ICustomerSearchResult>resp.Payload;
                        if (resp.Status == 200 && result.Status !== 0) {
                            let message: string = 'Primary email specified already exists\n.Do you want to view all details against this record ?';
                            let confirmDialog: ConfirmDialogModule = new ConfirmDialogModule();
                            confirmDialog.open(message)
                                .then((selectedResult) => {
                                    if (selectedResult.selectedValue) {
                                        let searchText: string = result.SearchText;
                                        let searchNavigationOptions: ClientEntities.SearchNavigationParameters = new ClientEntities.SearchNavigationParameters(
                                            ClientEntities.SearchViewSearchEntity.Customer,
                                            searchText
                                        );
                                        this.context.navigator.navigateToPOSView("SearchView", searchNavigationOptions);
                                    }
                                });
                        }
                    }
                })
                .catch((err) => {
                    console.log((err));
                    MessageHelpers.ShowErrorMessage(this.context, "Error", JSON.stringify(err));
                });

        }
    }
}
