import { Directive, forwardRef } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

@Directive({
    selector: '[greaterThanZero]',
    providers: [{ 
        provide: NG_VALIDATORS, 
        useExisting: forwardRef(() => GreaterThanZeroValidator), 
        multi: true 
    }]
})

export class GreaterThanZeroValidator implements Validator {
    
    constructor(){ }
    
    validate(c: AbstractControl) {
        if(c.value < 1)
            return { greaterThanZero: false }
        return null;
    }

}
