import { ClientEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/ApplicationTriggers";


export default class PreLogOnTrigger extends Triggers.PreLogOnTrigger {
    /**
        * Executes the trigger functionality.
        * @param {Triggers.IPreLogOnTriggerOptions} options The options provided to the trigger.
    */
    public async execute(options: Triggers.IPreLogOnTriggerOptions): Promise<ClientEntities.ICancelable> {
        this.context.logger.logVerbose("Executing PreLogOnTriggerOptions with options " + JSON.stringify(options) + ".");
        return Promise.resolve({ canceled: false });
        
    }
}