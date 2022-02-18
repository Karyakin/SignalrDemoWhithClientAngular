import {Component, OnDestroy, OnInit} from '@angular/core';
import {SignalrService} from "./signalr.service";
import {AuthService} from "./auth/auth.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy{

  constructor(
    public signalrService: SignalrService,
    public authService: AuthService
  )
  {}


 ngOnInit() {
    this.signalrService.startConnection();

    /*setTimeout(() => {
      this.signalrService.askServerListener();
      this.signalrService.askServer();
    }, 2000);*/
 }


 ngOnDestroy() {
    this.signalrService.hubConnection.off("askServerResponse");
 }
}
