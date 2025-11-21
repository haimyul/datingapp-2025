import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '@core/services/account-service';
import { RegisterCreds } from '@interfaces/user';

@Component({
    selector: 'app-register',
    imports: [FormsModule],
    templateUrl: './register.html',
    styleUrl: './register.css'
})
export class Register {
    private accountService = inject(AccountService);
    protected creds = {} as RegisterCreds;
    registerCancelled = output<boolean>();

    // validCreds() {
    //     const creds = Object.values(this.creds);
    //     return creds.length && creds.every((value) => value);
    // }

    register() {
        this.accountService.register(this.creds).subscribe({
            next: (response) => {
                console.log(response);
                this.cancel();
            },
            error: (error) => console.error(error)
        });
    }

    cancel() {
        this.registerCancelled.emit(false);
    }
}
