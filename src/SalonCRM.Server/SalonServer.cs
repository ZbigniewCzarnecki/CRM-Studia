using System.Net;
using System.Net.Sockets;

namespace SalonCRM.Server;

public class SalonServer
{
    public const int MaxClients = 3;
    private const int DefaultPort = 5000;

    private readonly int _port;
    private readonly DataStore _dataStore = new();
    private readonly SemaphoreSlim _semaphore = new(MaxClients, MaxClients);
    private int _clientIdCounter = 0;

    public SalonServer(int port = DefaultPort)
    {
        _port = port;
    }

    public void Start()
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        Console.WriteLine($"[Serwer] Nasłuchuje na porcie {_port}, limit klientów: {MaxClients}");

        while (true)
        {
            var tcpClient = listener.AcceptTcpClient();
            var clientId = Interlocked.Increment(ref _clientIdCounter);
            var handler = new ClientHandler(tcpClient, _dataStore, _semaphore, clientId);
            var thread = new Thread(handler.Handle) { IsBackground = true };
            thread.Start();
        }
    }
}
