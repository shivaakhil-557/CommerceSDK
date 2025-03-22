/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */
import ko from "knockout";
import { ICustomViewControllerContext } from "PosApi/Create/Views";
import { Entities } from "../DataService/DataServiceEntities.g";
import * as Messages from "../DataService/DataServiceRequests.g";
import ExampleCreateDialog from "../Controls/Dialogs/Create/Example/ExampleCreateDialogModule";
import ExampleEditDialog from "../Controls/Dialogs/Edit/ExampleEditDialogModule";
import PingResultDialog from "../Controls/Dialogs/Display/PingResultDialogModule";
import { ObjectExtensions, ArrayExtensions } from "PosApi/TypeExtensions";
import { ClientEntities } from "PosApi/Entities";
import { ShowMessageDialogClientRequest } from "PosApi/Consume/Dialogs";

/**
 * The ViewModel for ExampleView.
 */
export default class ExampleViewModel {
    public title: ko.Observable<string>;
    public loadedData: Entities.ExampleEntity[];
    public isItemSelected: () => boolean;
    private _selectedItem: Entities.ExampleEntity;
    private _context: ICustomViewControllerContext;

    constructor(context: ICustomViewControllerContext) {
        this._context = context;
        this.title = ko.observable(context.resources.getString("string_0001"));
        this.loadedData = [];
        this.isItemSelected = () => !ObjectExtensions.isNullOrUndefined(this._selectedItem);
    }

    public load(): Promise<void> {
        return this._context.runtime
            .executeAsync(new Messages.BoundControllerV2.GetAllExampleEntitiesRequest())
            .then(response => {
                if (!response.canceled) {
                    this.loadedData = response.data.result;
                }
            });
    }

    /**
     * Handler for list item selection.
     * @param {Entities.ExampleEntity[]} items
     */
    public seletionChanged(items: Entities.ExampleEntity[]): Promise<void> {
        this._context.logger.logInformational("Item selected:" + JSON.stringify(items));
        this._selectedItem = ArrayExtensions.firstOrUndefined(items);
        return Promise.resolve();
    }

    public createExampleEntity = async(): Promise<boolean> => {
        let dialog: ExampleCreateDialog = new ExampleCreateDialog();
        const result = await dialog.open();
        if (!ObjectExtensions.isNullOrUndefined(result)) {
            try {
                const ExampleEntityCreateResponse: ClientEntities.ICancelableDataResult<Messages.BoundControllerV2.CreateExampleEntityResponse> = await this._context.runtime.executeAsync(new Messages.BoundControllerV2.CreateExampleEntityRequest(result));
                if (!ExampleEntityCreateResponse.canceled && !ObjectExtensions.isNullOrUndefined(ExampleEntityCreateResponse.data.result)) {
                    let messageOptions: ClientEntities.Dialogs.IMessageDialogOptions = {
                        title: "Success",
                        message: ExampleEntityCreateResponse.data.result.toString(),
                        showCloseX: true,
                        button1: {
                            id: "OK",
                            label: "OK",
                            result: "success",
                            isPrimary: true
                        },
                        subTitle: "Example entity created succesfully",
                    }
                    await this._context.runtime.executeAsync(new ShowMessageDialogClientRequest(messageOptions));
                    return Promise.resolve(true);
                }
                else {
                    let messageOptions: ClientEntities.Dialogs.IMessageDialogOptions = {
                        title: "Error",
                        message: ExampleEntityCreateResponse.data.result.toString(),
                        showCloseX: true,
                        button1: {
                            id: "OK",
                            label: "OK",
                            result: "error",
                            isPrimary: true
                        },
                        subTitle: "Example entity creation failed",
                    }
                    await this._context.runtime.executeAsync(new ShowMessageDialogClientRequest(messageOptions));
                    return Promise.resolve(false);
                }

            } catch (e) {
                let messageOptions: ClientEntities.Dialogs.IMessageDialogOptions = {
                    title: "Error",
                    message: JSON.stringify(e),
                    showCloseX: true,
                    button1: {
                        id: "OK",
                        label: "OK",
                        result: "error",
                        isPrimary: true
                    },
                    subTitle: "Example entity creation failed",
                }
                await this._context.runtime.executeAsync(new ShowMessageDialogClientRequest(messageOptions));
                return Promise.resolve(false);
            }
        }
        else {
            return Promise.resolve(false);
        }
    }

    public editExampleEntity(): Promise<boolean> {
        let dialog: ExampleEditDialog = new ExampleEditDialog();
        return dialog
            .open(this._selectedItem)
            .then(updatedItem => {
                // No action if the dialog was canceled
                if (ObjectExtensions.isNullOrUndefined(updatedItem)) {
                    this._context.logger.logInformational("Update canceled for data: " + JSON.stringify(updatedItem));
                    return Promise.resolve(false);
                }

                this._context.logger.logInformational("Updated data is: " + JSON.stringify(updatedItem));
                // Perform the update and reload the loaded data to reflect the change:
                return this._context.runtime
                    .executeAsync(new Messages.BoundControllerV2.UpdateExampleEntityRequest(updatedItem.UnusualEntityId, updatedItem))
                    .then(response => {
                        if (!response.canceled && response.data.result) {
                            this._context.logger.logInformational("Update success for id: " + updatedItem.UnusualEntityId);
                            return this.load().then((): boolean => true); // Load the updated data
                        }
                        this._context.logger.logInformational("Update failed for id: " + updatedItem.UnusualEntityId);
                        return Promise.resolve(false);
                    });
            }).catch(reason => {
                this._context.logger.logError("Error occurred in the edit dialog: " + JSON.stringify(reason));
                return Promise.resolve(false);
            });
    }

    public deleteExampleEntity(): Promise<void> {
        // Delete the selected entity and reload the loaded data to reflect the change:
        return this._context.runtime
            .executeAsync(new Messages.BoundControllerV2.DeleteExampleEntityRequest(this._selectedItem.UnusualEntityId))
            .then(response => {
                if (!response.canceled && response.data.result) {
                    this._context.logger.logInformational("Delete success for id: " + this._selectedItem.UnusualEntityId);
                    return this.load(); // Load the updated data
                }
                this._context.logger.logInformational("Delete failed for id " + this._selectedItem.UnusualEntityId);
                return Promise.resolve();
            });
    }

    public runPingTest(): Promise<void> {
        return this._context.runtime
            .executeAsync(new Messages.StoreOperations.SimplePingGetRequest())
            .then(pingGetResponse => {
                return this._context.runtime
                    .executeAsync(new Messages.StoreOperations.SimplePingPostRequest())
                    .then(pingPostResponse => {
                        let pingResultDialog: PingResultDialog = new PingResultDialog();
                        return pingResultDialog.open(pingGetResponse.data.result, pingPostResponse.data.result);
                    });
            });
    }
}
