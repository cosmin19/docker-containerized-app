import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
    selector:'confirmation-modal',
    templateUrl: 'confirmation-modal.component.html'
})

export class ConfirmationModalComponent implements OnInit {

    @Input() summary: string;
    @Input() details: string;
    @Output() response: EventEmitter<boolean> = new EventEmitter<boolean>();
    
    constructor( ) { }

    ngOnInit(){
    }

    submit(response: boolean) {
        this.response.emit(response);
    }
}
