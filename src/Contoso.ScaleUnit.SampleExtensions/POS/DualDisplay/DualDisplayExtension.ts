/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import ko from "knockout";
import {
    DualDisplayCustomControlBase,
    IDualDisplayCustomControlState,
    IDualDisplayCustomControlContext,
    CartChangedData,
    CustomerChangedData,
    LogOnStatusChangedData
} from "PosApi/Extend/DualDisplay";
import { IDataList, IDataListOptions, DataListInteractionMode } from "PosApi/Consume/Controls";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities } from "PosApi/Entities";

export default class DualDisplayCustomControl extends DualDisplayCustomControlBase {
    public cartLinesDataList: IDataList<ProxyEntities.CartLine>;
    public tenderLinesDataList: IDataList<ProxyEntities.TenderLine>;
    public readonly cartTotalAmount: ko.Computed<number>;
    public readonly customerName: ko.Computed<string>;
    public readonly customerAccountNumber: ko.Computed<string>;
    public readonly isLoggedOn: ko.Computed<boolean>;
    public readonly employeeName: ko.Computed<string>;

    public isActiveCartLines: ko.Observable<boolean>;
    public isActivePaymentLines: ko.Observable<boolean>;
    public totalDiscountAmount: ko.Observable<string>;
    public cartNetAmountWithoutTax: ko.Observable<string>;
    public taxValue: ko.Observable<string>;
    public totalPaymentAmount: ko.Observable<string>;
    public amountDue: ko.Observable<string>;
    public welcomeCustomerText: ko.Observable<string>;

    //public readonly cartTotalAmountLabel: string;
    //public readonly customerNameLabel: string;
    //public readonly customerAccountNumberLabel: string;
    //public readonly employeeNameLabel: string;

    private static readonly TEMPLATE_ID: string = "Microsot_Pos_Extensibility_HND_DualDisplay";

    private _cart: ko.Observable<ProxyEntities.Cart>;
    private _customer: ko.Observable<ProxyEntities.Customer>;
    private _loggedOn: ko.Observable<boolean>;
    private _employee: ko.Observable<ProxyEntities.Employee>;

    private _cartLines: ProxyEntities.CartLine[];
    private _tenderLines: ProxyEntities.CartLine[];

