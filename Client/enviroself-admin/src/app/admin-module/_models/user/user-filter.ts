import { BaseFilterDto } from "src/app/common-modules/shared-module/_models/filter/base-filter";

export class UserFilterAdminDto extends BaseFilterDto {
    email?: string;
    username?: string;
    firstname?: string;
    lastname?: string;
}