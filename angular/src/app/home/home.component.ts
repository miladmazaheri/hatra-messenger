import { Component, Injector, ChangeDetectionStrategy, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { IMessageDto, MessageDto, SignalRService } from './signal-r.service'
import { Subscription } from 'rxjs';
import { NgControl } from '@angular/forms';
@Component({
  templateUrl: './home.component.html',
  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent extends AppComponentBase implements OnInit,OnDestroy {
  constructor(public signalRService: SignalRService, injector: Injector) {
    super(injector);
  }
  subscription:Subscription;
 
  messageText: string;
  MesssagesData:MessageDto[]=[];
  ngOnInit(): void {
    
  }

  connectToHub(){
    this.signalRService.startConnection();
    this.subscription = this.signalRService.messageSource$.subscribe(x=>{
      if(x){
        x.IsSelf = this.appSession.user.userName == x.Sender;
        this.MesssagesData.push(x);
      }
    });
  }

  sendMessage() {
    if (this.messageText) {
      this.signalRService.sendMessage(this.messageText)
      this.messageText = null;
    }
  }
  sendPrivateMessage() {
    if (this.messageText) {
      this.signalRService.sendPrivateMessage(this.messageText)
      this.messageText = null;
    }
  }


  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
