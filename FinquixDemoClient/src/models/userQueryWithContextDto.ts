import { UserQuery } from "./userQuery";

export interface UserQueryWithContextDto extends UserQuery {
    currentStockData: any[];
    currentCryptoData: any[];
}