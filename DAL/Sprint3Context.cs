//using System.Data.Entity;
//using StayFit.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Data.Entity.ModelConfiguration.Conventions;
//using StayFit.Models.Data;

//namespace StayFit.DAL
//{
//    public class Sprint3Context : DbContext
//    {
//        public Sprint3Context() : base("Sprint3Context")
//        {
//        }
//        public DbSet<Manager> Managers { get; set; }
//        public DbSet<Employee> Employees { get; set; }
//        public DbSet<Client> Clients { get; set; }
//        public DbSet<Lead> Leads { get; set; }
//        public DbSet<NextOfKin> NextOfKins { get; set; }
//        public DbSet<Occupation> Occupations { get; set; }
//        public DbSet<Package> Packages { get; set; }
//        public DbSet<LoyaltyPoint> LoyaltyPoints { get; set; }
//        public DbSet<EmployeeBonus> EmployeeBonuses { get; set; }

//        public DbSet<Exercises> Exercisess { get; set; }
//        public DbSet<Nutrition> Nutritions { get; set; }

//        public DbSet<ExrecisePlan> ExrecisePlans { get; set; }
//        public DbSet<DietPlan> DietPlans { get; set; }

//        public DbSet<ClientDietPlan> ClientDietPlans { get; set; }
//        public DbSet<ClientExercisePlan> ClientExercisePlans { get; set; }
//        public DbSet<LeadNotes> LeadNotes { get; set; }
//        public DbSet<EmployeeCommission> EmployeeCommissions { get; set; }
//        public DbSet<Vat> Vats { get; set; }
//        public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
//        public DbSet<GymSession> GymSessions { get; set; }
//        public DbSet<SessionInvoice> SessionInvoices { get; set; }
//        public DbSet<Progress> Progresses { get; set; }


//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
//        }
//    }
//}