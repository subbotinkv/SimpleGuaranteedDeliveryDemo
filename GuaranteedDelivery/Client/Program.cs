using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private const string DefaultUrl = "http://localhost:9000/";

        private const string Ext = ".msg";

        private const int BackoffTimeout = 1000;

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the service URL:");
            Console.Write($"[{DefaultUrl}]: ");
            string url = Console.ReadLine();
            if (string.IsNullOrEmpty(url))
            {
                url = DefaultUrl;
            }

            // Запускаем в фоне "обработчик сообщений" (MessageProcessor)
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    string fileName = Directory.GetFiles(".", $"*{Ext}").FirstOrDefault();
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        bool delivered = true;
                        try
                        {
                            HttpClient client = new HttpClient();
                            HttpResponseMessage result = client.PostAsJsonAsync(url + "/api/lines", File.ReadAllText(fileName)).Result;
                            result.EnsureSuccessStatusCode();
                        }
                        catch (Exception)
                        {
                            delivered = false;

                            // Добавим задержку на случай если ошибка была временной.
                            Thread.Sleep(BackoffTimeout);
                        }

                        if (delivered)
                        {
                            // После успешной доставки, удаляем "сообщение" из "хранилища".
                            File.Delete(fileName);
                        }
                    }
                }
            });


            Console.WriteLine("Enter message or type 'exit' to quit...");

            string message = Console.ReadLine();
            while (message != "exit")
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    // Записываем "сообщение" в "хранилище" (MessageStore).
                    File.WriteAllText($"{Guid.NewGuid()}{Ext}", message);
                }

                message = Console.ReadLine();
            }
        }
    }
}
