import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { AppRoutes } from './app.routing';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { SidebarModule } from './common-modules/sidebar-module/sidebar.module';
import { NavbarModule } from './common-modules/navbar-module/navbar.module';
import { FooterModule } from './common-modules/footer-module/footer.module';
import { FixedPluginModule } from './common-modules/fixedplugin-module/fixedplugin.module';
import { AccountModule } from './account-module/account.module';
import { AdminModule } from './admin-module/admin.module';
import { SharedModule } from './common-modules/shared-module/shared.module';
import { AppConfig } from './app.config';

@NgModule({
    declarations: [
        AppComponent,
    ],
    imports: [
        RouterModule.forRoot(AppRoutes, {scrollPositionRestoration: 'top'}),

        SidebarModule,
        NavbarModule,
        FooterModule,
        FixedPluginModule,

        SharedModule,
        AccountModule,
        AdminModule
    ],
    providers: [
        AppConfig,
    ],
    schemas: [ CUSTOM_ELEMENTS_SCHEMA ],
    bootstrap: [AppComponent]
})
export class AppModule { }
