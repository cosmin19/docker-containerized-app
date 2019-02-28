import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AlertService } from 'src/app/common-modules/shared-module/_services';
import { UserService } from 'src/app/user-module/_services/user.service';
import { MediaFileService } from '../../_services/media-file.service';
import { Title } from '@angular/platform-browser';
import { LazyLoadEvent } from 'primeng/primeng';
import { PagedListDto } from 'src/app/common-modules/shared-module/_models/pagination/paged-list';
import { MediaFileFilterDto } from '../../_models/media-file-filter';
import { MediaFileSmallDto } from '../../_models/media-file-small';
import { saveAs } from 'file-saver';

@Component({
    templateUrl: 'media-file.component.html',
    encapsulation: ViewEncapsulation.None
})

export class MediaFileComponent implements OnInit {

    filter: MediaFileFilterDto = new MediaFileFilterDto();
    list: MediaFileSmallDto[] = [];
    totalItems: number = 0;

    isRequested: boolean = false;

    confirmationSummary: string = "Are you sure?"
    confirmationDetails: string = "Confirm to proceed";

    fileAboutToDelete: number = 0;

    constructor(
        private _userService: UserService,
        private titleService: Title,
        private _alertService: AlertService,
        private _mediaFileService: MediaFileService
    ) {
    }

    ngOnInit() { 
        this.titleService.setTitle('Media files');
        this.getData();
    }

    loadLazy(event: LazyLoadEvent) {
        this.filter.pageSize = event.rows;
        this.filter.pageNumber = event.first / event.rows + 1;

        this.getData();
    }

    getData(){
        // If already requested, wait for response
        if(this.isRequested == true)
            return;
        
        this.isRequested = true;

        this._mediaFileService.getFileListFiltered(this.filter)
        .subscribe (
            (data: PagedListDto<MediaFileSmallDto>) => {
                this.isRequested = false;

                this.list = data.list;
                this.totalItems = data.pagingHeader.totalItems;
            },
            error => {
                this.isRequested = false;

                this._alertService.danger(error.error.message);
        });
    }

    upload(event, form): void {
        if (this.isRequested == true){
            this._alertService.info("Uploading...");
            return;
        }
        this.isRequested = true;

        if (event.files.length == 0) {
            this._alertService.warning("No file selected");
            return;
        }

        let input = new FormData();
        var fileToUpload = event.files[0];
        input.append("file", fileToUpload);

        this._mediaFileService.upload(input)
        .subscribe(
            data => {
                this.isRequested = false;
                form.clear();
                if (data.success) {
                    this._alertService.success(data.message);
                    this.getData();
                }
                else
                    this._alertService.danger(data.message);
            },
            error => {
                this.isRequested = false;
                form.clear();
                this._alertService.danger(error.error.message);

            }
        );
    }

    download(fileId: number, fileName: string) {
        this._mediaFileService.download(fileId)
        .subscribe(
            blob => {
                saveAs(blob, fileName, {type: blob.type});
            },
            error => {
                this._alertService.danger(error.error.message);
            }
        )
    }

    delete(fileId: number) {
        this.fileAboutToDelete = fileId;
    }

    confirmationResponse(value: boolean) {
        if(value) {
            this._mediaFileService.delete(this.fileAboutToDelete)
            .subscribe(
                data => {
                    if(data.success) {
                        this._alertService.success(data.message);
                        this.getData();
                    }
                    else
                        this._alertService.danger(data.message);

                    this.fileAboutToDelete = 0;
                },
                error => {
                    this._alertService.danger(error.error.message);
                    this.fileAboutToDelete = 0;
                }
            );
        }
        else {
            this.fileAboutToDelete = 0;
        }
    }
}
