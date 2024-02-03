using System.Collections;
using System.Net;
using System.Net.Sockets;

public class Server
{
    private TcpListener listener;
    private ArrayList clientList;

    public Server(int maxClientCount)
    {
        clientList = new ArrayList(maxClientCount);
    }

    public void Start(ushort port)
    {
        clientList.Clear();

        try
        {
            listener = new TcpListener(IPAddress.Any, port);

            listener.Start();

            Logger.Log("The server has started!");

            AcceptLoop();
        }
        catch (Exception exception)
        {
            Logger.Log(exception.Message);
        }
    }

    private void AcceptLoop()
    {
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            ClientHandler handler = new ClientHandler(this, client);

            lock (clientList.SyncRoot)
            {
                clientList.Add(handler);
            }

            handler.Start();
        }
    }

    public void OnReceiveClientMessage(string message)
    {
        lock (clientList.SyncRoot)
        {
            Logger.Log(message);

            // Send message to connected clients.
            for (int i = clientList.Count - 1; i >= 0; i--)
            {
                ClientHandler? handler = clientList[i] as ClientHandler;

                if (handler != null)
                {
                    handler.SendMessage(message);
                }
            }
        }
    }

    public void OnClientDisconnect(ClientHandler handler)
    {
        lock (clientList.SyncRoot)
        {
            clientList.Remove(handler);
        }
    }
}
