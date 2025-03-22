/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
import CheckCustomersInB2COperationResponse from "./CheckCustomersInB2COperationResponse";
import CheckCustomersInB2COperationRequest from "./CheckCustomersInB2COperationRequest";
import { ClientEntities } from "PosApi/Entities";
import SearchCustomersInB2CDialogModule from "../../Controls/Dialogs/Create/SearchCustomersInB2C/SearchCustomersInB2CDialogModule";
import { ShowMessageDialogClientRequest } from "PosApi/Consume/Dialogs";

/**
 * (Sample) Request handler for the CheckCustomersInB2COperationRequest class.
 */
export default class CheckCustomersInB2COperationRequestHandler extends ExtensionOperationRequestHandlerBase<CheckCustomersInB2COperationResponse> {
    /**
     * Gets the supported request type.
     * @return {RequestType<TResponse>} The supported request type.
     */
    public supportedRequestType(): ExtensionOperationRequestType<CheckCustomersInB2COperationResponse> {
        return CheckCustomersInB2COperationRequest;
    }

    /**
     * Executes the request handler asynchronously.
     * @param {CheckCustomersInB2COperationRequest<TResponse>} request The request.
     * @return {Promise<ICancelableDataResult<TResponse>>} The cancelable async result containing the response.
     */
    public executeAsync(request: CheckCustomersInB2COperationRequest<CheckCustomersInB2COperationResponse>): Promise<ClientEntities.ICancelableDataResult<CheckCustomersInB2COperationResponse>> {

        this.context.logger.logInformational("Log message from CheckCustomersInB2COperationRequestHandler executeAsync().", this.context.logger.getNewCorrelationId());
        let messageToCheckCustomersInB2C: string = "Message from CheckCustomersInB2COperationRequestHandler: " + request.messageToCheckCustomersInB2C;

        let response: CheckCustomersInB2COperationResponse = new CheckCustomersInB2COperationResponse(messageToCheckCustomersInB2C);
        return new Promise((resolve: (value?: ClientEntities.ICancelableDataResult<CheckCustomersInB2COperationResponse>) => void) => {

            // Simulating delay so that busy indicator is shown until timeout.
            setTimeout(resolve, 2000 /*milliseconds*/);
        }).then((): ClientEntities.ICancelableDataResult<CheckCustomersInB2COperationResponse> => {

            // CheckCustomersInB2Cing the message to console.
            console.log(messageToCheckCustomersInB2C);
            this.ExecuteSearchCustomerDialog();
            return <ClientEntities.ICancelableDataResult<CheckCustomersInB2COperationResponse>>{
                canceled: false,
                data: response
            };
        });
    }

    private ExecuteSearchCustomerDialog = () => {
        let dialog: SearchCustomersInB2CDialogModule = new SearchCustomersInB2CDialogModule();
        dialog.open().then((result) => {
            if (!result) {
                this.context.runtime.executeAsync(new ShowMessageDialogClientRequest(<ClientEntities.Dialogs.IMessageDialogOptions>{
                    showCloseX: true,
                    message: "Something went wrong",
                    button1: <ClientEntities.Dialogs.IDialogResultButton>{
                        id: "OK",
                        label: "OK",
                        isPrimary: true
                    },
                    title: "Error"
                }));
            }
        });
    }
}