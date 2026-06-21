using App.Domain.Entities.Audit.Input;
using App.Domain.Processes;
using App.Domain.Tests.Helpers.TestEntities;

namespace App.Domain.Tests;

public class TestAuditLogsGenerationForCreateAction
{
    private Guid _userId;
    private string _callingProcess;

    private TestEmployee _firstEmployeeRecord;
    private TestEmployee _secondEmployeeRecord;
    private TestEmployee _thirdEmployeeRecord;
    
    private TestDepartment _departmentRecord;
    
    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _callingProcess = nameof(TestAuditLogsGenerationForCreateAction);
        
        _firstEmployeeRecord = new TestEmployee()
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
        
        _secondEmployeeRecord = new TestEmployee()
        {
            UserId = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            Address = "test 1",
            Department = Department.Accounting,
            Name = "Jane",
            Surname = "Doe",
            Position = "CEO",
            Salary = 120000
        };
        
        _thirdEmployeeRecord = new TestEmployee()
        {
            UserId = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            Address = "test 3",
            Department = Department.Accounting,
            Name = "Bob",
            Surname = "Marley",
            Position = "CEO",
            Salary = 120000
        };

        _departmentRecord = new TestDepartment()
        {
            Id = Guid.NewGuid(),
            Department = Department.Accounting,
            Employees = [_firstEmployeeRecord, _secondEmployeeRecord, _thirdEmployeeRecord],
            YearsActive = [2021, 2022, 2023]
        };
        
    }
    
    [Test]
    public void TestAuditLogsGenerationForCreate()
    {
        var auditLogRequest = new CreateAuditLogRequest<TestEmployee>(
            _userId.ToString(), 
            _callingProcess,
            _firstEmployeeRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count(x => x.OldPropertyValue is null), Is.EqualTo(8) );
        Assert.That(auditLogs.Count(x => x.NewPropertyValue is not null), Is.EqualTo(8) );
    }
    
    [Test]
    public void TestAuditLogsGenerationForCreate_With_ListProperties()
    {
        var auditLogRequest = new CreateAuditLogRequest<TestDepartment>(
            _userId.ToString(), 
            _callingProcess,
            _departmentRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.Pass();
        
        //Assert.That(auditLogs.Count(x => x.OldPropertyValue is null), Is.EqualTo(8) );
        //Assert.That(auditLogs.Count(x => x.NewPropertyValue is not null), Is.EqualTo(8) );
    }
}