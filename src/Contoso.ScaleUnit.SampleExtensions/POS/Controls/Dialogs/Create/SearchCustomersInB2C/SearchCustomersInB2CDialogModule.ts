import ko from "knockout";
import * as Dialogs from "PosApi/Create/Dialogs";
import { GenericController } from "../../../../DataService/DataServiceRequests.g";
import { ObjectExtensions,StringExtensions } from "PosApi/TypeExtensions";
import FileHelpers from "../../../../Utilities/FileHelper";

type DialogResolve = (isDownloaded: boolean) => void;
type DialogReject = (reason: any) => void;

export default class SearchCustomersInB2CDialogModule extends Dialogs.ExtensionTemplatedDialogBase {
    private _resolve: DialogResolve;
    public isFileDownloaded: ko.Observable<boolean>;
    public waitingLabelTxt: ko.Observable<string>;
    constructor() {
        super();
        this.waitingLabelTxt = ko.observable("Loading .");
        this.isFileDownloaded = ko.observable(false);
        this.LoadCustomers();
        setInterval(() => {
            if (!this.isFileDownloaded() && this.waitingLabelTxt().substr(-1, 10).length < 15) {
                let wtTxt: string = this.waitingLabelTxt();
                this.waitingLabelTxt(wtTxt + ".");
            }
        },500);
        
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this,element);
    }

    public open(): Promise<boolean> {
        let promise: Promise<boolean> = new Promise((resolve: DialogResolve, reject: DialogReject) => {
            this._resolve = resolve;
            let option: Dialogs.ITemplatedDialogOptions = {
                title: "Search customers in Azure B2C tenant",
                subTitle:"To search for customers who are present in back office and in Azure B2C for eCommerce",
                button1: {
                    id: "btnCreate",
                    label: this.context.resources.getString("string_2001"),
                    isPrimary: true,
                    onClick: this.btnUpdateClickHandler.bind(this)
                },
                button2: {
                    id: "btnCancel",
                    label: this.context.resources.getString("string_2004"),
                    onClick: this.btnCancelClickHandler.bind(this)
                },
                onCloseX: () => this.btnCancelClickHandler()
            };

            this.openDialog(option);
        });

        return promise;
    }

    private btnUpdateClickHandler(): boolean {
        this.resolvePromise(this.isFileDownloaded());
        return true;
    }

    private btnCancelClickHandler(): boolean {
        this.resolvePromise(this.isFileDownloaded());
        return true;
    }

    private resolvePromise(isDownloaded: boolean): void {
        if (ObjectExtensions.isFunction(this._resolve)) {
            this._resolve(isDownloaded);
            this._resolve = null;
        }
    }

    private LoadCustomers = () => {
        this.context.runtime.executeAsync(new GenericController.GetExistingB2C_CustomersRequest("", ""))
            .then((response) => {
                if (!response.canceled && !StringExtensions.isNullOrWhitespace(response.data.result.DataResult) && response.data.result.DataResult) {
                    let fileName: string = `Customer_Search_B2C ${new Date().toDateString()}`;
                    FileHelpers.convertBase64ToExcel(response.data.result.DataResult, fileName, "excelFile");
                    this.isFileDownloaded(true);
                }
                else {

                }
            })
            .catch((err) => {
                console.log(JSON.stringify(err));
            })
    }
}
