namespace Identity.API.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // Add-Migration InitialMigration -OutputDir Data/Migrations
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customize your identity models here, if needed
            base.OnModelCreating(modelBuilder);

            // Configure OpenIddict
            modelBuilder.UseOpenIddict();
        }
    }
}
