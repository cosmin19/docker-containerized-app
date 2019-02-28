import { PagingHeader } from "./paging-header";

export class PagedListDto<T> {
    list: T[];
    pagingHeader: PagingHeader;
}