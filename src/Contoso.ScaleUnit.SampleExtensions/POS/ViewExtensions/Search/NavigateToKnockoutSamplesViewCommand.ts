﻿/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { Icons } from "PosApi/Create/Views";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";

export default class NavigateToKnockoutSamplesViewCommand extends SearchView.ProductSearchExtensionCommandBase {
    /**
     * Creates a new instance of the NavigateToExampleViewCommand class.
     * @param {IExtensionCommandContext<ProductDetailsView.IProductSearchToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<SearchView.IProductSearchToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "navigateToKnockoutSamplesViewCommand";
        this.label = "Navigate to Full System Knockout Sample View";
        this.extraClass = Icons.CloudUpload;
    }

    /**
     * Initializes the command.
     * @param {ProductDetailsView.IProductDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: SearchView.IProductSearchExtensionCommandState): void {
        this.canExecute = true;
        this.isVisible = true;
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        this.context.navigator.navigate("KnockoutSamplesView");
    }
}