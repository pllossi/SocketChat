using System.Runtime.InteropServices;
using SocketChat.Core;
using System.Text;

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

        var client = new Client(host,port);
        client.Connect();

        if (!client.IsConnected)
        {
            Console.Error.WriteLine("Connection Unsucceful!");
            Environment.Exit(1);
        }

        client.StartListeningAsync((message) => Console.WriteLine(message));

        bool keepGoing = true;

        while(keepGoing && client.IsConnected)
        {
            var message = Console.ReadLine();
            if (!string.IsNullOrEmpty(message))
            {
                bool resp = client.TrySendMessage(message);
                if (resp)
                {
                    byte[] raw = Encoding.UTF8.GetBytes(message);
                    Console.WriteLine(message);
                    string byt = "";
                    foreach(var value in raw)
                    {
                        byt += value;
                    }
                    Console.WriteLine("Bytes : " + byt);
                }
            }
            keepGoing = message != "exit";
        }
    }
}