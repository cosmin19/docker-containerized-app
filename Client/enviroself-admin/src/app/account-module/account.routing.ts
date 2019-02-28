import { RouterModule, Routes } from "@angular/router";
import { LoginComponent } from "./_components/login/login.component";
import { RegisterComponent } from "./_components/register/register.component";
import { ResetPasswordComponent } from "./_components/reset-password/reset-password.component";
import { EmailConfirmationComponent } from "./_components/email-confirmation/email-confirmation.component";
import { AppConstants } from "../common-modules/shared-module/_constants/app-constants";

const USER: string = AppConstants.USER;
const ADMIN: string = AppConstants.ADMIN;

export const accountRoutes: Routes = [
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'register',
        component: RegisterComponent
    },
    {
        path: 'account/reset-password',
        component: ResetPasswordComponent
    },
    {
        path: 'register/validate',
        component: EmailConfirmationComponent
    }
];

export const AccountRoutes = RouterModule.forChild(accountRoutes);