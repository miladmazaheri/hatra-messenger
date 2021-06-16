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
  private _messageSource = new BehaviorSubject<MessageDto>(null);
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
      .withUrl(AppConsts.remoteServiceBaseUrl + '/chat-hub', { accessTokenFactory: () => authToken })
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
      this._messageSource.next(MessageDto.fromJS(data));
    });
    this.hubConnection.on('privateMessage', (data) => {
      console.log(data);
    });
    console.log('Listeners Initialized!');
  }

  public sendMessage = (message: any) => {
    this.hubConnection.invoke("SendMessage", message)
  }
  public sendPrivateMessage = (message: any) => {
    this.hubConnection.invoke("SendPrivateMessage",2,'1FCE8D21-1DCC-41B5-8D50-50D93D97901C', message)
  }
}


export interface IMessageDto {
  Sender: string | undefined;
  Content: string | undefined;
  IsSelf:boolean |false;
}
export class MessageDto implements IMessageDto {
  Sender: string;
  Content: string;
  IsSelf:boolean;
  MessageDto(obj:IMessageDto) {
    this.Sender = obj.Sender;
    this.Content = obj.Content;
  }


  static fromJS(jsonStr: string): MessageDto {
    var obj = JSON.parse(jsonStr) as MessageDto;
    return obj;
  }
}
