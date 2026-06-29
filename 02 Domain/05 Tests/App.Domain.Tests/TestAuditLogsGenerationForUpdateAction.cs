using App.Domain.Entities.Audit.Input;
using App.Domain.Processes;
using App.Domain.Tests.Helpers.TestEntities;

namespace App.Domain.Tests;

public class TestAuditLogsGenerationForUpdateAction
{
    private Guid _userId;
    private string _callingProcess;

    private TestEmployee _firstEmployeeRecord;
    private TestEmployee _secondEmployeeRecord;
    private TestEmployee _thirdEmployeeRecord;
    
    private TestDepartment _firstDepartmentRecord;
    private TestDepartment _secondDepartmentRecord;
    
    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _callingProcess = nameof(TestAuditLogsGenerationForUpdateAction);
        
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
        
        _secondDepartmentRecord = new TestDepartment()
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            Department = Department.Finance,
            Employees = [_secondEmployeeRecord, _thirdEmployeeRecord],
            YearsActive = [2021, 2022, 2026]
        };
    }
    
    [Test]
    public void TestAuditLogsGenerationForUpdate_Employee()
    {
        var auditLogRequest = new UpdateAuditLogRequest<TestEmployee>(
            _userId.ToString(),
            _callingProcess,
            oldEntityToLog: _firstEmployeeRecord,
            newEntityToLog: _secondEmployeeRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count, Is.EqualTo(3));
    }
    
    [Test]
    public void TestAuditLogsGenerationForUpdate_Department_With_Employees()
    {
        var auditLogRequest = new UpdateAuditLogRequest<TestDepartment>(
            _userId.ToString(),
            _callingProcess,
            oldEntityToLog: _firstDepartmentRecord,
            newEntityToLog: _secondDepartmentRecord);
        
        var auditLogs = AuditLogService.GenerateAuditLogs(auditLogRequest);
        
        Assert.That(auditLogs.Count(x => x.EntityId == _firstEmployeeRecord.Id.ToString()), Is.EqualTo(7));
        Assert.That(auditLogs.Count(x => x.EntityType == nameof(TestDepartment)), Is.EqualTo(2));
    }
}