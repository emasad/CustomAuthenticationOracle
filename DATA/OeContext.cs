using Domain;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA
{
    public class OeContext : DbContext
    {
        public OeContext() : base("name=OracleDbContext") { }
        //User Id=TESTDB;Password=1234;Data Source=10.10.80.81:1521/XE
        //public DbSet<TSubject> TSubjects { get; set; }
        //public DbSet<Love> Loves { get; set; }
        //public DbSet<Department> Departments { get; set; }
        //public DbSet<SysUser> SysUsers { get; set; }
        //public DbSet<Employee> Employees { get; set; }.
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("SBSOFT");
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .Map(m =>
                {
                    m.ToTable("UserRoles");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                });
            // ...ConfigurationManager.ConnectionStrings["Entities"].ConnectionString
        }
    }
}
