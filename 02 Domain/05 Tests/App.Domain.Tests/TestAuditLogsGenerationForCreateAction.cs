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
    
    private TestDepartment _firstDepartmentRecord;
    
    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _callingProcess = nameof(TestAuditLogsGenerationForCreateAction);
        
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
        
        _thirdEmployeeRecord = new TestEmployee()
        {
            Id = Guid.NewGuid(),
            Ssn = "hfdoew783492",
            CreatedOn = DateTime.Now,
            Address = "test 3",
            Department = Department.Marketing,
            Name = "Bob",
            Surname = "Marley",
            Position = "Artist",
            Salary = 120000
        };

        _firstDepartmentRecord = new TestDepartment()
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            Department = Department.Accounting,
            Employees = [_firstEmployeeRecord, _secondEmployeeRecord, _thirdEmployeeRecord],
            YearsActive = [2021, 2022, 2023]
        };
    }
    
    [Test]
    public void TestAuditLogsGenerationForCreate_Employee()
    {
        var auditLogRequest = new CreateAuditLogRequest<TestEmployee>(
            _userId.ToString(), 
            _callingProcess,
            _firstEmployeeRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count(x => x.OldPropertyValue is null), Is.EqualTo(7));
        Assert.That(auditLogs.Count(x => x.NewPropertyValue is not null), Is.EqualTo(7));
    }
    
    [Test]
    public void TestAuditLogsGenerationForCreate_Department_With_Employees()
    {
        var auditLogRequest = new CreateAuditLogRequest<TestDepartment>(
            _userId.ToString(), 
            _callingProcess,
            _firstDepartmentRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count, Is.EqualTo(24));
    }
}