import {SignalrService, User} from 'src/app/signalr.service';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';


@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(
    public signalrService: SignalrService,
    public router: Router
  ) {
    let tempPersonId = localStorage.getItem("personId");
    if (tempPersonId) {
      if (this.signalrService.hubConnection) { //if already connected
        this.reauthMeListener();
        this.reauthMe(tempPersonId);
      }
      else {
        this.signalrService.ssObs().subscribe((obj: any) => {
          if (obj.type == "HubConnStarted") {
            this.reauthMeListener();
            this.reauthMe((tempPersonId || '').toString());
          }
        });
      }
    }
  }


  public isAuthenticated: boolean = false;


  //2Tutorial
  async authMe(person: string, pass: string) {
    let personInfo = {userName: person, password: pass};

    await this.signalrService.hubConnection.invoke("authMe", personInfo)
      .then(() => this.signalrService.toastr.info("Loging in attempt..."))
      .catch(err => console.error(err));
  }


  //3Tutorial
  authMeListenerSuccess() {
    this.signalrService.hubConnection.on("authMeResponseSuccess", (user:User) => {
      console.log(user);
      this.signalrService.userData = {...user};
      localStorage.setItem("personId", user.id);
      this.isAuthenticated = true;
      this.signalrService.toastr.success("Login successful!");
      this.signalrService.router.navigateByUrl("/home");
    });
  }

  //2Tutorial
  authMeListenerFail() {
    this.signalrService.hubConnection.on("authMeResponseFail", () => {
      this.signalrService.toastr.error("Wrong credentials!");
    });
  }


  //3Tutorial
  async reauthMe(personId: string) {
    await this.signalrService.hubConnection.invoke("reauthMe", personId)
      .then(() => this.signalrService.toastr.info("Loging in attempt..."))
      .catch(err => console.error(err));
  }




  //3Tutorial
  reauthMeListener() {
    this.signalrService.hubConnection.on("reauthMeResponse", (user:User) => {
      console.log(user);
      this.signalrService.userData = {...user};
      this.isAuthenticated = true;
      this.signalrService.toastr.success("Re-authenticated!");
      if (this.signalrService.router.url == "/auth") this.signalrService.router.navigateByUrl("/home");
    });
  }



}
