import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../../_services';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { ResetPasswordDto } from '../../_models/reset-password';
import { CustomValidators } from '../../_directives';

@Component({
    templateUrl: 'reset-password.component.html',
})

export class ResetPasswordComponent  implements OnInit {
    _form: FormGroup;
    loading: boolean;

    userId: number;
    token: string;

    constructor(fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private titleService: Title,
        private _accountService: AccountService,
        private _alertService: AlertService
    ) {
        this._form = fb.group({
            password: [null],
            confirmPassword: [null]
        }, { validator: CustomValidators.CheckIfMatchingPasswords('password', 'confirmPassword') });
    }

    ngOnInit() {
        /* Set loading false */
        this.loading = false;

        /* Set Reset password title*/
        this.titleService.setTitle("Reset password");

        /* If already logged in, log out */
        if (this._accountService.getUserLoginStatus().getValue() == true) {
            this._accountService.logout(false);
        }

        /* Get params */
        this.route.queryParams.subscribe(params => {
            this.token = params['token'].replace(/ /g, '+'); // Angular replace '+' with ' ' so we replace them back
            this.userId = Number(params['userId'])
        });

        /*  Validate params */
        if(this.userId == null || this.userId == 0 || this.token == null || this.token.length == 0) {
            this._alertService.danger("Invalid data tokens");
            this.router.navigate(['/login']);
        }
    }
    submit() {
        /* Return if already submited */
        if(this.loading == true)
            return;
        /* Set loading true */
        this.loading = true;

        let modelDto = new ResetPasswordDto(this.userId, this.token, this.password.value);
        /* Reset password */
        this._accountService.resetPassword(modelDto)
        .subscribe(
            data => {
                if(data.success == true) {
                    this._alertService.success(data.message);
                    this.router.navigate(['/login']);
                }
                else {
                    this.loading = false;
                    this._alertService.danger(data.message);
                }
            },
            error => {
                this.loading = false;
                this._alertService.danger(error.error.message);
            }
        );
    }

    /* GETTERS  */
    get password() {
        return this._form.get('password') as FormControl;
    }
    
    get confirmPassword() {
        return this._form.get('confirmPassword') as FormControl;
    }
}

