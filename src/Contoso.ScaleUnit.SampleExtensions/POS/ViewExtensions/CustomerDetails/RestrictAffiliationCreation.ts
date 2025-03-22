import {
    CustomerDetailsCustomControlBase,
    ICustomerDetailsCustomControlState,
    ICustomerDetailsCustomControlContext
} from "PosApi/Extend/Views/CustomerDetailsView";
import { StringExtensions } from "PosApi/TypeExtensions";
import ko from "knockout";

export default class RestrictAffiliationCreation extends CustomerDetailsCustomControlBase {
    private static readonly TEMPLATE_ID: string = "Columbia_Pos_Extensibility_RestrictAffiliationCreation";

    constructor(id: string, context: ICustomerDetailsCustomControlContext) {
        super(id, context);
    }

    /**
     * Binds the control to the specified element.
     * @param {HTMLElement} element The element to which the control should be bound.
     */
    public onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: RestrictAffiliationCreation.TEMPLATE_ID,
                data: this
            }
        },null);

        // To explicitly restrict affiliation Addition from POS
        // START

        let affiliationElementDiv: HTMLDivElement = document.getElementById('affiliationList').previousElementSibling as HTMLDivElement;
        let lstChildAffiliationDivElements: NodeListOf<HTMLDivElement> = affiliationElementDiv.querySelectorAll('div');
        let hyperLinkAffiliationElement: HTMLDivElement = lstChildAffiliationDivElements[lstChildAffiliationDivElements.length - 2]; // Since the hyperlink is 3rd element in the child divs
        let addBtnAffiliationElement: HTMLDivElement = lstChildAffiliationDivElements[lstChildAffiliationDivElements.length - 1]; // Since the add button is 4th element in the child divs
        hyperLinkAffiliationElement.innerHTML = StringExtensions.EMPTY;
        addBtnAffiliationElement.style.display = 'none';

        //END
    }

    /**
     * Initializes the control.
     * @param {ICustomerDetailsCustomControlState} state The initial state of the page used to initialize the control.
     */
    public init(state: ICustomerDetailsCustomControlState): void {

    }
}