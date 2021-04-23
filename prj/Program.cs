

using System.Threading.Tasks;
using practice.EntityClasses;
using repository;

namespace prj
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var employee=new EmployeeEntity(){
                Name="mostafa mohammed"
            };

            var manager=new ManagerEntity(){
                Name="sara ahmed",
                DepartmentId=4
            };

            var repository=new llbl();
            await repository.PostManagerEmployeeData(employee,manager);
            
            await repository.GetEmployeesAnManagers();
            
            
        }
    }
}
