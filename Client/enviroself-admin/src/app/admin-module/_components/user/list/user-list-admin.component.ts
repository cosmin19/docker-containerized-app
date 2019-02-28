import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { UserSmallAdminDto } from '../../../_models/user/user-small';
import { PagedListDto } from 'src/app/common-modules/shared-module/_models/pagination/paged-list';
import { LazyLoadEvent } from 'primeng/primeng';
import { FormBuilder } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { UserAdminService } from '../../../_services/user-admin.service';
import { UserFilterAdminDto } from '../../../_models/user/user-filter';

@Component({
    templateUrl: './user-list-admin.component.html',
    encapsulation: ViewEncapsulation.None
})
export class UserListAdminComponent implements OnInit {

    userFilter: UserFilterAdminDto = new UserFilterAdminDto();
    users: UserSmallAdminDto[] = [];
    usersTotalItems: number = 0;

    isRequested: boolean = false;
    searchButtonMessage: string = "Search";

    constructor(
        fb: FormBuilder,
        private titleService: Title,
        private _userAdminService: UserAdminService,
        private _alertService: AlertService
    ) { }

    ngOnInit() {
        this.titleService.setTitle('Admin - Users');
        this.getData();
    }

    loadUsersLazy(event: LazyLoadEvent) {
        this.userFilter.pageSize = event.rows;
        this.userFilter.pageNumber = event.first / event.rows + 1;

        this.getData();
    }

    getData(){
        // If already requested, wait for response
        if(this.isRequested == true)
            return;
        
        this.isRequested = true;
        this.searchButtonMessage = "Loading...";

        this._userAdminService.getUserListFiltered(this.userFilter)
        .subscribe (
            (data: PagedListDto<UserSmallAdminDto>) => {
                this.isRequested = false;
                this.searchButtonMessage = "Search";

                this.users = data.list;
                this.usersTotalItems = data.pagingHeader.totalItems;
            },
            error => {
                this.isRequested = false;
                this.searchButtonMessage = "Search";

                this._alertService.danger(error.error.message);
        });
    }
}
