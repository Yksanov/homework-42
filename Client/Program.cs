using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

class Program
{
    static void Main(string[] args)
    {
        RunClient();
    }

    static void RunClient()
    {
        try
        {
            while (ClientWorker()) { }
            Console.WriteLine("Client stopped!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    static bool ClientWorker()
    {
        IPAddress ipAddress = IPAddress.Loopback; // localhost or 127.0.0.1 - наш компьютер
        IPEndPoint ipep = new IPEndPoint(ipAddress, 8000);
        using Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        sender.Connect(ipep);
        Console.WriteLine($"You connected to {sender.RemoteEndPoint}");
        Console.WriteLine("Enter your message:");
        string message = Console.ReadLine();
        byte[] sendingBytes = Encoding.UTF8.GetBytes(message);
        sender.Send(sendingBytes);
        byte[] recvbytes = new byte[1024];
        int bytesReceived = sender.Receive(recvbytes);
        string response = Encoding.UTF8.GetString(recvbytes, 0, bytesReceived);
        Console.WriteLine(response);
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
        return !message.Contains("Stop");
    }
}