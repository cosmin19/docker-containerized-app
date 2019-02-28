import { NgModule } from "@angular/core";
import { SharedModule } from "../common-modules/shared-module/shared.module";
import { AccountModule } from "../account-module/account.module";
import { MediaRoutes } from "./media.routing";
import { MediaFileComponent } from "./_components/media-file/media-file.component";
import { MediaFileService } from "./_services/media-file.service";
import { FileUploadModule } from 'primeng/fileupload';

@NgModule({
    imports: [         
        MediaRoutes,
        SharedModule,
        AccountModule,
        FileUploadModule
    ],
    declarations: [
        MediaFileComponent,
    ],
    providers: [
        MediaFileService
    ],
    exports: []
})
export class MediaModule { }