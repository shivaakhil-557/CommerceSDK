import ko from "knockout";
import * as Dialogs from "PosApi/Create/Dialogs";
import { IConfirmDialogResult } from "./IConfirmDialogResult";
import { ObjectExtensions } from "PosApi/TypeExtensions";

type MessageDialogResolve = (value: IConfirmDialogResult) => void;
type MessageDialogReject = (reason: any) => void;

export default class ConfirmDialogModule extends Dialogs.ExtensionTemplatedDialogBase {
    private resolve: MessageDialogResolve;
    public Message: ko.Observable<string>;
    constructor() {
        super();
        this.Message = ko.observable("");
    }

    public onReady(el: HTMLElement): void {
        ko.applyBindings(this, el);
    }

    public open(message: string): Promise<IConfirmDialogResult> {
        this.Message(message);
        let promise: Promise<IConfirmDialogResult> = new Promise((resolve: MessageDialogResolve, reject: MessageDialogReject) => {
            this.resolve = resolve;
            let option: Dialogs.ITemplatedDialogOptions = {
                title: "Confirmation !",
                subTitle: "",
                onCloseX: this.onCloseX.bind(this),
                button1: {
                    id: "Button1",
                    label: "YES",
                    isPrimary: true,
                    onClick: this.button1ClickHandler.bind(this)
                },
                button2: {
                    id: "Button2",
                    label: "NO",
                    onClick: this.button2ClickHandler.bind(this)
                }
            };

            this.openDialog(option);
        });


        return promise;
    }

    private onCloseX(): boolean {
        this.resolvePromise(false);
        return true;
    }

    private button1ClickHandler(): boolean {
        this.resolvePromise(true);
        return true;
    }

    private button2ClickHandler(): boolean {
        this.resolvePromise(false);
        return true;
    }

    private resolvePromise(result: boolean): void {
        if (ObjectExtensions.isFunction(this.resolve)) {
            this.resolve(<IConfirmDialogResult>{
                selectedValue: result
            });
        }
    }
}