using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        RunServer();
    }

    static void RunServer()
    {
        try
        {
            ServerWorker();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void ServerWorker()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipep = new IPEndPoint(ipAddress, 8000);
        using Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(ipep);
        listener.Listen(10);
        while (true)
        {
            Console.WriteLine("Waiting for a connection...");
            using Socket handler = listener.Accept();

            Thread myThread = new Thread(listener.Listen);
            
            Console.WriteLine("Clients connected!");
            byte[] receiveBuffer = new byte[1024];
            int bytesCount = handler.Receive(receiveBuffer);
            string receiveMessage = Encoding.UTF8.GetString(receiveBuffer, 0, bytesCount);
            Console.WriteLine(receiveMessage);
            string response = "".Trim();
            if (receiveMessage.Contains("Привет"))
            {
                response = "Привет с сервера!";
            }
            else if (receiveMessage.Contains("Сколько время?"))
            {
                response = DateTime.Now.ToLongTimeString();
            }
            else if (receiveMessage.Contains("Какое число сегодня?"))
            {
                response = DateTime.Today.ToShortDateString();
            }
            else if (receiveMessage.Contains("IP адрес сервера?"))
            {
                response = ipAddress.ToString();
            }
            else if (receiveMessage.Contains("Ты где находишься?"))
            {
                response = "Городе Бишкек";
            }
            else if (receiveMessage.Contains("Порт"))
            {
                response = ipep.Port.ToString();
            }
            else if(receiveMessage.Contains("random"))
            {
                response = new Random().Next(1, 21).ToString();
            }
            else
            {
                response = $"Length of your message: {receiveMessage.Length}";
                response = "Непонятный вопрос!";
            }

            response = $"Ответ: {response}";
            byte[] sendBuffer = Encoding.UTF8.GetBytes(response);
            handler.Send(sendBuffer);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            
            myThread.Start();
            
            if (receiveMessage.Contains("Stop")) 
                break;
        }
        Console.WriteLine("Server stopped!");
    }
}