using App.Domain.Entities.Audit.Input;
using App.Domain.Processes;
using App.Domain.Tests.Helpers.TestEntities;

namespace App.Domain.Tests;

public class TestAuditLogsGenerationForDeleteAction
{
    private Guid _userId;
    private string _callingProcess;

    private TestEmployee _firstEmployeeRecord;
    private TestEmployee _secondEmployeeRecord;
    
    private TestDepartment _firstDepartmentRecord;
    
    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _callingProcess = nameof(TestAuditLogsGenerationForDeleteAction);
        
        _firstEmployeeRecord = new TestEmployee()
        {
            Id = Guid.NewGuid(),
            Ssn = "37082eenck2q",
            CreatedOn = DateTime.Now,
            Address = "test",
            Department = Department.Accounting,
            Name = "John",
            Surname = "Doe",
            Position = "CEO",
            Salary = 120000
        };
        
        _secondEmployeeRecord = new TestEmployee()
        {
            Id = Guid.NewGuid(),
            Ssn = "nfi328e413",
            CreatedOn = DateTime.Now,
            Address = "test 1",
            Department = Department.Accounting,
            Name = "Jane",
            Surname = "Doe",
            Position = "VP",
            Salary = 120000
        };
        
        _firstDepartmentRecord = new TestDepartment()
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            Department = Department.Accounting,
            Employees = [_firstEmployeeRecord, _secondEmployeeRecord],
            YearsActive = [2021, 2022, 2023]
        };
    }

    [Test]
    public void TestAuditLogsGenerationForDelete_Employee()
    {
        var auditLogRequest = new DeleteAuditLogRequest<TestEmployee>(
            _userId.ToString(),
            _callingProcess,
            _firstEmployeeRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count(x => x.OldPropertyValue is not null && x.NewPropertyValue is null), Is.EqualTo(7));
    }
    
    [Test]
    public void TestAuditLogsGenerationForDelete_Department_With_Employees()
    {
        var auditLogRequest = new DeleteAuditLogRequest<TestDepartment>(
            _userId.ToString(),
            _callingProcess,
            _firstDepartmentRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count(x => x is { EntityType: nameof(TestDepartment), OldPropertyValue: not null, NewPropertyValue: null }), Is.EqualTo(3) );
        Assert.That(auditLogs.Count(x => x is { EntityType: nameof(TestEmployee), OldPropertyValue: not null, NewPropertyValue: null }), Is.EqualTo(14) );
    }
}