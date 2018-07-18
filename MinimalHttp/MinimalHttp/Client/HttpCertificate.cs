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

        public bool ValidatePublicKey(byte[] key)
        {
            if (key == null || key.Length == 0) throw new ArgumentNullException(nameof(key));

            return CompareBytes(this.GetPublicKey(), key);
        }

        private bool CompareBytes(byte[] first, byte[] second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            if (first.Length != second.Length) return false;

            for(int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i]) return false;
            }

            return true;
        }

        public override bool Equals(X509Certificate other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return CompareBytes(this.GetRawCertData(), other.GetRawCertData());
        }

        public bool Equals(HttpCertificate other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return CompareBytes(this.GetRawCertData(), other.GetRawCertData());
        }

        public bool Equals(byte[] other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return CompareBytes(this.GetRawCertData(), other);
        }
    }
}
