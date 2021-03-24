export class SocketData {
  Event: string;
  Data: string;

  constructor(event: string, data: any) {
    this.Event = event;
    if (typeof data == 'string') this.Data = data;
    else this.Data = JSON.stringify(data);
  }
}
