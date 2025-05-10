using System;

namespace DEAT.WebAPI.TigerBeetleServices.Exceptions;

public class AccountAlreadyExistsException : Exception
{
    public AccountAlreadyExistsException(string message) : base(message)
    {
    }

    public AccountAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}