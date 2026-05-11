using SalonCRM.Client;

var id = args.Length > 0 && int.TryParse(args[0], out var parsed) ? parsed : 1;
var client = new SalonClient(id);
client.Run();
