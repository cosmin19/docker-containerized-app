import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from 'src/app/account-module/_services';
import { TemplateService } from '../shared-module/_services/template.service';
import { AppConstants } from '../shared-module/_constants/app-constants';

declare var $: any;

export interface RouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
    role_required: string;
}

export const ROUTES: RouteInfo[] = [
    // Account
    { path: 'login', title: 'Log In', icon: 'ti-user', class: '', role_required: 'anonymous' },
    { path: 'register', title: 'Register', icon: 'ti-user', class: '', role_required: 'anonymous' },

    //User
    { path: 'user', title: 'My Account', icon: 'ti-user', class: '', role_required: AppConstants.USER },
    { path: 'change-password', title: 'Change Password', icon: 'ti-key', class: 'hidden', role_required: AppConstants.USER },
    { path: 'mediafiles', title: 'Media files', icon: 'ti-key', class: '', role_required: AppConstants.USER },
];

@Component({
    selector: 'sidebar-cmp',
    templateUrl: 'sidebar.component.html',
})

export class SidebarComponent implements OnInit {
    menuItems: any[];

    isLoggedIn: boolean = false;

    constructor(
        private _accountService: AccountService, 
        private templateService: TemplateService
    ) { 

    }

    ngOnInit() {
        /* Subscribe to LOGGED IN modifications */
        this._accountService.getUserLoginStatus()
        .subscribe(
            (isLoggedIn: boolean) => {
                /* Check user LOGIN status */
                this.isLoggedIn = isLoggedIn;
                
                /* Set nav items */
                this.setMenuItems(isLoggedIn);
            }
        );
    }

    setMenuItems(isLoggedIn: boolean) {
        if(isLoggedIn == true)
            this.menuItems = ROUTES.filter(menuItem => menuItem.role_required == AppConstants.USER);
        else
            this.menuItems = [];
    }



    sidebarToggle() {
        this.templateService.sidebarToggle();
    }
    panelToggle(value: boolean) {
        this.setMenuItems(this.isLoggedIn);
    }
    isMobileMenu() {
        if ($(window).width() > 991) {
            return false;
        }
        return true;
    }

    logOut() {
        this._accountService.logout(true);
    }

    getUserEmail() {
        return this._accountService.getUserEmail();
    }
}
