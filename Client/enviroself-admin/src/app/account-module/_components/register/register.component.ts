
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../../_services';
import { CustomValidators } from '../../_directives';
import { RegisterDto } from '../../_models';
import { Title } from '@angular/platform-browser';
import { AlertService } from 'src/app/common-modules/shared-module/_services';

@Component({
    templateUrl: 'register.component.html'
})

export class RegisterComponent implements OnInit {
    _form: FormGroup;
    loading: boolean;

    constructor(fb: FormBuilder,
        private router: Router,
        private _accountService: AccountService,
        private _alertService: AlertService,
        private titleService: Title,
    ) {
        if (_accountService.getUserLoginStatus().getValue() == true)
            this._accountService.logout(false);
        
        this._form = fb.group({
            email: [null],
            password: [null],
            confirmPassword: [null],
            firstname: [null],
            lastname: [null],
            termsAndConditions: [null]
        }, { validator: CustomValidators.CheckIfMatchingPasswords('password', 'confirmPassword') });
    }

    ngOnInit(): void {
        /* Set title */
        this.titleService.setTitle("Register");

        /* Set loading false */
        this.loading = false;
    }

    submit(): void {
        /* If condition not accepted, return  */
        if(this.termsAndConditions.value == false)
            return;

        /* Return if already submited */
        if(this.loading == true)
            return;

        /* Set loading true */
        this.loading = true;

        /* Map user data's */
        let user = new RegisterDto();
        user.email = this.email.value;
        user.password = this.password.value;
        user.firstname = this.firstname.value;
        user.lastname = this.lastname.value;
        user.termsAndConditions = this.termsAndConditions.value;

        /* Register */
        this._accountService.register(user)
        .subscribe(
            data => {
                /* Show succes */
                this._alertService.success(data.message);

                /* Navigate to login */
                this.router.navigate(['/login']);

                /* Set loading false */
                this.loading = false;
            },
            error => {
                /* Set loading false */
                this.loading = false;
                
                /*Show error */
                this._alertService.danger(error.error.message);
            }
        );
    }

    /* GETTERS */
    get email() {
        return this._form.get('email') as FormControl;
    }
    get password() {
        return this._form.get('password') as FormControl;
    }
    get confirmPassword() {
        return this._form.get('confirmPassword') as FormControl;
    }
    get firstname() {
        return this._form.get('firstname') as FormControl;
    }
    get lastname() {
        return this._form.get('lastname') as FormControl;
    }
    get termsAndConditions() {
        return this._form.get('termsAndConditions') as FormControl;
    }
}
