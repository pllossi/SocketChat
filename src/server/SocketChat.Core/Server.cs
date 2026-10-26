using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualBasic;

namespace SocketChat.Core;

public class Server
{
    public int Port
    {
        get;
        private set;
    }
    public string HostName
    {
        get;
        private set;
    }
    private Socket listener;
    private List<Socket> clientHandeler;
    private byte[] buffer;
    public List<bool> IsClientConnected()
    {
        var resp = new List<bool>();
        if (clientHandeler is null)
        {
            resp.Add(false);
        }
        else
        {
            foreach (var recv in clientHandeler)
            {
                resp.Add(recv.Connected);
            }
        }
        return resp;
    }
    public Server(string hostName, int port)
    {
        Port = port;
        HostName = hostName;
    }

    public void Start()
    {
        var hostIp = Dns.GetHostEntry(HostName).AddressList.First();
        var localendpoint = new IPEndPoint(hostIp, Port);
        listener = new Socket(
            hostIp.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        listener.Bind(localendpoint);
        listener.Listen(10);

        clientHandeler.Add(listener.Accept());

    }

    public void StartListeningAsync(Action<string> onMessageRecived)
    {
        for (int i = 0; i < clientHandeler.Count; i++)
        {
            var client = clientHandeler[i];
            Task.Run(() =>
            {
                this.buffer = new byte[1024];
                while (client.Connected)
                {
                    int recivedBytes = client.Receive(this.buffer);
                    if (recivedBytes == 0)
                    {
                        return;
                    }
                    string decodedMSG = Encoding.UTF8.GetString(this.buffer, 0, recivedBytes);
                    onMessageRecived(decodedMSG);
                    sendMessageToOthers(decodedMSG, client);
                }
            });
        }
    }
    /*
    public bool TrySendMessage(string msg)
    {
        if (!IsClientConnected)
        {
            return false;
        }
        byte[] raw = Encoding.UTF8.GetBytes(msg);
        int sent = clientHandeler.Send(raw);
        if (sent == raw.Length)
        {
            return true;
        }
        return false;
    }*/

    private void sendMessageToOthers(string msg, Socket sender)
    {
        foreach (var recv in clientHandeler)
        {
            if (recv is null)
            {
                break;
            }
            if (recv == sender)
            {
                break;
            }
            byte[] raw = Encoding.UTF8.GetBytes(msg);
            int sent = recv.Send(raw);
            while (sent != raw.Length)
            {

            }
        }
    }
}
