import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { UserAdminService } from '../../../_services/user-admin.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserAdminDto } from 'src/app/admin-module/_models/user/user';
import { SelectListItem } from 'src/app/common-modules/shared-module/_models/select-list-item';
import { UserEditAdminDto } from 'src/app/admin-module/_models/user/user-edit';
import { AccountService } from 'src/app/account-module/_services';

@Component({
    templateUrl: './user-admin.component.html',
    encapsulation: ViewEncapsulation.None
})
export class UserAdminComponent implements OnInit {

    userId: number;
    user: UserAdminDto = new UserAdminDto();
    _form: FormGroup;
    genderList: SelectListItem[];
    roleList: SelectListItem[];
    emailDisabled: boolean = true;

    pictureName:string;
    pictureUrl: string;

    isRequested: boolean = false;

    selfEmailChangedLogOutAfter: boolean = false;
    selfRoleChangedLogOutAfter: boolean = false;
    loading: boolean = true;

    constructor(
        fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private titleService: Title,
        private _userAdminService: UserAdminService,
        private _accountService: AccountService,
        private _alertService: AlertService
    ) { 
        this._form = fb.group({
            role: [null],
            email: [null],
            emailConfirmed: [null],
            phoneNumber: [null],
            firstname: [null],
            lastname: [null],
            gender: [null],
            accessFailedCount: [{value: null, disabled: true}],
            lockoutEnabled: [null],
            lockoutEnd: [{value: null, disabled: true}],
            facebookId: [{value: null, disabled: true}],
            createdOnUtc: [{value: null, disabled: true}]
        });
    }

    change(){
        this.emailDisabled = !this.emailDisabled;
    }

    ngOnInit() {
        this.titleService.setTitle('Admin - User');

        /* Get paramns */
        this.route.queryParams.subscribe(params => {
            this.userId = Number(params['userId'])
        });

        if(!this.userId || this.userId == 0) {
            this._alertService.danger("Invalid user");
            this.router.navigate(['admin/users']);
            return;
        }

        this.prepareData();
    }

    prepareData() {
        this._userAdminService.getUser(this.userId)
        .subscribe(
            data => {
                /* Get data */
                this.user = data;

                if(data.pictureUrl)
                    this.pictureUrl = data.pictureUrl
                else if(data.firstname && data.lastname)
                    this.pictureName = data.firstname + " " + data.lastname;
                else
                    this.pictureName = data.email;
                
                /* Prepare gender list */
                this.genderList = [{ value: '0', text: "Gender" }];
                if(data.genderList)
                    this.genderList = this.genderList.concat(data.genderList);

                /* Set role list */
                this.roleList = data.roleList;

                /* Populate form data */
                this.populateForm();

                this.loading = false;
            },
            error => {
                this._alertService.danger(error.error.message);
            }
        )
    }

    populateForm() {
        this._form.get('role').setValue(this.user.role);
        this._form.get('email').setValue(this.user.email);
        this._form.get('emailConfirmed').setValue(this.user.emailConfirmed);
        this._form.get('phoneNumber').setValue(this.user.phoneNumber);
        this._form.get('firstname').setValue(this.user.firstname);
        this._form.get('lastname').setValue(this.user.lastname);

        if(this.user.gender)
            this._form.get('gender').setValue(this.user.gender);
        else
            this._form.get('gender').setValue('0');

        this._form.get('accessFailedCount').setValue(this.user.accessFailedCount);
        this._form.get('lockoutEnabled').setValue(this.user.lockoutEnabled);
        this._form.get('lockoutEnd').setValue(this.user.lockoutEnd);
        this._form.get('facebookId').setValue(this.user.facebookId);
        this._form.get('createdOnUtc').setValue(this.user.createdOnUtc);
    }

    submit() {
        // If already requested, wait for response
        if(this.isRequested == true)
            return;
        
        this.isRequested = true;
        this.loading = true;

        let model = new UserEditAdminDto();

        model.id = this.userId;
        model.role = this.role.value;
        if(model.role != this.user.role && this.user.email == this._accountService.getUserEmail())
            this.selfRoleChangedLogOutAfter = true;

        model.editEmail = !this.emailDisabled;
        model.emailConfirmed = this.emailConfirmed.value;
        if(model.editEmail) {
            model.email = this.email.value;
            if(this.user.email == this._accountService.getUserEmail())
                this.selfEmailChangedLogOutAfter = true;
        }

        model.phoneNumber = this.phoneNumber.value;
        model.firstname = this.firstname.value;
        model.lastname = this.lastname.value;
        model.gender = this.gender.value;

        model.lockoutEnabled = this.lockoutEnabled.value;

        console.log(model.phoneNumber);

        this._userAdminService.editUser(model)
        .subscribe(
            data => {
                this.isRequested = false;
                if(data.success) {
                    this._alertService.success(data.message);
                    if(this.selfEmailChangedLogOutAfter || this.selfRoleChangedLogOutAfter)
                        this._accountService.logout(true);
                }
                else
                    this._alertService.danger(data.message);
                this.prepareData();
            },
            error => {
                this.isRequested = false;
                this._alertService.danger(error.error.message);
            }
        );
    }

    /* GETTERS  */
    get role() {
        return this._form.get('role') as FormControl;
    }
    get email() {
        return this._form.get('email') as FormControl;
    }
    get emailConfirmed() {
        return this._form.get('emailConfirmed') as FormControl;
    }
    get phoneNumber() {
        return this._form.get('phoneNumber') as FormControl;
    }
    get firstname() {
        return this._form.get('firstname') as FormControl;
    }
    get lastname() {
        return this._form.get('lastname') as FormControl;
    }
    get gender() {
        return this._form.get('gender') as FormControl;
    }
    get accessFailedCount() {
        return this._form.get('accessFailedCount') as FormControl;
    }
    get lockoutEnabled() {
        return this._form.get('lockoutEnabled') as FormControl;
    }
    get lockoutEnd() {
        return this._form.get('lockoutEnd') as FormControl;
    }
    get facebookId() {
        return this._form.get('facebookId') as FormControl;
    }
    get createdOnUtc() {
        return this._form.get('createdOnUtc') as FormControl;
    }
}
