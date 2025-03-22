/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ShowMessageDialogClientRequest, ShowMessageDialogClientResponse, IMessageDialogOptions } from "PosApi/Consume/Dialogs";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { StringExtensions } from "PosApi/TypeExtensions";
import { GenericController } from "../DataService/DataServiceRequests.g";
import { IBasicResponse } from "../Entities/Common";
import ConfirmDialogModule from "../Controls/Dialogs/Display/ConfirmDialogModule";
import MessageHelpers from "./MessageHelpers";

/**
 * A class containing helper functions for error handling.
 */
export class ErrorHelper {
    /**
     * Displays an error message corresponding to the provided reason.
     * @param {IExtensionContext} context The extension context.
     * @param {any} reason The error/reason to display.
     * @returns {Promise<ClientEntities.ICancelable>} The promise representing the completion of displaying the error message.
     */
    public static displayErrorAsync(context: IExtensionContext, reason: any): Promise<ClientEntities.ICancelable> {
        let messageDialogOptions: IMessageDialogOptions;
        if (reason instanceof ClientEntities.ExtensionError) {
            messageDialogOptions = {
                message: reason.localizedMessage
            };
        } else if (reason instanceof Error) {
            messageDialogOptions = {
                message: reason.message
            };
        } else if (typeof reason === "string") {
            messageDialogOptions = {
                message: reason
            };
        } else {
            messageDialogOptions = {
                message: "An unexpected error occurred."
            };
        }

        let errorMessageRequest: ShowMessageDialogClientRequest<ShowMessageDialogClientResponse>
            = new ShowMessageDialogClientRequest(messageDialogOptions);

        return context.runtime.executeAsync(errorMessageRequest);
    }
}

export default class CommonHelpers {

    public static CheckExistingCustomerBeforeSave = async (context: any, customer: ProxyEntities.Customer): Promise<void> => {
        if (!StringExtensions.isNullOrWhitespace(customer.ReceiptEmail) && !StringExtensions.isNullOrWhitespace(customer.Email)) {
            let requestVal: string = JSON.stringify({
                Operation: "ISEXISTINGCUSTOMERS",
                EMAILID: customer.Email
            });
            let isExistingCustomerSearch: GenericController.ExecuteGenericRequestRequest<GenericController.ExecuteGenericRequestResponse> = new GenericController.ExecuteGenericRequestRequest(requestVal, "");
            context.runtime.executeAsync(isExistingCustomerSearch)
                .then((response: ClientEntities.ICancelableDataResult<GenericController.ExecuteGenericRequestResponse>) => {
                    if (!response.canceled && !StringExtensions.isNullOrWhitespace(response.data.result.DataResult)) {
                        let resp: IBasicResponse = JSON.parse(response.data.result.DataResult) as IBasicResponse;
                        if (resp.Status == 200 && (<boolean>resp.Payload)) {
                            let message: string = 'Primary email specified already exists\n.Do you want to view all details against this record ?';
                            let confirmDialog: ConfirmDialogModule = new ConfirmDialogModule();
                            confirmDialog.open(message)
                                .then((selectedResult) => {
                                    if (selectedResult.selectedValue) {
                                        let searchNavigationOptions: ClientEntities.SearchNavigationParameters = new ClientEntities.SearchNavigationParameters(
                                            ClientEntities.SearchViewSearchEntity.Customer,
                                            customer.Email
                                        );
                                        context.navigator.navigateToPOSView("SearchView", searchNavigationOptions);
                                    }
                                });
                        }
                    }
                })
                .catch((err) => {
                    console.log((err));
                    MessageHelpers.ShowErrorMessage(context, "Error", JSON.stringify(err));
                });

        }
    }

}
