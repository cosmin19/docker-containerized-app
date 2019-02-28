import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { AccountService } from '../_services';
import { AppConstants } from 'src/app/common-modules/shared-module/_constants/app-constants';

const USER: string = AppConstants.USER;
const ADMIN: string = AppConstants.ADMIN;

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(
        private _router: Router,
        private _accountService: AccountService) {

    }
    // Check if is not blocked
    // Check if APPROVED, not PENDING or smth else
    // Check if is activated
    // Check if USER/ADMIN

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

        if (this._accountService.getUserLoginStatus().getValue() == true) {

            const expectedRole = route.data['expectedRole'];
            const userRole = this._accountService.getUserRole();
            /* ********************************************* ROLE SECTION ********************************************* */
            /*
            * 1. Daca expected este ADMIN si userul NU este admin, redirecteaza catre homepage
            * 2. Daca expected este ADMIN si userul este admin, return true
            * 3. Daca expected este USER si userul este admin, return true
            * 4. Daca expected este USER si userul este user, return true
            * 5. Redirect catre pagina de login
            */

            if (expectedRole == ADMIN) {
                if (userRole == ADMIN) {
                    return true;
                }
                else if (userRole == USER) {
                    this._router.navigate(['']);
                    return false;
                }
                else {
                    this._router.navigate(['/login']);
                    return false;
                }
            }
            else if (expectedRole == USER) {
                if (userRole == ADMIN || userRole == USER) {
                    return true;
                }
                else {
                    this._router.navigate(['/login']);
                    return false;
                }
            }
            else {
                this._router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
                return false;
            }
        }

        this._router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
    }
}
