import { SelectListItem } from "src/app/common-modules/shared-module/_models/select-list-item";

export class UserAdminDto {
    id: number;

    role?: number;
    email: string;
    emailConfirmed: boolean;

    phoneNumber?: string;
    firstname?: string;
    lastname?: string;
    gender?: string;

    accessFailedCount: number;
    lockoutEnabled: boolean;
    lockoutEnd?: string;

    facebookId?: number;
    pictureUrl?: string;

    createdOnUtc: string;

    genderList: SelectListItem[];
    roleList: SelectListItem[];
}