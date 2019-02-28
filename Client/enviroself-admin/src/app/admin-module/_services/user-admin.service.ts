import { Injectable } from "@angular/core";
import { AccountBaseService } from "src/app/account-module/_services/account-base.service";
import { AppConfig } from "src/app/app.config";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PagedListDto } from "src/app/common-modules/shared-module/_models/pagination/paged-list";
import { UserSmallAdminDto } from "../_models/user/user-small";
import { Observable } from "rxjs";
import { UserFilterAdminDto } from "../_models/user/user-filter";
import { UserAdminDto } from "../_models/user/user";
import { MessageDto } from "src/app/common-modules/shared-module/_models";
import { UserEditAdminDto } from "../_models/user/user-edit";

@Injectable()
export class UserAdminService extends AccountBaseService{
    private _baseUrl: string = '';

    constructor(
        private http: HttpClient,
        _appConfig: AppConfig
    ) {
        super();
        this._baseUrl = _appConfig.baseUrl;
    }

    getUserListFiltered(filter: UserFilterAdminDto): Observable<PagedListDto<UserSmallAdminDto>> {
        let body = JSON.stringify(filter);
        const headers = this.auth_ct_json_header().headers;

        return this.http.post<PagedListDto<UserSmallAdminDto>>(this._baseUrl + "api/admin/user/GetFiltered", body, { headers: headers });
    }

    getUser(userId: number): Observable<UserAdminDto> {
        const headers = this.auth_ct_json_header().headers;
        const params = new HttpParams().set('userId', userId.toString());

        return this.http.get<UserAdminDto>(this._baseUrl + "api/admin/user/GetUser", { headers: headers, params: params });
    }

    editUser(model: UserEditAdminDto): Observable<MessageDto> {
        let body = JSON.stringify(model);
        const headers = this.auth_ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + "api/admin/user/EditUser", body, { headers: headers });
    }
}