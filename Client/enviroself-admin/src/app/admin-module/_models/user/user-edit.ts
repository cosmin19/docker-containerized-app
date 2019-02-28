export class UserEditAdminDto {
    id: number;

    role: number;

    email: string;
    editEmail: boolean;
    emailConfirmed: boolean;

    phoneNumber?: string;
    firstname?: string;
    lastname?: string;
    gender?: string;

    lockoutEnabled: boolean;
}