using App.Domain.Entities.Audit.Input;
using App.Domain.Processes;
using App.Domain.Tests.Helpers.TestEntities;

namespace App.Domain.Tests;

public class TestAuditLogsGenerationForDeleteAction
{
    private Guid _userId;
    private string _callingProcess;

    private TestEmployee _originalEmployeeRecord;
    
    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _callingProcess = nameof(TestAuditLogsGenerationForDeleteAction);
        
        _originalEmployeeRecord = new TestEmployee()
        {
            UserId = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            Address = "test",
            Department = Department.Accounting,
            Name = "John",
            Surname = "Doe",
            Position = "CEO",
            Salary = 120000
        };
    }

    [Test]
    public void TestAuditLogsGenerationForDelete()
    {
        var auditLogRequest = new DeleteAuditLogRequest<TestEmployee>(
            _userId.ToString(),
            _callingProcess,
            _originalEmployeeRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count(x => x.OldPropertyValue is not null), Is.EqualTo(8) );
        Assert.That(auditLogs.Count(x => x.NewPropertyValue is null), Is.EqualTo(8) );
    }
}