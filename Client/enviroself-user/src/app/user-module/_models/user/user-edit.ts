export class UserEditDto {
    phone: string;
    firstname: string;
    lastname: string;
    gender: string;

    constructor(phone: string, firstname: string, lastname: string, gender: string) {
        this.phone = phone;
        this.lastname = lastname;
        this.firstname = firstname;
        this.gender = gender;
    }
}