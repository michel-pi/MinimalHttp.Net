using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MinimalHttp.Client
{
    public class HttpCertificate : X509Certificate2
    {
        public HttpCertificate(X509Certificate cert) : base(new X509Certificate2(cert))
        {

        }

        public bool ValidatePeriod()
        {
            var start = this.NotBefore;
            var end = this.NotAfter;

            var cur = DateTime.Now;

            if (cur < start) return false;
            if (cur > end) return false;

            return true;
        }
    }
}
