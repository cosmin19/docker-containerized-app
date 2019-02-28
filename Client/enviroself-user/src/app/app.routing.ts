import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './common-modules/shared-module/_components/page-not-found/page-not-found.component';
import { AppConstants } from './common-modules/shared-module/_constants/app-constants';

const USER: string = AppConstants.USER;
const ADMIN: string = AppConstants.USER;

export const AppRoutes: Routes = [
    {
        path: '',
        redirectTo: 'user',
        pathMatch: 'full',
    },
    { 
        path: '**', 
        component: PageNotFoundComponent 
    }
]
