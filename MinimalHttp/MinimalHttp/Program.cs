#if DEBUG

using System;

using MinimalHttp.Client;

namespace MinimalHttp
{
    class Program
    {
        public static void Main(string[] args)
        {
            HttpClient client = new HttpClient();

            client.CertificateValidationCallback = (HttpCertificate certificate) =>
            {
                if (certificate == null) return false;

                return certificate.ValidatePeriod();
            };

            try
            {
                var response = client.Get("https://google.com");
            }
            catch(SslValidationException ssl_exception)
            {
                Console.WriteLine(ssl_exception.ToString());
            }
            

            Console.WriteLine("done");

            Console.ReadLine();
        }
    }
}

#endif