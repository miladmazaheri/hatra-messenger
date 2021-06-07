import { Component, Injector, ChangeDetectionStrategy, OnInit, OnDestroy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { SignalRService } from './signal-r.service'
import { Subscription } from 'rxjs';
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
  MesssagesData:string='';
  ngOnInit(): void {
    this.signalRService.startConnection();
    this.subscription = this.signalRService.messageSource$.subscribe(x=>{
      if(x){
        this.MesssagesData+=x+'\n'
      }
    });
  }

  sendMessage() {
    if (this.messageText) {
      this.signalRService.sendMessage(this.messageText)
      this.messageText = null;
    }
  }


  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
