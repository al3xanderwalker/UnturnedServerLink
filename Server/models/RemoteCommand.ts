export class RemoteCommand {
  Sender: string;
  Command: string;

  constructor(sender: string, command: string) {
    this.Sender = sender;
    this.Command = command;
  }
}
