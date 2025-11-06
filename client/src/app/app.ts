import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';
// import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    // imports: [RouterOutlet],
    templateUrl: './app.html',
    styleUrl: './app.css'
})
export class App implements OnInit {
    private http = inject(HttpClient);

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
        this.members.set(await this.getMembers())
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
