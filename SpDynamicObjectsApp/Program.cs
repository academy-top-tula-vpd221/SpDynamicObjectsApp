using System.Dynamic;

/*
dynamic employee = new ExpandoObject();
employee.Name = "Bob";
employee.Age = 43;
employee.Salary = 56000.0;
employee.Skills = new List<string> { "C++", "C#", "JavaScript" };

Console.WriteLine($"Name: {employee.Name} Age: {employee.Age}");
foreach ( var item in employee.Skills )
    Console.WriteLine(item);

employee.SalaryAdd = (Action<int>)(s => employee.Salary += s);
Console.WriteLine($"Salary: {employee.Salary}");
employee.SalaryAdd(15000);
Console.WriteLine($"Salary: {employee.Salary}");
*/


dynamic bob = new EmployeeDynamic();
bob.Name = "Bobby";
bob.Age = 35;
bob.Salary = 60000;

Func<int, int> salaryAdd = (int s) =>
{
    bob.Salary += s;
    return bob.Salary;
};

bob.SalaryAdd = salaryAdd;

Console.WriteLine($"Name: {bob.Name}, Age: {bob.Age}, Salary: {bob.Salary}");
bob.SalaryAdd(27000);
Console.WriteLine($"Name: {bob.Name}, Age: {bob.Age}, Salary: {bob.Salary}");

dynamic joe = new EmployeeDynamic();
joe.Name = "Joe";
joe.Age = 26;
joe.Salary = 33000;
Console.WriteLine($"Name: {joe.Name}, Age: {joe.Age}, Salary: {joe.Salary}");

dynamic res = bob + joe;
Console.WriteLine($"Name: {res.Name}, Age: {res.Age}, Salary: {res.Salary}");


class EmployeeDynamic : DynamicObject
{
    Dictionary<string, object> members = new Dictionary<string, object>();

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if(value is not null)
        {
            members[binder.Name] = value;
            return true;
        }

        return false;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = null;
        if(members.ContainsKey(binder.Name))
        {
            result = members[binder.Name];
            return true;
        }
        return false;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        result = null;

        if (args?[0] is int number)
        {
            dynamic method = members[binder.Name];
            result = method(number);
        }

        return result != null;
    }

    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result)
    {
        result = null;
        dynamic second = (EmployeeDynamic)arg;

        if(binder.Operation == System.Linq.Expressions.ExpressionType.Add)
        {
            dynamic resultEmployee = new EmployeeDynamic();
            resultEmployee.members["Name"] = "Result";
            resultEmployee.members["Age"] = 0;
            resultEmployee.members["Salary"] = (int)members["Salary"] + (int)second.members["Salary"];

            result = resultEmployee;
        }

        return result != null;
    }
}