import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { UserSmallDto } from '../../_models/user/user-small';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { UserEditDto } from '../../_models/user/user-edit';
import { SelectListItem } from 'src/app/common-modules/shared-module/_models/select-list-item';
import { Title } from '@angular/platform-browser';

@Component({
    templateUrl: 'user.component.html',
    encapsulation: ViewEncapsulation.None
})

export class UserComponent implements OnInit {
    user: UserSmallDto;
    _form: FormGroup;
    loading: boolean;
    
    pictureUrl: string;// = "assets/img/avatar.jpg"
    genderList: SelectListItem[];
    pictureName:string;

    constructor(fb: FormBuilder,
        private _userService: UserService,
        private _alertService: AlertService,
        private _titleService: Title
    ) {
        this._form = fb.group({
            phone: [null],
            firstname: [null],
            lastname: [null],
            gender: [null]
        });
    }

    ngOnInit(){
        this.loading = false;
        this._titleService.setTitle("User");
        this.prepareUserData();
    }

    prepareUserData() {
        this._userService.getUser()
        .subscribe(
            data => {
                this.user = data;
                if(data.pictureUrl)
                    this.pictureUrl = data.pictureUrl
                else if(data.firstname && data.lastname)
                    this.pictureName = data.firstname + " " + data.lastname;
                else
                    this.pictureName = data.email;

                this.genderList = [{ value: '0', text: "Gender" }];
                if(data.genderList)
                    this.genderList = this.genderList.concat(data.genderList);

                this._form.get('phone').setValue(data.phone); 
                this._form.get('firstname').setValue(data.firstname); 
                this._form.get('lastname').setValue(data.lastname);
                if(data.gender)
                    this._form.get('gender').setValue(data.gender);
                else
                    this._form.get('gender').setValue('0');
            },
            error => {
                this._alertService.danger(error.error.message);
            }
        );
    }

    submit() {
        /* Return if already submited */
        if(this.loading == true)
            return;
        /* Set loading true */
        this.loading = true;

        let model = new UserEditDto(this.phone.value, this.firstname.value, this.lastname.value, this.gender.value);
        /* Login */
        this._userService.editUser(model)
        .subscribe(
            data => {
                /* Set loading false */
                this.loading = false;

                if(data.success) {
                    this._alertService.success(data.message);
                    this.prepareUserData();
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
    get phone() {
        return this._form.get('phone') as FormControl;
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
}
