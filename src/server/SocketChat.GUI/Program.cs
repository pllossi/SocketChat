using System.Runtime.InteropServices;
using SocketChat.Core;

namespace SocketChat.GUI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = "";
        var port = 0;
        if (args.Length == 0)
        {
            host = "localhost";
            port = 11000;
        } else
        {
            host = args[0];
            port = int.Parse(args[1]);
        }

        var server = new Server(host,port);
        server.Start();

        while (true)
        {
            server.StartListeningAsync((message) => Console.WriteLine("[From client] " + message));
        }    
    }
}