using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using practice.DatabaseSpecific;
using practice.EntityClasses;
using practice.HelperClasses;
using practice.Linq;
using SD.LLBLGen.Pro.DQE.SqlServer;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using ViewModel.Persistence;

namespace repository
{
    public class llbl
    {

        /* 
            
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

            
             */
        public llbl()
        {
            RuntimeConfiguration.ConfigureDQE<SQLServerDQEConfiguration>(
                c=>c.AddDbProviderFactory(typeof(System.Data.SqlClient.SqlClientFactory)));
                RuntimeConfiguration.AddConnectionString
                ("ConnectionString.SQL Server (SqlClient)","data source=DESKTOP-CUE6HRC;initial catalog=llbl; Integrated Security=SSPI");
        }

        public async Task PostManagerEmployeeData(EmployeeEntity employee,ManagerEntity manager){

            using(var adapter=new DataAccessAdapter()){
                //var em=manager.Employees.AddNew();

                await adapter.SaveEntityAsync(manager,true);

                employee.ManagerId=manager.Id;

                Console.WriteLine(manager.Id+"  manager.Id");

                await adapter.SaveEntityAsync(employee,true);

                Console.WriteLine(employee.Id+"  employee.Id");
            }
        }

        public async Task GetManagerById(int id){
            

            using(var adapter=new DataAccessAdapter()){
            var linq=new LinqMetaData(adapter);
                var manager=await linq.Manager.FirstOrDefaultAsync(m=>m.Id==id);
                Console.WriteLine("( {0} manager, his is is {1} ) GetById",manager.Name,manager.Id);
            }
            
        }

        public async Task UpdateManagersAndEmployees(ManagerEntity manager,EmployeeEntity employee){
             
             using(var adapter=new DataAccessAdapter()){

             adapter.StartTransaction(IsolationLevel.ReadCommitted, "FirstTransaction");
             
             try{
                 manager.IsNew=false;
                 employee.IsNew=false;

                 await adapter.SaveEntityAsync(manager);
                 await adapter.SaveEntityAsync(employee);

                 adapter.Commit();

             }
             catch (Exception e){
                 adapter.Rollback();
                 throw new Exception("Transaction rollbacked {0}",e);
             }

             }

        }
        public void GetManagers(){

            EntityCollection<ManagerEntity> mg = new EntityCollection<ManagerEntity>();
            using(DataAccessAdapter adapter = new DataAccessAdapter())
            {
                adapter.FetchEntityCollection(mg,null); // fetch all customers
            }
            EntityView2<ManagerEntity> mgView = new EntityView2<ManagerEntity>(mg);

            foreach(var x in mgView){
                Console.WriteLine("manager name : {0}, his id {1} GetManagers",x.Name,x.Id);
            }

        }

        public async Task GetEmployeesAnManagers(){

            EntityCollection<DepartmentEntity> dp = new EntityCollection<DepartmentEntity>();
            
            using(DataAccessAdapter adapter = new DataAccessAdapter())
            {
                var linq=new LinqMetaData(adapter);

                var EmpMng=await linq.Manager.ProjectToManagerEmployeeViewModel().ToListAsync();

                //adapter.FetchEntityCollection(dp, null); // fetch all customers

                foreach(var x in EmpMng){
                    foreach(var emp in x.Employees)
                    {
                    Console.WriteLine("( Employee : {0} > card number {1}, his manager : {2} > card number:{3}",
                    emp.Name,emp.Id,x.Name,x.Id);
                    }
                }

            }
        }

        public async Task DeleteEmployee(int id){

            using(DataAccessAdapter adapter = new DataAccessAdapter())
            {
                var linq=new LinqMetaData(adapter);
                var employee=await linq.Employee.FirstOrDefaultAsync(m=>m.Id==id);
                var IsDone=await adapter.DeleteEntityAsync(employee);

                if(!IsDone){
                    Console.WriteLine("Something went wrong");
                }
            }

        }

    
    }
}