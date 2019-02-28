import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { AccountBaseService } from './account-base.service';
import { RegisterDto, ChangePasswordDto, LogInDto } from '../_models';
import { AppConfig } from 'src/app/app.config';
import { Router } from '@angular/router';
import { MessageDto } from 'src/app/common-modules/shared-module/_models';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { ForgotPasswordDto } from '../_models/forgot-password';
import { ResetPasswordDto } from '../_models/reset-password';
import { ChangeEmailDto } from '../_models/change-email';
import { ResendEmailConfirmationDto } from '../_models/resend-email-confirmation';
import { EmailConfirmationDto } from '../_models/email-confirmation';
import { AppConstants } from 'src/app/common-modules/shared-module/_constants/app-constants';
import { BehaviorSubject } from 'rxjs';


@Injectable()
export class AccountService extends AccountBaseService {

    private USER: string = AppConstants.USER;
    private ADMIN: string = AppConstants.ADMIN;

    /* ------------------------------ Variables ------------------------------ */
    _baseUrl: string = "";

    private userLoginStatusSubject = new BehaviorSubject<boolean>(false);
    loggedIn: boolean = false;
    userIsAdmin: boolean = false;

    /* ------------------------------ Ctor ------------------------------ */
    constructor(
        _appConfig: AppConfig,
        private http: HttpClient,
        private _alertService: AlertService,
        private router: Router
    ) {
        super();

        this._baseUrl = _appConfig.baseUrl;
    }

    login(user: LogInDto): Observable<any> {
        let bodyData = JSON.stringify(user);
        const headers = this.ct_json_header().headers;

        return this.http.post<any>(this._baseUrl + 'api/account/login', bodyData, { headers: headers });
    }

    facebookLogin(accessToken: string): Observable<any> {
        let bodyData = JSON.stringify({ accessToken: accessToken });

        const headers = this.ct_json_header().headers;
        return this.http.post<any>(this._baseUrl + 'api/account/FacebookAuthentication', bodyData, { headers });
    }

    register(model: RegisterDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + 'api/account/register', bodyData, { headers: headers });
    }

    changePassword(model: ChangePasswordDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.auth_ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + 'api/account/ChangePassword', bodyData, { headers: headers });
    }

    forgotPassword(model: ForgotPasswordDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + 'api/account/ForgotPassword', bodyData, { headers: headers });
    }

    resetPassword(model: ResetPasswordDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + 'api/account/ResetPassword', bodyData, { headers: headers });
    }

    changeEmail(model: ChangeEmailDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.auth_ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + 'api/account/ChangeEmail', bodyData, { headers: headers });
    }

    resendEmailConfirmation(model: ResendEmailConfirmationDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + 'api/account/ResendEmailConfirmation', bodyData, { headers: headers });
    }

    confirmEmail(model: EmailConfirmationDto): Observable<MessageDto> {
        let bodyData = JSON.stringify(model);
        const headers = this.ct_json_header().headers;

        return this.http.post<MessageDto>(this._baseUrl + 'api/account/ConfirmEmail', bodyData, { headers: headers });
    }

    logout(redirectToLogin: boolean) {
        localStorage.removeItem(this.jwt_token);
        this.loggedIn = false;
        this.userIsAdmin = false;

        this.userLoginStatusSubject.next(false);

        if(redirectToLogin)
            this.router.navigate(['/login']);

        return true;
    }

    /* ------------------------------ CHECK LOG IN CLIENT / ADMIN ------------------------------ */

    postLogIn() {
        /* If token is invalid */
        if(this.isTokenValid() == false) {
            this.logout(true);
            return;
        }

        /* Token is valid */
        this.loggedIn = true;
        this.userIsAdmin = this.getUserRole() == this.ADMIN;

        if(this.userIsAdmin == false)
            this.logout(true);

        /* Emit new subject value */
        this.userLoginStatusSubject.next(true);
    }

    checkUserStatus(): void {
        this.loggedIn = this.isTokenValid();
        if(this.loggedIn)
            this.userIsAdmin = this.getUserRole() === this.ADMIN;
        else {
            this.logout(false);
        }

        this.userLoginStatusSubject.next(this.loggedIn);
    }

    getUserLoginStatus(): BehaviorSubject<boolean> {
        return this.userLoginStatusSubject;
    }

    getUserIsAdmin(): boolean {
        return this.userIsAdmin;
    }

    /* --------------------==================== UTILS ====================-------------------- */
    getUserId(): number {
        var token = super.getDecodedToken();
        if (token && token.id)
            return token.id as number;
        return 0;
    }

    getUserEmail(): string {
        var token = super.getDecodedToken();
        if (token && token.sub)
            return token.sub;
        return '';
    }

    getUserRole(): string {
        var token = super.getDecodedToken();
        if (token && token.roles)
            return token.roles;
        return '';
    }
    getEmailConfirmed(): boolean {
        var token = super.getDecodedToken();
        if (token && token.emailConfirmed)
            try {
                return JSON.parse(token.emailConfirmed);
            }
            catch (e) {
                return false;
            }
        return false;
    }
}