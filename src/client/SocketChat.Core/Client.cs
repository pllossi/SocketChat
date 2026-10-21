using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketChat.Core;

public class Client
{
    private Socket commChannel;
    public int Port
    {
        get; 
        private set;
    }
    public string HostName { 
        get; 
        private set; 
    }

    public bool IsConnected => 
        commChannel is not null 
        && commChannel.Connected;
    
    private byte[] buffer;

    public Client(string hostName,int port)
    {
        Port = port;
        HostName = hostName;
    }

    public void Connect()
    {
        var hostIP = Dns.GetHostEntry(HostName).AddressList.First();
        var remoteEndpoint = new IPEndPoint(hostIP,Port);
        this.commChannel =  new Socket(
            hostIP.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );
        this.commChannel.Connect(remoteEndpoint);
    }
    public bool TrySendMessage(string msg)
    {
        if (!IsConnected)
        {
            return false;
        }
        byte[] raw = Encoding.UTF8.GetBytes(msg);
        int sent = commChannel.Send(raw);
        if(sent == raw.Length)
        {
           return true; 
        }
        return false;
    }
    public void StartListeningAsync(Action<string> onMessageRecived)
    {
        Task.Run(() =>
        {
            this.buffer = new byte[1024];
            while (IsConnected)
            {
                int recivedBytes = this.commChannel.Receive(this.buffer);
                if (recivedBytes == 0)
                {
                    return;
                }
                string decodedMSG = Encoding.UTF8.GetString(this.buffer,0,recivedBytes);
                onMessageRecived(decodedMSG);
            }
        });        
    }
}
