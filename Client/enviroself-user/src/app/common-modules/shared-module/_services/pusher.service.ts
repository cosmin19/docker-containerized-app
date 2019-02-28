import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppConfig } from 'src/app/app.config';
import { AccountBaseService } from 'src/app/account-module/_services/account-base.service';
import { MessageDto } from '../_models';
import { Observable } from 'rxjs';
import { NewLikeDto } from '../_components/pusher-test/pusher-test.component';
import { PusherConstants } from '../_constants/pusher-constants';

declare const Pusher: any;

@Injectable()
export class PusherService extends AccountBaseService {

    pusher: any;
    channel: any;

    _baseUrl: string = "";

    constructor(
        private http: HttpClient,
        _appConfig: AppConfig
    ) {
        super();
        // Set server url
        this._baseUrl = _appConfig.baseUrl;

        // Configure pusher
        this.pusher = new Pusher(_appConfig.pusherKey, {
            cluster: _appConfig.pusherCluster,
            encrypted: true
        });

        // Set channel var
        this.channel = this.pusher.subscribe(PusherConstants.CHANNEL_NAME);
    }

    like(model: NewLikeDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.auth_ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + "api/pusher/Like", bodyData, { headers: headers });
    }

    userOnline(): Observable<any> {
        const headers = this.auth_ct_json_header().headers;

        return this.http.get(this._baseUrl + "api/pusher/UserOnline", { headers: headers });
    }

    userOffline(): Observable<any> {
        const headers = this.auth_ct_json_header().headers;

        return this.http.get(this._baseUrl + "api/pusher/UserOffline", { headers: headers });
    }
}