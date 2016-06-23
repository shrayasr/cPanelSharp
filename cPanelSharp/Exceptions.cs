using System;

namespace cPanelSharp
{
    public class MissingCredentialsException : Exception { }
    public class InvalidCredentialsException : Exception { }

    public class InvalidParametersException : Exception
    {
        public InvalidParametersException(string message) : base(message) { }
    }
}
