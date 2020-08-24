import { Component, OnInit, Input } from '@angular/core';
import { Message } from '../../_models/Message';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { tap } from 'rxjs/operators';
import { CursorError } from '@angular/compiler/src/ml_parser/lexer';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recepientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService,
    private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recepientId)
      .pipe(
        tap(messages => {
          for (let i = 0; i < messages.length; i++) {
            if (messages[i].isRead === false && messages[i].recepientId === currentUserId) {
              this.userService.markAsRead(currentUserId, messages[i].id);
            }
          }
        })
      )
      .subscribe(messages => {
        this.messages = messages;
      }, error => {
          this.alertify.error(error);
      });
  }

  sendMessage() {
    this.newMessage.recepientId = this.recepientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe((message: Message) => {
        this.messages.unshift(message);
        console.log(message);
        this.newMessage.content = '';
      }, error => {
          this.alertify.error(error);
      });
  }
}
