export class ResetPasswordDto {
    id: number;
    token: string;
    newPassword: string;

    constructor(id: number, token: string, newPassword: string) {
        this.id = id;
        this.token = token;
        this.newPassword = newPassword
    }   
}