import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { IHttpConnectionOptions } from '@aspnet/signalr';
import { AppConsts } from '@shared/AppConsts';
import { UtilsService } from 'abp-ng2-module';
import { BehaviorSubject } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private hubConnection: signalR.HubConnection
  private _messageSource = new BehaviorSubject<string>('');
  messageSource$ = this._messageSource.asObservable();

  public startConnection = () => {
    const authToken = new UtilsService().getCookieValue("Abp.AuthToken");
    const options: IHttpConnectionOptions = {
      accessTokenFactory: () => {
        return authToken;
      }
    };
    this.hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(AppConsts.remoteServiceBaseUrl + '/chat-hub',{ accessTokenFactory: () => authToken })
      // .withUrl(AppConsts.appBaseUrl + '/chat-hub', options)
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.initialListeners();
      })
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  private initialListeners() {
    this.hubConnection.on('getMessage', (data) => {
      console.log(data);
      this._messageSource.next(data);
    });


    console.log('Listeners Initialized!');
  }

  public sendMessage = (message: any) => {
    this.hubConnection.invoke("SendMessage", message)
  }
}
