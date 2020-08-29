import { Component, OnInit } from '@angular/core';
import { ChatService } from '../_services/chat.service';
import { ChatMessage } from '../_models/ChatMessage';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {

  constructor(private chatService: ChatService) { }

  ngOnInit(): void {
    this.chatService.retrieveMappedObject().subscribe((receivedObj: ChatMessage) => { this.addToInbox(receivedObj); });  // calls the service method to get the new messages sent

  }

  msgDto: ChatMessage = new ChatMessage();
  msgInboxArray: ChatMessage[] = [];

  send(): void {
    if (this.msgDto) {
      if (this.msgDto.user.length == 0 || this.msgDto.user.length == 0) {
        window.alert("Both fields are required.");
        return;
      } else {
        this.chatService.broadcastMessage(this.msgDto);                   // Send the message via a service
      }
    }
  }

  addToInbox(obj: ChatMessage) {
    let newObj = new ChatMessage();
    newObj.user = obj.user;
    newObj.msgText = obj.msgText;
    this.msgInboxArray.push(newObj);

  }
}
