using System;

namespace Sunday
{
    public class SundayAdError
    {
        private string message;
        private string source;

        public SundayAdError(string errorMessage, string adSource)
        {
            message = errorMessage;
            source = adSource;
        }

        public string GetErrorMessage()
        {
            return message;
        }

        public string GetAdSource()
        {
            return source;
        }

        public override string ToString()
        {
            return source + ": " + message;
        }
    }
}