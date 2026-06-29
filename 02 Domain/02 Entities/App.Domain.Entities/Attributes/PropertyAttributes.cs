namespace App.Domain.Entities.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class EntityIdentifierAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class AuditIgnoreAttribute : Attribute { }