    constructor(id: string, context: IDualDisplayCustomControlContext) {
        super(id, context);

        //this.cartTotalAmountLabel = this.context.resources.getString("string_5");
        //this.customerNameLabel = this.context.resources.getString("string_6");
        //this.customerAccountNumberLabel = this.context.resources.getString("string_7");
        //this.employeeNameLabel = this.context.resources.getString("string_8");

        this._cart = ko.observable(null);
        this._cartLines = [];
        this._tenderLines = [];
        this._customer = ko.observable(null);
        this._loggedOn = ko.observable(false);
        this._employee = ko.observable(null);

        this.totalDiscountAmount = ko.observable("0.00");
        this.amountDue = ko.observable("$ 0.00");
        this.taxValue = ko.observable("0.00");
        this.totalPaymentAmount = ko.observable("0.00");
        this.cartNetAmountWithoutTax = ko.observable("0.00");

        this.isActiveCartLines = ko.observable(true);
        this.isActivePaymentLines = ko.observable(false);
        this.welcomeCustomerText = ko.observable(context.resources.getString("DualDisplay_WelcomeText"));

        this.cartTotalAmount = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._cart()) ? 0.00 : this._cart().TotalAmount;
        });

        this.customerName = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._customer()) ? StringExtensions.EMPTY : this._customer().Name;
        });

        this.customerAccountNumber = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._customer()) ? StringExtensions.EMPTY : this._customer().AccountNumber;
        });

        this.isLoggedOn = ko.computed(() => {
            return this._loggedOn();
        });

        this.employeeName = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._employee()) ? StringExtensions.EMPTY : this._employee().Name;
        });

        this.cartChangedHandler = (data: CartChangedData) => {
            this._cart(data.cart);
            this.ManageCartLines(data.cart);
            this.ManageTenderLines(data.cart);
        };

        this.customerChangedHandler = (data: CustomerChangedData) => {
            this._customer(data.customer);
            this.welcomeCustomerText(`${context.resources.getString("DualDisplay_WelcomeText")} ${data.customer.Name}`);
        };

        this.logOnStatusChangedHandler = (data: LogOnStatusChangedData) => {
            this.isProcessing = true;
            window.setTimeout(() => {
                this.isProcessing = false;
            }, 1000);

            this._loggedOn(data.loggedOn);
            this._employee(data.employee);
        };

        //let transactionGridDiv: HTMLDivElement = document.querySelector("#TransactionGrid") as HTMLDivElement;
        //let childTDiv: HTMLDivElement = transactionGridDiv.children.item(0) as HTMLDivElement;
        //let pivotHeaderDiv: HTMLDivElement = childTDiv.children.item(1) as HTMLDivElement;
        //let pivotHeaderItemsDiv: HTMLDivElement = pivotHeaderDiv.children.item(1) as HTMLDivElement;
        //let pivotShowNavBtnsDiv: HTMLDivElement = pivotHeaderItemsDiv.children.item(0) as HTMLDivElement;
        //let paymentBtn: HTMLButtonElement = pivotShowNavBtnsDiv.children.item(1) as HTMLButtonElement;
        //paymentBtn.addEventListener("click", () => {
        //    this.showPaymentLines();
        //});

        this.context.logger.logInformational("DualDisplayCustomControl constructed", this.context.logger.getNewCorrelationId());
    }

    public onReady(element: HTMLElement): void {
        let cartLinesDataListOptions: IDataListOptions<ProxyEntities.CartLine> = {
            interactionMode: DataListInteractionMode.None,
            data: this._cartLines,
            columns: [
                {
                    title: this.context.resources.getString("DualDisplay_ItemId"),
                    ratio: 65,
                    collapseOrder: 1,
                    minWidth: 75,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Description) ? StringExtensions.EMPTY : `${cartLine.Description}\nDiscount : ${cartLine.DiscountAmount}`;
                    }
                },
                {
                    title: this.context.resources.getString("DualDisplay_Quantity"),
                    ratio: 10,
                    collapseOrder: 2,
                    minWidth: 10,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Quantity) ? StringExtensions.EMPTY : `${cartLine.Quantity.toString()} ${cartLine.UnitOfMeasureSymbol}`;
                    }
                },
                {
                    title: this.context.resources.getString("DualDisplay_NetAmountWithoutTax"),
                    ratio: 25,
                    collapseOrder: 3,
                    minWidth: 15,
                    isRightAligned: true,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.NetAmountWithoutTax) ? StringExtensions.EMPTY : cartLine.NetAmountWithoutTax.toFixed(2);
                    }
                }
            ]
        };

        let tenderLinesDataListOptions: IDataListOptions<ProxyEntities.TenderLine> = {
            interactionMode: DataListInteractionMode.None,
            data: this._tenderLines,
            columns: [
                {
                    title: this.context.resources.getString("DualDisplay_PaymentMethod"),
                    ratio: 65,
                    collapseOrder: 1,
                    minWidth: 75,
                    computeValue: (tenderLine: ProxyEntities.TenderLine): string => {
                        return ObjectExtensions.isNullOrUndefined(tenderLine.TenderTypeId) ? StringExtensions.EMPTY : tenderLine.TenderTypeId;
                    }
                },
                {
                    title: this.context.resources.getString("DualDisplay_Currency"),
                    ratio: 10,
                    collapseOrder: 2,
                    minWidth: 10,
                    computeValue: (tenderLine: ProxyEntities.TenderLine): string => {
                        return ObjectExtensions.isNullOrUndefined(tenderLine.Currency) ? StringExtensions.EMPTY : tenderLine.Currency;
                    }
                },
                {
                    title: this.context.resources.getString("DualDisplay_Amount"),
                    ratio: 25,
                    collapseOrder: 3,
                    minWidth: 15,
                    isRightAligned: true,
                    computeValue: (tenderLine: ProxyEntities.TenderLine): string => {
                        return ObjectExtensions.isNullOrUndefined(tenderLine.Amount) ? StringExtensions.EMPTY : tenderLine.Amount.toFixed(2);
                    }
                }
            ]
        };

        ko.applyBindingsToNode(element, {
            template: {
                name: DualDisplayCustomControl.TEMPLATE_ID,
                data: this
            }
        }, null);

        // Create CartLines DataList
        let dualDisplayCartLineRootElem: HTMLDivElement = element.querySelector("#dualDisplayCartLinesDataList") as HTMLDivElement;
        this.cartLinesDataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", cartLinesDataListOptions, dualDisplayCartLineRootElem);

        // Create Payments DataList
        let dualDisplaytenderLineRootElem: HTMLDivElement = element.querySelector("#dualDisplayPaymentLinesDataList") as HTMLDivElement;
        this.tenderLinesDataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", tenderLinesDataListOptions, dualDisplaytenderLineRootElem);
    }

    public init(state: IDualDisplayCustomControlState): void {
        this._cart(state.cart);
        this._customer(state.customer);
        this._loggedOn(state.loggedOn);
        this._employee(state.employee);
        this._cartLines = ObjectExtensions.isNullOrUndefined(this._cart()) ? [] : this._cart().CartLines;
        this._tenderLines = ObjectExtensions.isNullOrUndefined(this._cart()) ? [] : this._cart().TenderLines;
    }

    public showCartLines = () => {
        this.isActiveCartLines(true);
        this.isActivePaymentLines(false);
    }

    public showPaymentLines = () => {
        this.isActivePaymentLines(true);
        this.isActiveCartLines(false);
    }

    public ManageCartLines = (cart: ProxyEntities.Cart) => {
        if (ObjectExtensions.isNullOrUndefined(cart)) {
            this.cartLinesDataList.data = [];
            return;
        }
        let filteredCartLines: ProxyEntities.CartLine[] = cart.CartLines.filter(f => !f.IsVoided);
        this.cartLinesDataList.data = filteredCartLines;

        if (this.cartLinesDataList.data.length > 0) {
            this.isProcessing = true;
            this._cartLines = filteredCartLines;
            this._tenderLines = cart.TenderLines;
            this.totalDiscountAmount(cart.DiscountAmount.toFixed(2));
            this.amountDue(`$ ${cart.AmountDue.toFixed(2)}`);
            this.taxValue(cart.TaxAmount.toFixed(2));
            this.totalPaymentAmount(cart.AmountPaid.toFixed(2));
            this.cartNetAmountWithoutTax(cart.SubtotalAmountWithoutTax.toFixed(2));
            this.isProcessing = false;
        }
    }

    public ManageTenderLines = (cart: ProxyEntities.Cart) => {
        if (ObjectExtensions.isNullOrUndefined(cart)) {
            this.tenderLinesDataList.data = [];
            return;
        }
        let filteredTenderLines: ProxyEntities.TenderLine[] = cart.TenderLines.filter(f => f.VoidStatusValue == ProxyEntities.TenderLineVoidStatus.None);
        this.tenderLinesDataList.data = filteredTenderLines;

        if (this.tenderLinesDataList.data.length > 0) {
            this._tenderLines = filteredTenderLines;
        }
    }
}
