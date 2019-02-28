import { Injectable } from '@angular/core';

declare var $: any;

@Injectable()
export class AlertService {

    type = ['','info','success','warning','danger'];
    from = ['top', 'bottom']; // 'top' OR 'bottom'
    align = ['left', 'center', 'right']; // 'left' OR 'center' OR 'right'
    timer = 100;

    /* Message can be HTML text */
    default(type:string, message: string, from: string, align: string) {
    	$.notify({
        	icon: "ti-info",
        	message: message
        },{
            type: type,
            timer: this.timer,
            placement: {
                from: from,
                align: align
            }
        });
    }

    info(message: string) {
        this.default(this.type[1], message, this.from[0], this.align[2]);
    }

    success(message: string) {
        this.default(this.type[2], message, this.from[0], this.align[2]);
    }

    warning(message: string) {
        this.default(this.type[3], message, this.from[0], this.align[2]);
    }

    danger(message: string) {
        this.default(this.type[4], message, this.from[0], this.align[2]);
    }

    userOnline(message: string) {
        this.default(this.type[1], message, this.from[1], this.align[0]);
    }
}