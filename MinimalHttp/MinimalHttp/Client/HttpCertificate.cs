using System;
using System.Security.Cryptography.X509Certificates;

namespace MinimalHttp.Client
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <seealso cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" />
    public class HttpCertificate : X509Certificate2
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MinimalHttp.Client.HttpCertificate" /> class.
        /// </summary>
        /// <param name="cert">
        ///     A <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate" /> class from which to
        ///     initialize this class.
        /// </param>
        public HttpCertificate(X509Certificate cert) : base(new X509Certificate2(cert))
        {
        }

        /// <summary>
        ///     Validates the period.
        /// </summary>
        /// <returns></returns>
        public bool ValidatePeriod()
        {
            var start = NotBefore;
            var end = NotAfter;

            var cur = DateTime.Now;

            if (cur < start) return false;

            return cur <= end;
        }

        /// <summary>
        ///     Validates the public key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">key</exception>
        public bool ValidatePublicKey(byte[] key)
        {
            if (key == null || key.Length == 0) throw new ArgumentNullException(nameof(key));

            return CompareBytes(GetPublicKey(), key);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Compares two <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate" /> objects for equality.
        /// </summary>
        /// <param name="other">
        ///     An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate" /> object to compare
        ///     to the current object.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the current
        ///     <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate" /> object is equal to the object
        ///     specified by the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(X509Certificate other)
        {
            return other != null && CompareBytes(GetRawCertData(), other.GetRawCertData());
        }

        /// <summary>
        ///     Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(HttpCertificate other)
        {
            return other != null && CompareBytes(GetRawCertData(), other.GetRawCertData());
        }

        /// <summary>
        ///     Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
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