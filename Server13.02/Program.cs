using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;

class CurrencyExchangeServer
{
    static readonly Dictionary<string, double> exchangeRates = new Dictionary<string, double>
    {
        {"USD_EURO", 0.92},
        {"EURO_USD", 1.09},
        {"USD_UAH", 38.50},
        {"UAH_USD", 0.026},
        {"EURO_UAH", 41.80},
        {"UAH_EURO", 0.024}
    };

    static void Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Сервер запущено. Очікування підключень...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
        Console.WriteLine($"Клієнт підключився: {endPoint.Address}:{endPoint.Port}");

        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim().ToUpper();
                Console.WriteLine($"Отримано запит: {request}");

                string response = exchangeRates.TryGetValue(request, out double rate)
                    ? rate.ToString()
                    : "Некоректний запит";

                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);
                Console.WriteLine($"Відправлено відповідь: {response}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine("Клієнт відключився");
        }
    }
}
