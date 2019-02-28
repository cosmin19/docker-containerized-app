import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from "@angular/core";
import { SharedModule } from "../common-modules/shared-module/shared.module";
import { AdminRoutes } from "./admin.routing";
import { UserAdminService } from "./_services/user-admin.service";
import { DashboardComponent } from "./_components/dashboard/dashboard.component";
import { UserListAdminComponent, UserAdminComponent } from "./_components/user";

@NgModule({
    imports: [
        AdminRoutes,
        SharedModule
    ],
    declarations: [
        DashboardComponent,
        UserListAdminComponent,
        UserAdminComponent
    ],
    providers: [
        UserAdminService
    ],  
    schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class AdminModule { }
