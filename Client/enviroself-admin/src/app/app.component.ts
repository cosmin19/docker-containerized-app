import { Component, OnInit, AfterViewInit } from '@angular/core';
import { TemplateService } from './common-modules/shared-module/_services/template.service';
import { AccountService } from './account-module/_services';
import { PusherService } from './common-modules/shared-module/_services/pusher.service';
import { AlertService } from './common-modules/shared-module/_services';
import { PusherConstants } from './common-modules/shared-module/_constants/pusher-constants';
import { UserStatusDto } from './common-modules/shared-module/_components/pusher-test/pusher-test.component';

declare var $: any;

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})

export class AppComponent implements OnInit, AfterViewInit {
    isLoggedIn: boolean = false;

    constructor(
        private _accountService: AccountService,
        private _pusherService: PusherService,
        private _templateService: TemplateService,
        private _alertService: AlertService
    ) {
        /* Check user status*/
        this._accountService.checkUserStatus();

        /* Bind to other users status */
        this._pusherService.channel.bind(PusherConstants.USER_STATUS_EVENT_NAME, (data: UserStatusDto) => {
            this.showUserStatus(data);
        });
    }

    ngOnInit() {
        /* Disable parent scroll (for lists in page)*/
        this._templateService.disableParentScroll();
    }

    ngAfterViewInit(): void {
        /* Subscribe to user status */
        this._accountService.getUserLoginStatus()
            .subscribe(
                (isLoggedIn: boolean) => {
                    /* Check user LOGIN status */
                    this.isLoggedIn = isLoggedIn;
                    if (isLoggedIn)
                        this.emitUserOnlineStatus();
                }
            );
    }

    showUserStatus(data: UserStatusDto) {
        if (!this.isLoggedIn)
            return;

        if (data.id == this._accountService.getUserId())
            return;

        if (data.online == true)
            this._alertService.userOnline(data.email + " is online.");
    }

    emitUserOnlineStatus() {
        this._pusherService.userOnline().subscribe(() => { });
    }
}
