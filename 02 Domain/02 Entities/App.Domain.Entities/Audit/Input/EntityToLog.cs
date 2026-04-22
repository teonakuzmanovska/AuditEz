namespace App.Domain.Entities.Audit.Input;

public class EntityToLog<T> where T : class
{
    public string? Id { get; set; }

    public EntityToLog() { }

    public EntityToLog(string? id)
    {
        Id = id;
    }
}