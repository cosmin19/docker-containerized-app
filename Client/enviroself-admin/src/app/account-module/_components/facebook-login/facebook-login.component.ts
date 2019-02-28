import { Component, HostListener, OnInit, Input } from '@angular/core';

import { Router } from '@angular/router';
import { AccountService } from '../../_services';
import { FacebookRedirectionMessage } from '../../_models/facebook-redirection-message';
import { AppConfig } from 'src/app/app.config';
import { AlertService } from 'src/app/common-modules/shared-module/_services';


@Component({
    selector: 'app-facebook-login',
    templateUrl: './facebook-login.component.html'
})
export class FacebookLoginComponent {

    @Input() returnUrl:string;

    private authWindow: Window;
    private locationOrigin;

    failed: boolean;
    error: string;
    errorDescription: string;

    jwt_token: string = "";

    loading: boolean;
    facebookClientId: string;

    constructor(
        private router: Router, 
        private _accountService: AccountService,
        _alertService: AlertService,
        _appConfig: AppConfig
    ) { 
        this.facebookClientId = _appConfig.facebookClientId;
        this.jwt_token = _accountService.jwt_token;

        /* Set event listener */
        if (window.addEventListener) {
            window.addEventListener("message", this.handleMessage.bind(this), false);
        }
        else {
            (<any>window).attachEvent("onmessage", this.handleMessage.bind(this));
        }
    }

    launchFbLogin() {
        /* Init variables */
        this.loading = true;
        this.locationOrigin = location.origin;
        this.failed = false;

        /* Check if facebookClientId is valid */
        if(this.facebookClientId.length < 1) {
            console.log('Facebook Client Id error');
            return;
        }

        /* Open window */
        this.authWindow = window.open('https://www.facebook.com/dialog/oauth?' + 
                                        '&response_type=token' +
                                        '&display=popup' +
                                        '&client_id=' + this.facebookClientId + 
                                        '&display=popup' + 
                                        '&redirect_uri=' + this.locationOrigin + '/assets/html/facebook-auth.html' + 
                                        '&scope=email', 
                                        null, 'width=600,height=400'
                                    );

    }

    handleMessage(event: Event) {
        /* Get message from event */
        const message = event as MessageEvent;
        /* Only trust messages from the below origin. */
        if (message.origin !== this.locationOrigin) return;
        /* Close windows*/
        this.authWindow.close();

        /* Get data */
        const result: FacebookRedirectionMessage = JSON.parse(message.data);

        /* Oops, error */
        if (result.status == false) {
            this.failed = true;
            this.loading = false;
            this.error = result.error;
            this.errorDescription = result.errorDescription;
        }
        /* Success */
        else {
            this._accountService.facebookLogin(result.accessToken)
            .subscribe(
                data => {
                    /* Get Authorization Header */
                    let auth_header = data['access_token'];
                    if (auth_header) {
                        /* Get JWT Token */
                        let auth_token = auth_header['Result'];
                        if (auth_token) {
                            /* Failed */
                            this.failed = false;

                            /* Save token in local storage*/
                            let user = { user: "facebook_account", token: auth_token };
                            localStorage.setItem(this.jwt_token, JSON.stringify(user));

                            /* Set log in variables*/
                            this._accountService.postLogIn();

                            
                            /* Navigate to route */
                            if(this.returnUrl)
                                this.router.navigate([this.returnUrl]);
                            else
                                this.router.navigate(['']);
                        }
                    }

                    /* Stop loader */
                    this.loading = false;
                },
                error => {
                    this.failed = true;
                    this.error = error;
                    this.loading = false;
            }); 
        }
    }
}