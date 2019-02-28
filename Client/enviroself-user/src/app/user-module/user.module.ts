import { NgModule } from "@angular/core";
import { UserComponent } from "./_components/user/user.component";
import { SharedModule } from "../common-modules/shared-module/shared.module";
import { UserService } from "./_services/user.service";
import { UserRoutes } from "./user.routing";
import { AccountModule } from "../account-module/account.module";

@NgModule({
    imports: [         
        UserRoutes,
        SharedModule,
        AccountModule
    ],
    declarations: [ 
        UserComponent,
    ],
    providers: [
        UserService
    ],
    exports: []
})
export class UserModule { }