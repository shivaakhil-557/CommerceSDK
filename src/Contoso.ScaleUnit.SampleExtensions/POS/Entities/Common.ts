import { ProxyEntities } from "PosApi/Entities";

export interface IBasicResponse {
    Status: number;
    Message: string;
    Payload: any;
    Remarks: string;
    ErrorDetails: string;
}

export interface ICustomerSearchResult {
    Status: number;
    SearchText: string;
}

export interface ICustomerSearch  {
    CustomerId: string;
    EmailId: string;
    CustomerName: string;
}

export default 
    class GenericStaticClass {
    public static CustomerDetails: ProxyEntities.Customer = undefined;
    public static CustomerFirstName: string = '';
}
