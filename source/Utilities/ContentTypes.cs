using System;

namespace MinimalHttp.Utilities
{
    /// <summary>
    /// Provides constants for commonly used mime types
    /// </summary>
    public static class ContentTypes
    {
        /// <summary>
        /// Gets the mime type used  for .aac audio files.
        /// </summary>
        public const string AUDIO_AAC = "audio/aac";
        /// <summary>
        /// Gets the mime type used  for .mp3 audio files.
        /// </summary>
        public const string AUDIO_MP3 = "audio/mpeg";
        /// <summary>
        /// Gets the mime type used  for .wav audio files.
        /// </summary>
        public const string AUDIO_WAV = "audio/wav";
        /// <summary>
        /// Gets the mime type used  for .webm audio files.
        /// </summary>
        public const string AUDIO_WEBM = "audio/webm";

        /// <summary>
        /// All characters are encoded before sent (spaces are converted to "+" symbols, and special characters are converted to ASCII HEX values)
        /// </summary>
        public const string APPLICATION_FORM_URLENCODED = "application/x-www-form-urlencoded";
        /// <summary>
        /// Gets the mime type used for binary data.
        /// </summary>
        public const string APPLICATION_OCTET_STREAM = "application/octet-stream";
        /// <summary>
        /// Gets the mime type used for .pdf files.
        /// </summary>
        public const string APPLICATION_PDF = "application/pdf";
        /// <summary>
        /// Gets the mime type used for json data.
        /// </summary>
        public const string APPLICATION_JSON = "application/json";

        /// <summary>
        /// Gets the mime type used for .bmp image files.
        /// </summary>
        public const string IMAGE_BMP = "image/bmp";
        /// <summary>
        /// Gets the mime type used for .gif image files.
        /// </summary>
        public const string IMAGE_GIF = "image/gif";
        /// <summary>
        /// Gets the mime type used for .ico image files.
        /// </summary>
        public const string IMAGE_ICO = "image/x-icon";
        /// <summary>
        /// Gets the mime type used for .jpg image files.
        /// </summary>
        public const string IMAGE_JPG = "image/jpeg";
        /// <summary>
        /// Gets the mime type used for .png image files.
        /// </summary>
        public const string IMAGE_PNG = "image/png";
        /// <summary>
        /// Gets the mime type used for .svg image files.
        /// </summary>
        public const string IMAGE_SVG = "image/svg+xml";

        /// <summary>
        /// Gets the mime type used for .mpeg video files.
        /// </summary>
        public const string VIDEO_MPEG = "video/mpeg";
        /// <summary>
        /// Gets the mime type used for .webm video files.
        /// </summary>
        public const string VIDEO_WEBM = "video/webm";

        /// <summary>
        /// Gets the mime type used for plain text.
        /// </summary>
        public const string TEXT_PLAIN = "text/plain";
        /// <summary>
        /// Gets the mime type used for .css files.
        /// </summary>
        public const string TEXT_CSS = "text/css";
        /// <summary>
        /// Gets the mime type used for .csv files.
        /// </summary>
        public const string TEXT_CSV = "text/csv";
        /// <summary>
        /// Gets the mime type used for .html files.
        /// </summary>
        public const string TEXT_HTML = "text/html";
        /// <summary>
        /// Gets the mime type used for .js files.
        /// </summary>
        public const string TEXT_JAVASCRIPT = "text/javascript";
    }
}
