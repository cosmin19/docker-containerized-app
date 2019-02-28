import { SelectListItem } from "src/app/common-modules/shared-module/_models/select-list-item";

export class UserSmallDto {
    id: number;
    email: string;
    phone: string;
    firstname: string;
    lastname: string;
    gender?: string;
    pictureUrl?: string;

    totalFiles?: number;
    totalFilesSize?: string;

    genderList: SelectListItem[];
}