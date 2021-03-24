export class ChatMessage {
  Sender: string;
  SenderId: string;
  Content: string;
  Icon: string;

  constructor(
    sender?: string,
    senderId?: string,
    content?: string,
    icon?: string
  ) {
    this.Sender = sender;
    this.SenderId = senderId;
    this.Content = content;
    this.Icon = icon;
  }
}
