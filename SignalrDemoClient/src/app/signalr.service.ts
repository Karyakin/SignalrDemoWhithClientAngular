import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import {ToastrService} from "ngx-toastr";
import {Router} from "@angular/router";
import {Observable, Subject} from "rxjs";

export class User{
  public id: string;
  public name: string;
  public connId: string;
  public msgs: Array<Message>;
}


export class Message{
  constructor(
    public content:string,
    public mine:boolean
  ){}
}


@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor(public toastr: ToastrService,
              public router: Router) { }

  hubConnection:signalR.HubConnection;
  userData: User;

  ssSubj = new Subject<any>();
  ssObs(): Observable<any> {
    return this.ssSubj.asObservable();
  }
  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/toastr', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.ssSubj.next({type: "HubConnStarted"});
      })
      .catch(err => console.log('Error while starting connection: ' + err))
  }
}
