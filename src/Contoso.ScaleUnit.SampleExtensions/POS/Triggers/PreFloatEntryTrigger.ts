/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import * as Triggers from "PosApi/Extend/Triggers/CashManagementTriggers";
import { ClientEntities } from "PosApi/Entities";

export default class PreFloatEntryTrigger extends Triggers.PreFloatEntryTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPreFloatEntryTriggerOptions} options The options provided to the trigger.
     * @return {Promise<ClientEntities.ICancelable>} The cancelable promise.
     */
    public execute(options: Triggers.IPreFloatEntryTriggerOptions): Promise<ClientEntities.ICancelable> {
        this.context.logger.logVerbose("Executing PreFloatEntryTrigger with options " + JSON.stringify(options) + " at " + new Date().getTime() + ".");
        return Promise.resolve({ canceled: false });
    }
}