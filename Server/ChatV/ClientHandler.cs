using System.Net.Sockets;
using System.Text;

public class ClientHandler
{
    private Server server;
    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;

    private string clientEndPoint;

    public ClientHandler(Server server, TcpClient client)
    {
        this.server = server;
        this.client = client;

        try
        {
            stream = client.GetStream();

            Socket socket = client.Client;

            clientEndPoint = $"{socket.RemoteEndPoint}";

            Logger.Log($"Connected Client : {clientEndPoint}");

            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8);

        }
        catch (Exception exception)
        {
            Logger.Log("Connection Failed.");
            Logger.Log(exception.Message);
        }
    }

    public void Start()
    {
        ThreadStart threadStart = new ThreadStart(ClientThread);
        Thread thread = new Thread(threadStart);

        thread.Start();
    }

    private void ClientThread()
    {
        try
        {
            string? message;

            while ((message = reader.ReadLine()) != null)
            {
                server.OnReceiveClientMessage(message);
            }
        }
        catch (Exception exception)
        {
            Logger.Log($"Disconnected Client : {clientEndPoint}");
            Logger.Log(exception.Message);
        }
        finally
        {
            server.OnClientDisconnect(this);
            reader.Close();
            writer.Close();
            client.Close();
        }
    }

    public void SendMessage(string message)
    {
        writer.WriteLine(message);
        writer.Flush();
    }
}
