import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AlertService } from './_services';
import { GreaterThanZeroValidator } from './_directives/validators/greaterThanZero-validator.directive';
import { SafePipe } from './_directives/pipes/safe.pipe';
import { PageNotFoundComponent } from './_components/page-not-found/page-not-found.component';
import { TemplateService } from './_services/template.service';
import { TableModule } from 'primeng/table';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AvatarModule } from 'ngx-avatar';
import { PusherService } from './_services/pusher.service';
import { PusherTestComponent } from './_components/pusher-test/pusher-test.component';
import { ConfirmationModalComponent } from './_components/_modals/confirmation-modal/confirmation-modal.component';

@NgModule({
    imports: [
        CommonModule,
        BrowserModule,
        BrowserAnimationsModule,

        FormsModule,
        ReactiveFormsModule,

        RouterModule,
        HttpClientModule,

        // PrimeNG
        TableModule,

        AvatarModule
    ],
    declarations: [
        SafePipe,
        GreaterThanZeroValidator,
        PageNotFoundComponent,
        PusherTestComponent,
        ConfirmationModalComponent
    ],
    providers: [
        AlertService,
        TemplateService,
        PusherService,

    ],
    exports: [
        // Angular Modules
        CommonModule,
        BrowserModule,
        BrowserAnimationsModule,

        FormsModule,
        ReactiveFormsModule,

        RouterModule,
        HttpClientModule,

        // PrimeNG Modules
        TableModule,

        AvatarModule,

        // Directives
        SafePipe,
        GreaterThanZeroValidator,

        // Components
        PageNotFoundComponent,
        PusherTestComponent,
        ConfirmationModalComponent
    ]
})
export class SharedModule { }
