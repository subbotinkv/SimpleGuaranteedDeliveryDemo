using System;
using System.IO;

using Microsoft.Owin.Hosting;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>(Config.Url))
            {
                Console.WriteLine($"Server is running on {Config.Url}");

                Console.WriteLine("Enter 'print' to print all recieved messages or type 'exit' to quit...");

                string command = Console.ReadLine();
                while (command != "exit")
                {
                    if (command == "print")
                    {
                        string messages = null;

                        if (File.Exists(Config.StorageFileName))
                        {
                            messages = File.ReadAllText(Config.StorageFileName);
                        }

                        Console.WriteLine(!string.IsNullOrEmpty(messages) ? messages : "You have no messages yet...");
                    }
                    else
                    {
                        Console.WriteLine("Unknown command, please try again...");
                    }

                    command = Console.ReadLine();
                }
            }
        }
    }
}
