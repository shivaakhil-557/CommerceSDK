import ko from "knockout";
import * as Dialogs from "PosApi/Create/Dialogs";
import * as Controls from "PosApi/Consume/Controls";
import { ObjectExtensions,StringExtensions } from "PosApi/TypeExtensions";
import { IBasicResponse, ICustomerSearch } from "../../../../Entities/Common";
import { GenericController } from "../../../../DataService/DataServiceRequests.g";
import { ClientEntities } from "PosApi/Entities";
import MessageHelpers from "../../../../Utilities/MessageHelpers";

type DialogResolve = (isDownloaded: ICustomerSearch) => void;
type DialogReject = (reason: any) => void;

export default class GenericCustomerSearchDialogModule extends Dialogs.ExtensionTemplatedDialogBase {
    private _resolve: DialogResolve;
    public dataListLn: Controls.IDataList<ICustomerSearch>;
    public dataListElementId: HTMLDivElement;
    public dataListLnColumns: Commerce.Extensibility.IDataListColumn<ICustomerSearch>[];
    public customerData: ko.ObservableArray<ICustomerSearch>;
    public selectedData: ko.Observable<ICustomerSearch>;
    public emailId: ko.Observable<string>;

    constructor() {
        super();
        this.customerData = ko.observableArray([]);
        this.selectedData = ko.observable(undefined);
        this.emailId = ko.observable("");
        this.customerData.subscribe((nv) => {
            this.DataListManager(nv);
        });


        this.emailId.subscribe((nv) => {
            this.LoadCustomers(nv);
        });
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
        this.dataListElementId = element.querySelector("#dataListLn") as HTMLDivElement;
        this.dataListLnColumns = [
            {
                title: "Customer Id",
                ratio: 25,
                collapseOrder: 1,
                minWidth: 50,
                computeValue: (rowData: ICustomerSearch): string => rowData.CustomerId
            },
            {
                title: "Email Id",
                ratio: 75,
                collapseOrder: 2,
                minWidth: 100,
                computeValue: (rowData: ICustomerSearch): string => rowData.EmailId
            }
        ]
        this.DataListManager(this.customerData());
    }

    private DataListManager = (dataArray: ICustomerSearch[]) => {
        if (!ObjectExtensions.isNullOrUndefined(this.dataListElementId)) {
            this.dataListElementId.innerHTML = "";
            let dataListOptions: Controls.IDataListOptions<ICustomerSearch> = {
                columns: this.dataListLnColumns,
                data: dataArray,
                interactionMode: Controls.DataListInteractionMode.SingleSelect
            };
            let correlationId: string = this.context.logger.getNewCorrelationId();
            this.dataListLn = this.context.controlFactory.create(correlationId, "DataList", dataListOptions, this.dataListElementId);
            this.dataListLn.addEventListener("SelectionChanged", (eventData: { items: ICustomerSearch[] }) => {
                this.selectionChanged(eventData.items);
            });
        }
    }

    public selectionChanged(items: ICustomerSearch[]): void {
        this.selectedData(undefined);
        if (items.length > 0) {
            this.selectedData(items[0]);
        }
    }
    public open(_emailId: string): Promise<ICustomerSearch> {
        this.emailId(_emailId);
        let promise: Promise<ICustomerSearch> = new Promise((resolve: DialogResolve, reject: DialogReject) => {
            this._resolve = resolve;
            let option: Dialogs.ITemplatedDialogOptions = {
                title: "Existing Customers",
                subTitle:"",
                button1: {
                    id: "btnCreate",
                    label: "OK",
                    isPrimary: true,
                    onClick: this.btnUpdateClickHandler.bind(this)
                },
                button2: {
                    id: "btnCancel",
                    label: "Cancel",
                    onClick: this.btnCancelClickHandler.bind(this)
                },
                onCloseX: () => this.btnCancelClickHandler()
            };

            this.openDialog(option);
        });

        return promise;
    }

    private btnUpdateClickHandler(): boolean {
        this.resolvePromise(this.selectedData());
        return true;
    }

    private btnCancelClickHandler(): boolean {
        this.resolvePromise(undefined);
        return true;
    }

    private resolvePromise(data: ICustomerSearch): void {
        if (ObjectExtensions.isFunction(this._resolve)) {
            this._resolve(data);
            this._resolve = null;
        }
    }

    private LoadCustomers = (emailId:string) => {
        let requestVal: string = JSON.stringify({
            Operation: "GETEXISTINGCUSTOMERS",
            EMAILID: emailId
        });
        let isExistingCustomerSearch: GenericController.ExecuteGenericRequestRequest<GenericController.ExecuteGenericRequestResponse> = new GenericController.ExecuteGenericRequestRequest(requestVal, "");
        this.context.runtime.executeAsync(isExistingCustomerSearch)
            .then((response: ClientEntities.ICancelableDataResult<GenericController.ExecuteGenericRequestResponse>) => {
                if (!response.canceled && !StringExtensions.isNullOrWhitespace(response.data.result.DataResult)) {
                    let resp: IBasicResponse = JSON.parse(response.data.result.DataResult) as IBasicResponse;
                    if (resp.Status == 200 && (<ICustomerSearch[]>resp.Payload).length > 0) {
                        this.customerData(resp.Payload as ICustomerSearch[]);
                    }
                }
            })
            .catch((err) => {
                console.log((err));
                MessageHelpers.ShowErrorMessage(this.context, "Error", JSON.stringify(err));
            });
    }
}
