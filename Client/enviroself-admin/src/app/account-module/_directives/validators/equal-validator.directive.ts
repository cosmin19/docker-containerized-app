import { Directive, forwardRef, Attribute } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

/* 
    First, we define directive using the @Directive annotation. Then we specify the selector. Selector is mandatory. 
    We will extend the built-in validators NG_VALIDATORS to use our equal validator in providers.
*/
@Directive({
    selector: '[validateEqual][formControlName],[validateEqual][formControl],[validateEqual][ngModel]',
    providers: [
        { provide: NG_VALIDATORS, useExisting: forwardRef(() => EqualValidator), multi: true }
    ]
})

/* 
    Our directive class should implement the Validator interface. Validator interface expecting a validate function. 
    In our constructor, we inject the attribute value via annotation @Attribute(‘validateEqual’) and assign it to the validateEqual variable. 
    In our example, the value of validateEqual would be “password”.
*/
export class EqualValidator implements Validator {
    constructor(@Attribute('validateEqual') public validateEqual: string,
    @Attribute('reverse') public reverse: string) {
    }

    private get isReverse() {
        if (!this.reverse) return false;
        return this.reverse === 'true' ? true: false;
    }

/*
    First, we read the value of our input and assign it to v. Then, we find the password input control in our form and assign it to e. 
    After that, we check for value equality, and return errors if it’s not equal.
*/

    validate(c: AbstractControl): { [key: string]: any } {
        // self value
        let v = c.value;

        // control vlaue
        let e = c.root.get(this.validateEqual);

        // value not equal
        if (e && v !== e.value && !this.isReverse) {
            return {
                validateEqual: false
            }
        }

        // value equal and reverse
        if (e && v === e.value && this.isReverse) {
            delete e.errors['validateEqual'];
            if (!Object.keys(e.errors).length) e.setErrors(null);
        }

        // value not equal and reverse
        if (e && v !== e.value && this.isReverse) {
            e.setErrors({ validateEqual: false });
        }

        return null;
    }
}
