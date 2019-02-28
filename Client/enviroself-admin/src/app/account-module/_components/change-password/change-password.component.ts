import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../../_services';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { CustomValidators } from '../../_directives';
import { ChangePasswordDto } from '../../_models';

@Component({
    templateUrl: 'change-password.component.html',
})

export class ChangePasswordComponent  implements OnInit {
    _form: FormGroup;
    loading: boolean;

    constructor(fb: FormBuilder,
        private router: Router,
        private titleService: Title,
        private _accountService: AccountService,
        private _alertService: AlertService
    ) {
        this._form = fb.group({
            oldPassword: [null],
            newPassword: [null],
            confirmPassword: [null]
        }, { validator: CustomValidators.CheckIfMatchingPasswords('newPassword', 'confirmPassword') });
    }

    ngOnInit() {
        /* Set loading false */
        this.loading = false;

        /* Set Change password title*/
        this.titleService.setTitle("Change password");
    }

    submit() {
        /* Return if already submited */
        if(this.loading == true)
            return;
        /* Set loading true */
        this.loading = true;

        let modelDto = new ChangePasswordDto(this.oldPassword.value, this.newPassword.value);
        /* Change Password */
        this._accountService.changePassword(modelDto)
        .subscribe(
            data => {
                if(data.success == true) {
                    this._alertService.success(data.message);
                    this.router.navigate(['']);
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
    get oldPassword() {
        return this._form.get('oldPassword') as FormControl;
    }

    get newPassword() {
        return this._form.get('newPassword') as FormControl;
    }
    
    get confirmPassword() {
        return this._form.get('confirmPassword') as FormControl;
    }
}

