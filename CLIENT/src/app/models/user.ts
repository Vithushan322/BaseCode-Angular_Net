export interface User {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    location: string;
    token?:string;
    isRememberUser?: boolean;
}
