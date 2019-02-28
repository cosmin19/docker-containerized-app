import { Component, OnInit } from '@angular/core';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { ChangeEmailDto } from 'src/app/account-module/_models/change-email';
import { AccountService } from 'src/app/account-module/_services';

@Component({
    selector:'change-email-modal',
    templateUrl: 'change-email-modal.component.html'
})

export class ChangeEmailModalComponent implements OnInit {
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

    ngOnInit(){
        this.loading = false;
    }

    submit() {
        /* Return if already submited */
        if(this.loading == true)
            return;
        /* Set loading true */
        this.loading = true;


        let model = new ChangeEmailDto(this.email.value);
        /* Login */
        this._accountService.changeEmail(model)
        .subscribe(
            data => {
                /* Set loading false */
                this.loading = false;

                if(data.success) {
                    this._alertService.success(data.message);
                    this._accountService.logout(true);
                }
                else
                    this._alertService.danger(data.message);
            },
            error => {
                /* Set loading false */
                this.loading = false;
                /* Alert */
                this._alertService.danger(error.error.message);
            }
        );
    }

    /* GETTERS  */
    get email() {
        return this._form.get('email') as FormControl;
    }
}
