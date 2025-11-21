import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { AccountService } from '@core/services/account-service';
import { Nav } from '@layout/nav/nav';
import { Home } from "@features/home/home";

// import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    // imports: [RouterOutlet],
    templateUrl: './app.html',
    styleUrl: './app.css',
    imports: [Nav, Home]
})
export class App implements OnInit {
    private http = inject(HttpClient);
    private accountService = inject(AccountService);

    protected readonly title = 'Dating App';
    // option I: needs zone.js
    // protected members: any;

    // option II: zoneless
    protected members = signal<any>([]);

    // option I
    // ngOnInit(): void {
    //     this.http.get('https://localhost:5001/api/members').subscribe({
    //         next: (response) => this.members = response,
    //         error: (error) => console.error(error),
    //         complete: () => console.log('Completed the http request')
    //     });
    // }

    // option II
    async ngOnInit() {
        this.setCurrentUser();
        this.members.set(await this.getMembers());
    }

    setCurrentUser() {
        const userString = localStorage.getItem('user');
        if (!userString) {
            return;
        }

        const user = JSON.parse(userString);
        this.accountService.currentUser.set(user);
    }

    async getMembers() {
        try {
            return lastValueFrom(
                this.http.get('https://localhost:5001/api/members')
            );
        } catch (error) {
            console.error(error);
            return Promise.reject(error);
        }
    }
}
