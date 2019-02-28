import { Component, OnInit } from '@angular/core';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { AccountService } from 'src/app/account-module/_services';
import { ForgotPasswordDto } from 'src/app/account-module/_models/forgot-password';

@Component({
    selector:'forgot-password-modal',
    templateUrl: 'forgot-password-modal.component.html'
})

export class ForgotPasswordModalComponent implements OnInit {
    _form: FormGroup;
    loading: boolean;

    constructor(fb: FormBuilder,
        private _accountService: AccountService,
        private _alertService: AlertService
    ) {
        this._form = fb.group({
            email: [null]
        });
    }

    ngOnInit() {
        this.loading = false;
    }

    submit() {
        /* Return if already submited */
        if (this.loading == true)
            return;
        /* Set loading true */
        this.loading = true;

        let modelDto = new ForgotPasswordDto(this.email.value);
        this._accountService.forgotPassword(modelDto)
            .subscribe(
                data => {
                    this.loading = false;

                    if (data.success == true)
                        this._alertService.success(data.message);
                    else
                        this._alertService.danger(data.message);
                },
                error => {
                    this._alertService.danger(error.error.message);
                    this.loading = false;
                }
            );
    }

    /* GETTERS  */
    get email() {
        return this._form.get('email') as FormControl;
    }
}
