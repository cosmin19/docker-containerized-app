import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { AccountService } from 'src/app/account-module/_services';
import { ROUTES } from '../sidebar-module/sidebar.component';
import { TemplateService } from '../shared-module/_services/template.service';

@Component({
    selector: 'navbar-cmp',
    templateUrl: 'navbar.component.html'
})

export class NavbarComponent implements OnInit{
    private listTitles: any[];
    location: Location;

    isLoggedIn: boolean = false;

    // @ViewChild("navbar-cmp") button;

    constructor(location:Location, private _accountService: AccountService, private templateService: TemplateService) {
        this.location = location;
    }

    ngOnInit(){
        this.listTitles = ROUTES.filter(listTitle => listTitle);

        /* Subscribe to LOGGED IN modifications */
        this._accountService.getUserLoginStatus()
        .subscribe(
            (isLoggedIn: boolean) => {
                this.isLoggedIn = isLoggedIn;
            }
        );
    }

    getTitle(){
        var titlee = window.location.pathname;
        titlee = titlee.substring(1);
        for(var item = 0; item < this.listTitles.length; item++){
            if(this.listTitles[item].path === titlee){
                return this.listTitles[item].title;
            }
        }
        return '';
    }

    sidebarToggle() {
        this.templateService.sidebarToggle();
    }

    logOut() {
        this._accountService.logout(true);
    }

    getUserEmail() {
        return this._accountService.getUserEmail();
    }
}
