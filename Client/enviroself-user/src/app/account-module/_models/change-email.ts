export class ChangeEmailDto {
    newEmail: string;
    constructor(newEmail:string) {
        this.newEmail= newEmail;
    }
}