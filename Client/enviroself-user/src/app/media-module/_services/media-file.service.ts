import { Injectable } from "@angular/core";
import { AccountBaseService } from "src/app/account-module/_services/account-base.service";
import { AppConfig } from "src/app/app.config";
import { Observable } from "rxjs";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PagedListDto } from "src/app/common-modules/shared-module/_models/pagination/paged-list";
import { MediaFileSmallDto } from "../_models/media-file-small";
import { MediaFileFilterDto } from "../_models/media-file-filter";
import { MessageDto } from "src/app/common-modules/shared-module/_models";

@Injectable()
export class MediaFileService extends AccountBaseService {

    _baseUrl: string = "";

    constructor(
        _appConfig: AppConfig,
        private http: HttpClient
    ) {
        super();

        this._baseUrl = _appConfig._cdnBaseUrl;
    }

    upload(model: FormData): Observable<any> {
        const headers = this.auth_header().headers;

        return this.http.post<any>(this._baseUrl + 'api/media/UploadFile', model, { headers: headers });
    }

    getFileListFiltered(filter: MediaFileFilterDto): Observable<PagedListDto<MediaFileSmallDto>> {
        let body = JSON.stringify(filter);
        const headers = this.auth_ct_json_header().headers;

        return this.http.post<PagedListDto<MediaFileSmallDto>>(this._baseUrl + "api/media/GetFiltered", body, { headers: headers });
    }
    
    download(fileId: number) {
        const headers = this.auth_ct_json_header().headers;
        const params = new HttpParams().set('fileId', fileId.toString());

        return this.http.get(this._baseUrl + "api/media/DownloadFile", { headers: headers, params: params, responseType: 'blob' });
    }

    delete(fileId: number): Observable<MessageDto> {
        const headers = this.auth_ct_json_header().headers;
        const params = new HttpParams().set('fileId', fileId.toString());

        return this.http.get<MessageDto>(this._baseUrl + "api/media/DeleteFile", { headers: headers, params: params });
    }
}