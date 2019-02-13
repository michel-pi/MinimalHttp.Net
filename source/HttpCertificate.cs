using System;
using System.Security.Cryptography.X509Certificates;

namespace MinimalHttp
{
    /// <summary>
    /// Extends the X509Certificate2
    /// </summary>
    public class HttpCertificate : X509Certificate2
    {
        /// <summary>
        /// Gets a Boolean indicating whether this certificate is already expired.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                var start = NotBefore;
                var end = NotAfter;

                var cur = DateTime.Now;

                if (cur < start) return true;

                return cur > end;
            }
        }

        /// <summary>
        /// Initializes a new HttpCertificate using the given X509Certificate.
        /// </summary>
        /// <param name="certificate">A X509Certificate.</param>
        public HttpCertificate(X509Certificate certificate) : base(new X509Certificate2(certificate))
        {
        }

        /// <summary>
        /// Compares the bytes of the public key of this instance with another byte[].
        /// </summary>
        /// <param name="key">The byte[] used to compare our public key.</param>
        /// <returns>A Boolean indicating whether the public key was equal.</returns>
        public bool ComparePublicKey(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key.Length == 0) throw new ArgumentOutOfRangeException(nameof(key));

            return CompareBytes(GetPublicKey(), key);
        }

        /// <summary>
        /// Compares a HttpCertificate with a X509Certificate.
        /// </summary>
        /// <param name="other">The X509Certificate to compare with.</param>
        /// <returns>A Boolean indicating whether these X509Certificate were equal.</returns>
        public override bool Equals(X509Certificate other)
        {
            return other != null && CompareBytes(GetRawCertData(), other.GetRawCertData());
        }

        /// <summary>
        /// Compares a HttpCertificate with a HttpCertificate.
        /// </summary>
        /// <param name="other">The HttpCertificate to compare with.</param>
        /// <returns>A Boolean indicating whether these HttpCertificate were equal.</returns>
        public bool Equals(HttpCertificate other)
        {
            return other != null && CompareBytes(GetRawCertData(), other.GetRawCertData());
        }

        /// <summary>
        /// Compares a HttpCertificate with the raw bytes of another X509Certificate.
        /// </summary>
        /// <param name="other">The raw bytes of another X509Certificate.</param>
        /// <returns>A Boolean indicating whether this HttpCertificate equals the other raw certificate bytes.</returns>
        public bool Equals(byte[] other)
        {
            return other != null && CompareBytes(GetRawCertData(), other);
        }

        private static bool CompareBytes(byte[] first, byte[] second)
        {
            if (first == null || second == null)
                return false;

            if (first.Length != second.Length) return false;

            for (int i = 0; i < first.Length; i++)
                if (first[i] != second[i]) return false;

            return true;
        }
    }
}
