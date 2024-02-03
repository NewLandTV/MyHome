using System.Net.Sockets;
using System.Text;

class Program
{
    private static string serverIP = "127.0.0.1";
    private static ushort serverPort = 9055;
    private static string userName;

    private static TcpClient client;
    private static NetworkStream stream;
    private static StreamReader reader;
    private static StreamWriter writer;

    public static void Main()
    {
        try
        {
            userName = Console.ReadLine().Trim();

            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8);

            ThreadStart receiveThreadStart = new ThreadStart(ReceiveThread);
            Thread receiveThread = new Thread(receiveThreadStart);

            receiveThread.IsBackground = true;

            receiveThread.Start();

            while (client.Connected)
            {
                string? message = Console.ReadLine();

                if (message != null)
                {
                    // Command
                    if (message.Equals("/Quit"))
                    {
                        Disconnect();
                    }

                    SendMessage($"{userName} : {message}");
                }
            }
        }
        catch
        {
            Console.WriteLine("Connection Failed.");
        }
    }

    private static void ReceiveThread()
    {
        string? message;

        try
        {
            bool canReceive = client.Connected && reader != null;

            if (canReceive)
            {
                while ((message = reader.ReadLine()) != null)
                {
                    Console.WriteLine(message);
                }
            }
        }
        catch
        {
            Console.WriteLine("Receive Error.");
        }
    }

    private static void SendMessage(string message)
    {
        if (writer == null)
        {
            return;
        }

        try
        {
            writer.WriteLine(message);
            writer.Flush();
        }
        catch
        {
            Console.WriteLine("Failed to send message.");
        }
    }

    private static void Disconnect()
    {
        SendMessage($"{userName} left the room.");
        reader.Close();
        writer.Close();
        stream.Close();
    }
}
