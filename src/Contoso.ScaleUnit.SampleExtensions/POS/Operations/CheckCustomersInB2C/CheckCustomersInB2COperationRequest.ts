/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import CheckCustomersInB2COperationResponse from "./CheckCustomersInB2COperationResponse";

/**
 * (Sample) Operation request for CheckCustomersInB2Cing a message to console.
 */
export default class CheckCustomersInB2COperationRequest<TResponse extends CheckCustomersInB2COperationResponse> extends ExtensionOperationRequestBase<TResponse> {
    public messageToCheckCustomersInB2C: string;
    public actionParameters: string[];

    constructor(correlationId: string) {
        super(5057, correlationId);
    }
}