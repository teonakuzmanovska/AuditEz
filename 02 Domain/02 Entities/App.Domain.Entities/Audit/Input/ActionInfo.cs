using System.Diagnostics.CodeAnalysis;
using App.Domain.Entities.Enum;

namespace App.Domain.Entities.Audit.Input;

public class ActionInfo
{
    public required string UserId { get; init; }
    
    public required ActionType Action { get; init; }
    
    public required string ProcessName { get; init; }
    
    [SetsRequiredMembers]
    public ActionInfo(string userId, ActionType action, string processName)
    {
        UserId = userId;
        Action = action;
        ProcessName = processName;
    }
}