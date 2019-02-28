import { Directive, forwardRef } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

@Directive({
    selector: '[emailValid]',
    providers: [{ 
        provide: NG_VALIDATORS, 
        useExisting: forwardRef(() => EmailValidator), 
        multi: true 
    }]
})

export class EmailValidator implements Validator {
    
    constructor(){ }
    
    validate(c: AbstractControl) {
        var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/g;
        if(!emailRegex.test(c.value))
            return {emailValid: false}

        return null;

    }

}
