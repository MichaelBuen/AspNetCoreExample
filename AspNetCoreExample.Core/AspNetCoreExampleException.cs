using System;

namespace AspNetCoreExample
{
    public class AspNetCoreExampleException : Exception
    {
        public Mnemonic? Mnemonic { get; set; }
        public System.Net.HttpStatusCode HttpStatusCode { get; set; }

        public AspNetCoreExampleException(
            Mnemonic mnemonic,
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.InternalServerError
        )
        {
            this.Mnemonic = mnemonic;
            this.HttpStatusCode = statusCode;
        }

        public AspNetCoreExampleException(
            string message,
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.InternalServerError
        ) : base(message) => this.HttpStatusCode = statusCode;


        public AspNetCoreExampleException(
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.InternalServerError
        ) : base("Contact the developer. This should not happen on production.") => 
            this.HttpStatusCode = statusCode;

    }
}
