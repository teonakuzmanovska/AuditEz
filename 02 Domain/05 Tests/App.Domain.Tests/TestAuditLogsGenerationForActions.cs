using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Enum;
using App.Domain.Processes;
using App.Domain.Tests.Helpers.TestEntities;

namespace App.Domain.Tests;

public class TestAuditLogsGenerationForActions
{
    private Guid _userId;
    private string _callingProcess;

    private TestEmployee _originalEmployeeRecord;
    private TestEmployee _editedEmployeeRecord;
    
    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _callingProcess = "TestMethod";
        
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
    public void TestAuditLogsGenerationForCreate()
    {
        var auditLogService = new AuditLogService<TestEmployee>();
        
        var actionInfo = new ActionInfo(_userId.ToString(), ActionType.Create, _callingProcess);

        var auditLogRequest = new AuditLogRequest<TestEmployee>(
            actionInfo,
            _originalEmployeeRecord.UserId.ToString(),
            oldEntityToLog: null,
            newEntityToLog: _originalEmployeeRecord);
        
        var auditLogs = auditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count(x => x.OldPropertyValue is null), Is.EqualTo(8) );
        Assert.That(auditLogs.Count(x => x.NewPropertyValue is not null), Is.EqualTo(8) );
    }

    [Test]
    public void TestAuditLogsGenerationForDelete()
    {
        var auditLogService = new AuditLogService<TestEmployee>();
        
        var actionInfo = new ActionInfo(_userId.ToString(), ActionType.Delete, _callingProcess);

        var auditLogRequest = new AuditLogRequest<TestEmployee>(
            actionInfo,
            _originalEmployeeRecord.UserId.ToString(),
            oldEntityToLog: _originalEmployeeRecord,
            newEntityToLog: null);
        
        var auditLogs = auditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count(x => x.OldPropertyValue is not null), Is.EqualTo(8) );
        Assert.That(auditLogs.Count(x => x.NewPropertyValue is null), Is.EqualTo(8) );
    }
    
    [Test]
    public void TestAuditLogsGenerationForUpdate()
    {
        var auditLogService = new AuditLogService<TestEmployee>();
        
        var actionInfo = new ActionInfo(_userId.ToString(), ActionType.Update, _callingProcess);

        var auditLogRequest = new AuditLogRequest<TestEmployee>(
            actionInfo,
            _originalEmployeeRecord.UserId.ToString(),
            oldEntityToLog: _originalEmployeeRecord,
            newEntityToLog: _editedEmployeeRecord);
        
        var auditLogs = auditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count, Is.EqualTo(2));
    }
}