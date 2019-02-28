import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AccountService } from '../../_services';
import { Title } from '@angular/platform-browser';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { EmailConfirmationDto } from '../../_models/email-confirmation';

@Component({
    templateUrl: 'email-confirmation.component.html',
})

export class EmailConfirmationComponent implements OnInit {
    userId: number;
    token: string;

    successfullyActivated: boolean;
    responseReceived: boolean;

    constructor(
        private route: ActivatedRoute,
        private titleService: Title,
        private _accountService: AccountService,
        private _alertService: AlertService
    ) { }

    ngOnInit() {
        /* Set Email confirmation title*/
        this.titleService.setTitle("Email confirmation");

        this.successfullyActivated = false;
        this.responseReceived = false;

        /* If already logged in, Log out */
        if (this._accountService.getUserLoginStatus().getValue() == true) {
            this._accountService.logout(false);
        }

        /* Get paramns */
        this.route.queryParams.subscribe(params => {
            this.token = params['token'].replace(/ /g, '+');  // Angular replace '+' with ' ' so we replace them back
            this.userId = Number(params['userId'])
        });

        /* Validate params */
        if(this.userId == null || this.userId == 0 || this.token == null || this.token.length == 0) {
            this._alertService.danger("Invalid data tokens");
        }

        this.activate();
    }

    activate() {
        let modelDto = new EmailConfirmationDto(this.userId, this.token);

        /* Confirm Email */
        this._accountService.confirmEmail(modelDto)
        .subscribe(
            data => {
                this.responseReceived = true;

                if(data.success == true) {
                    this._alertService.success(data.message);
                    this.successfullyActivated = true;
                }
                else {
                    this._alertService.danger(data.message);
                    this.successfullyActivated = false;
                }
            },
            error => {
                this.responseReceived = true;
                this._alertService.danger(error.error.message);
            }
        );
    }
}

