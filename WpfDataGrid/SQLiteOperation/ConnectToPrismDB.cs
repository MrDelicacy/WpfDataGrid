using System.Data.Entity;

namespace WpfDataGrid.SQLiteOperation
{
    public class ConnectToPrismDB : DbContext
    {
        public ConnectToPrismDB():base("DefaultConnection")
        {
        }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<WorkProcessAction> WorkProcessActions { get; set; }
        public DbSet<UndoWorkProcessAction> UndoWorkProcessActions { get; set; }
        public DbSet<OrderIterationRow> OrderIterationRows { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<TestWeightInfo> TestWeightInfoes { get; set; }
        public DbSet<CompletedWork> CompletedWorks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<WorkProcessAction>()
                .HasRequired<CustomerOrder>(w => w.CustomerOrder)
                .WithMany(c=>c.WorkProcessActions)
                .HasForeignKey<int>(w => w.CustomerOrderId);

            modelBuilder.Entity<UndoWorkProcessAction>()
                .HasRequired<CustomerOrder>(w => w.CustomerOrder)
                .WithMany(c => c.UndoWorkProcessActions)
                .HasForeignKey<int>(w => w.CustomerOrderId);

            modelBuilder.Entity<OrderIterationRow>()
                .HasRequired<CustomerOrder>(o => o.CustomerOrder)
                .WithMany(c => c.OrderIterations)
                .HasForeignKey<int>(o => o.CustomerOrderId);

            modelBuilder.Entity<CustomerOrder>()
                .HasRequired<Customer>(c => c.Customer)
                .WithMany(c => c.CustomerOrders)
                .HasForeignKey<int>(w => w.CustomerId);

            modelBuilder.Entity<CustomerOrder>()
                .HasOptional(c => c.OrderDetail)
                .WithRequired(d => d.Order);

            modelBuilder.Entity<TestWeightInfo>()
                .HasRequired<CustomerOrder>(t => t.CustomerOrder)
                .WithMany(c => c.TestWeightInfoes)
                .HasForeignKey<int>(t => t.CustomerOrderId);

            modelBuilder.Entity<CustomerOrder>()
                .HasOptional(c => c.CompletedWork)
                .WithRequired(c => c.Order);
        }
    }
}
