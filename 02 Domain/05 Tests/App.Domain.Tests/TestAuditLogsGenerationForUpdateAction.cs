using App.Domain.Entities.Audit.Input;
using App.Domain.Processes;
using App.Domain.Tests.Helpers.TestEntities;

namespace App.Domain.Tests;

public class TestAuditLogsGenerationForUpdateAction
{
    private Guid _userId;
    private string _callingProcess;

    private TestEmployee _originalEmployeeRecord;
    private TestEmployee _editedEmployeeRecord;
    
    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _callingProcess = nameof(TestAuditLogsGenerationForUpdateAction);
        
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
        
        _editedEmployeeRecord = new TestEmployee()
        {
            UserId = _originalEmployeeRecord.UserId,
            CreatedOn = DateTime.Now,
            Address = "test",
            Department = Department.Finance,
            Name = "John",
            Surname = "Doe",
            Position = "CEO",
            Salary = 150000
        };
    }
    
    [Test]
    public void TestAuditLogsGenerationForUpdate()
    {
        var auditLogRequest = new UpdateAuditLogRequest<TestEmployee>(
            _userId.ToString(),
            _callingProcess,
            oldEntityToLog: _originalEmployeeRecord,
            newEntityToLog: _editedEmployeeRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count, Is.EqualTo(2));
    }
}