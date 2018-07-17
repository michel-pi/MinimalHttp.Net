using System;

using MinimalHttp.Client;

namespace MinimalHttp
{
    class Program
    {
        public static void Main(string[] args)
        {
            HttpClient client = new HttpClient();

            var response = client.Get("https://chromacheats.com");

            Console.ReadLine();
        }
    }
}
