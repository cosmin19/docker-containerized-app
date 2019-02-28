import { AuthGuard } from "../account-module/_guards";
import { RouterModule, Routes } from "@angular/router";
import { UserComponent } from "./_components/user/user.component";
import { ChangePasswordComponent } from "../account-module/_components/change-password/change-password.component";
import { AppConstants } from "../common-modules/shared-module/_constants/app-constants";

const USER: string = AppConstants.USER;
const ADMIN: string = AppConstants.ADMIN;

export const userRoutes: Routes = [
    {
        path: 'user',
        component: UserComponent,
        canActivate: [AuthGuard],
        data: { expectedRole: USER }
    },
    {
        path:'change-password',
        component: ChangePasswordComponent,
        data: { expectedRole: USER }
    }
];

export const UserRoutes = RouterModule.forChild(userRoutes); 