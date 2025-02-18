using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UDPClient
{
    static void Main()
    {
        Socket clientSocket = null;
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 5000));

            Console.Write("Введіть текст: ");
            string message = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("Виявлено порожнє повідомлення. Припинення зв’язку.");
                return;
            }

            byte[] messageData = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(messageData);

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

            Console.WriteLine($"Відповідь сервера: {response}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Помилка сокета: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сталася помилка: {ex.Message}");
        }
        finally
        {
            if (clientSocket != null)
            {
                try
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при закритті сокета: {ex.Message}");
                }
            }
        }
    }
}
