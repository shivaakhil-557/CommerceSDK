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
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import GenericCustomerSearchDialogModule from "../../Controls/Dialogs/Create/GenericCustomerSearch/GenericCustomerSearchDialogModule";

export default class GetExistingCustomerCommandV2 extends CustomerAddEditView.CustomerAddEditExtensionCommandBase {
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

        this.id = "getExistingCustomerCommandV2";
        this.label = "Search Customer V2";
        this.extraClass = "iconFilter"

        this.customerUpdatedHandler = (data: CustomerAddEditView.CustomerAddEditCustomerUpdatedData) => {
            this.currentCustomer = ko.observable(data.customer);
            this.receiptEmail(data.customer.Email ?? data.customer.ReceiptEmail);
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
        if (!StringExtensions.isNullOrWhitespace(customer.ReceiptEmail) && !StringExtensions.isNullOrWhitespace(customer.Email)) {
            let customerDialog: GenericCustomerSearchDialogModule = new GenericCustomerSearchDialogModule();
            customerDialog.open(customer.Email)
                .then((selectedValue) => {
                    if (!ObjectExtensions.isNullOrUndefined(selectedValue)) {
                        let searchNavigationOptions: ClientEntities.SearchNavigationParameters = new ClientEntities.SearchNavigationParameters(
                            ClientEntities.SearchViewSearchEntity.Customer,
                            selectedValue.EmailId
                        );
                        this.context.navigator.navigateToPOSView("SearchView", searchNavigationOptions);
                    }
                });
        }
    }
}
