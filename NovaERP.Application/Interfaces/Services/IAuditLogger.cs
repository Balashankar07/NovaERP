namespace NovaERP.Application.Interfaces.Services;

public interface IAuditLogger
{
    Task LogAsync(string action, string entityName, string entityId, string oldValues = "", string newValues = "");
}
