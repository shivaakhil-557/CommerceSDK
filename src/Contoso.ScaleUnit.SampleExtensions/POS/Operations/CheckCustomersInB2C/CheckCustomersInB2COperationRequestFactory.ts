/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import CheckCustomersInB2COperationResponse from "./CheckCustomersInB2COperationResponse";
import CheckCustomersInB2COperationRequest from "./CheckCustomersInB2COperationRequest";
import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<CheckCustomersInB2COperationResponse> =
    /**
     * Gets an instance of CheckCustomersInB2COperationRequest.
     * @param {number} operationId The operation Id.
     * @param {string[]} actionParameters The action parameters.
     * @param {string} correlationId A telemetry correlation ID, used to group events logged from this request together with the calling context.
     * @return {CheckCustomersInB2COperationRequest<TResponse>} Instance of CheckCustomersInB2COperationRequest.
     */
    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<CheckCustomersInB2COperationRequest<CheckCustomersInB2COperationResponse>>> {

    let operationRequest: CheckCustomersInB2COperationRequest<CheckCustomersInB2COperationResponse> = new CheckCustomersInB2COperationRequest<CheckCustomersInB2COperationResponse>(correlationId);
        operationRequest.messageToCheckCustomersInB2C = "Current Operation id: " + operationId + ", Action parameters: " + JSON.stringify(actionParameters);
        operationRequest.actionParameters = actionParameters;
    return Promise.resolve(<ClientEntities.ICancelableDataResult<CheckCustomersInB2COperationRequest<CheckCustomersInB2COperationResponse>>>{
        canceled: false,
        data: operationRequest
    });
};

export default getOperationRequest;