import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';


@Injectable()
export class AccountBaseService {

    jwt_token: string = "env_auth_token";
    _jwtHelper: JwtHelperService;

    constructor() {
        this._jwtHelper = new JwtHelperService ();
    }

    ct_json_header() {
        let httpOptions = {
            headers: new HttpHeaders({ 'Content-Type': 'application/json' })
        };
        return httpOptions;
    }

    auth_ct_json_header() {
        if(!this.isTokenValid()) {
            localStorage.removeItem(this.jwt_token);
        }

        let user = localStorage.getItem(this.jwt_token);
        if (user) {
            let auth_token = JSON.parse(user)['token'];
            if (auth_token) {

                let httpOptions = {
                    headers: new HttpHeaders({
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + auth_token
                    })
                };
                return httpOptions;
            }
        }
        return null;
    }

    auth_header() {
        if(!this.isTokenValid()) {
            localStorage.removeItem(this.jwt_token);
        }

        let user = localStorage.getItem(this.jwt_token);
        if (user) {
            let auth_token = JSON.parse(user)['token'];
            if (auth_token) {

                let httpOptions = {
                    headers: new HttpHeaders({
                        'Authorization': 'Bearer ' + auth_token
                    })
                };
                return httpOptions;
            }
        }
        return null;
    }

    /* ------------------------------ Get Decoded Token ------------------------------ */
    getDecodedToken() {
        let user = localStorage.getItem(this.jwt_token);
        if (user) {
            let auth_token = JSON.parse(user)['token'];
            if (auth_token) {
                return this._jwtHelper.decodeToken(auth_token);
            }
        }
        return null;
    }

    isTokenValid() : boolean {
        let user = localStorage.getItem(this.jwt_token);
        if (user) {
            let auth_token = JSON.parse(user)['token'];
            if (auth_token) {
                let result = this._jwtHelper.isTokenExpired(auth_token);

                // Daca e expirat, sterge-l
                if(result == true)
                    localStorage.removeItem(this.jwt_token);

                // If is not expired (false), return valid (true)
                return !result;
            }
        }
        return false;
    }
}
