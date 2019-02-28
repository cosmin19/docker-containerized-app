import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { AppConfig } from 'src/app/app.config';
import { MessageDto } from 'src/app/common-modules/shared-module/_models';
import { UserSmallDto } from '../_models/user/user-small';
import { AccountBaseService } from 'src/app/account-module/_services/account-base.service';
import { UserEditDto } from '../_models/user/user-edit';


@Injectable()
export class UserService extends AccountBaseService {

    _baseUrl: string = "";

    constructor(
        _appConfig: AppConfig,
        private http: HttpClient
    ) {
        super();

        this._baseUrl = _appConfig.baseUrl;
    }

    getUser(): Observable<UserSmallDto> {
        const headers = this.auth_ct_json_header().headers;

        return this.http.get<UserSmallDto>(this._baseUrl + 'api/user/GetUser', { headers: headers });
    }

    editUser(model: UserEditDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.auth_ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + 'api/user/EditUser', bodyData, { headers: headers });
    }
}