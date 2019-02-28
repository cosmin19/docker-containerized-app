import { AuthGuard } from "../account-module/_guards";
import { RouterModule, Routes } from "@angular/router";
import { AppConstants } from "../common-modules/shared-module/_constants/app-constants";
import { MediaFileComponent } from "./_components/media-file/media-file.component";

const USER: string = AppConstants.USER;
const ADMIN: string = AppConstants.ADMIN;

export const routes: Routes = [
    {
        path: 'mediafiles',
        component: MediaFileComponent,
        canActivate: [AuthGuard],
        data: { expectedRole: USER }
    }
];

export const MediaRoutes = RouterModule.forChild(routes); 