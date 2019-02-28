import { NgModule } from '@angular/core';
import { AccountService } from './_services';
import { AuthGuard } from './_guards';
import { AccountBaseService } from './_services/account-base.service';
import { LoginComponent } from './_components/login/login.component';
import { RegisterComponent } from './_components/register/register.component';
import { SharedModule } from '../common-modules/shared-module/shared.module';
import { EmailValidator, EqualValidator } from './_directives';
import { ResetPasswordComponent } from './_components/reset-password/reset-password.component';
import { EmailConfirmationComponent } from './_components/email-confirmation/email-confirmation.component';
import { ChangePasswordComponent } from './_components/change-password/change-password.component';
import { AccountRoutes } from './account.routing';
import { FacebookLoginComponent } from './_components/facebook-login/facebook-login.component';
import { ChangeEmailModalComponent } from './_components/_modals/change-emal-modal/change-email-modal.component';
import { ForgotPasswordModalComponent } from './_components/_modals/forgot-password-modal/forgot-password-modal.component';



@NgModule({
    imports: [
        AccountRoutes,
        SharedModule
    ],
    declarations: [
        LoginComponent,
        RegisterComponent,
        ResetPasswordComponent,
        EmailConfirmationComponent,
        ChangePasswordComponent,
        FacebookLoginComponent,
        ForgotPasswordModalComponent,
        ChangeEmailModalComponent,

        EmailValidator,
        EqualValidator
    ],
    providers: [
        AccountBaseService,
        AccountService,
        AuthGuard,
    ],
    exports: [
        ChangeEmailModalComponent
    ]
})
export class AccountModule { }
