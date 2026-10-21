using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketChat.Core;

public class Server
{
    public int Port
    {
        get; 
        private set;
    }
    public string HostName { 
        get; 
        private set; 
    }
    private Socket listener;
    private Socket clientHandeler;
    private byte[] buffer;
    public bool IsClientConnected => clientHandeler is not null && clientHandeler.Connected;

    public Server(string hostName,int port)
    {
        Port = port;
        HostName = hostName;
    }

    public void Start()
    {
        var hostIp = Dns.GetHostEntry(HostName).AddressList.First();
        var localendpoint = new IPEndPoint(hostIp,Port);
        listener = new Socket(
            hostIp.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        listener.Bind(localendpoint);
        listener.Listen(10);

        clientHandeler = listener.Accept();

    }

    public void StartListeningAsync(Action<string> onMessageRecived)
    {
        Task.Run(() =>
        {
            this.buffer = new byte[1024];
            while (IsClientConnected)
            {
                int recivedBytes = this.clientHandeler.Receive(this.buffer);
                if (recivedBytes == 0)
                {
                    return;
                }
                string decodedMSG = Encoding.UTF8.GetString(this.buffer,0,recivedBytes);
                onMessageRecived(decodedMSG);
            }
        });        
    }
    public bool TrySendMessage(string msg)
    {
        if (!IsClientConnected)
        {
            return false;
        }
        byte[] raw = Encoding.UTF8.GetBytes(msg);
        int sent = clientHandeler.Send(raw);
        if(sent == raw.Length)
        {
           return true; 
        }
        return false;
    }
}
