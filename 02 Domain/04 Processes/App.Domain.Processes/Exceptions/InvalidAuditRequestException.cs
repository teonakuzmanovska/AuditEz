namespace App.Domain.Processes.Exceptions;

public class InvalidAuditRequestException : Exception
{
    public InvalidAuditRequestException(string message) : base(message) { }
    
    public InvalidAuditRequestException(string message, Exception inner) : base(message, inner) { }
}