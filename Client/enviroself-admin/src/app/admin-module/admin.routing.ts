import { AuthGuard } from "../account-module/_guards";
import { RouterModule, Routes } from "@angular/router";
import { DashboardComponent } from "./_components/dashboard/dashboard.component";
import { AppConstants } from "../common-modules/shared-module/_constants/app-constants";
import { UserListAdminComponent, UserAdminComponent } from "./_components/user";

const USER: string = AppConstants.USER;
const ADMIN: string = AppConstants.ADMIN;

export const adminRoutes: Routes = [
    {
        path: 'admin', 
        canActivate: [AuthGuard],
        data: {
            expectedRole: ADMIN
        },
        children: [
            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full',
            },
            {
                path: 'dashboard',
                component: DashboardComponent,
            },
            { 
                path: 'users', 
                component: UserListAdminComponent 
            },
            {
                path: 'user',
                component: UserAdminComponent
            }
        ]
    },
];

export const AdminRoutes = RouterModule.forChild(adminRoutes); 