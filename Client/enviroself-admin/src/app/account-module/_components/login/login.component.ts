import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../../_services';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { LogInDto } from '../../_models';
import { AlertService } from 'src/app/common-modules/shared-module/_services';

@Component({
    templateUrl: 'login.component.html',
})

export class LoginComponent  implements OnInit {
    _form: FormGroup;
    returnUrl: string = "";
    jwt_token: string = "";

    loading: boolean;

    constructor(fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private titleService: Title,
        private _accountService: AccountService,
        private _alertService: AlertService
    ) {
        this.jwt_token = _accountService.jwt_token;

        this._form = fb.group({
            email: [null],
            password: [null],
            rememberMe: [null]
        });

    }

    ngOnInit() {
        /* Set loading false */
        this.loading = false;

        /* Set log in title*/
        this.titleService.setTitle("Log In");

        /* If already logged in, go to my profile */
        if (this._accountService.getUserLoginStatus().getValue() == true) {
            this.router.navigate(['/user']);
        }
        /* Else, remove token and save returnUrl */
        else {
            this._accountService.logout(false);
            this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        }
    }
    submit() {
        /* Return if already submited */
        if(this.loading == true)
            return;
        /* Set loading true */
        this.loading = true;

        /// TODO: Remember me implementations 
        console.log("Remember me: ", this.rememberMe.value)

        let user = new LogInDto(this.email.value, this.password.value);
        /* Login */
        this._accountService.login(user)
        .subscribe(
            data => {
                /* Set loading false */
                this.loading = false;

                /* Get Authorization Header */
                let auth_header = data['access_token'];
                if (auth_header) {
                    /* Get JWT Token */
                    let auth_token = auth_header['Result'];
                    if (auth_token) {
                        /* Save token in local storage*/
                        let user = { user: this.email.value, token: auth_token };
                        localStorage.setItem(this.jwt_token, JSON.stringify(user));

                        /* Set log in variables*/
                        this._accountService.postLogIn();

                        /* Navigate to route */
                        this.router.navigate([this.returnUrl]);
                    }
                }
            },
            error => {
                /* Set loading false */
                this.loading = false;
                /* Remove token if exists */
                localStorage.removeItem(this.jwt_token);
                /* Alert */
                this._alertService.danger(error.error.message);
            }
        );
    }

    /* GETTERS  */
    get email() {
        return this._form.get('email') as FormControl;
    }

    get password() {
        return this._form.get('password') as FormControl;
    }
    
    get rememberMe() {
        return this._form.get('rememberMe') as FormControl;
    }
}

