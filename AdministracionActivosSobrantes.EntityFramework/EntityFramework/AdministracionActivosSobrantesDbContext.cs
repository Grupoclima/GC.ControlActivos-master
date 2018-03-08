using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Abp.EntityFramework;
using AdministracionActivosSobrantes.Adjustments;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Contractors;
using AdministracionActivosSobrantes.CustomFields;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.HistoryChanges;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.PriceChanges;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.ToolAssets;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.EntityFramework
{
    public class AdministracionActivosSobrantesDbContext : AbpDbContext
    {
        //TODO: Define an IDbSet for each Entity...

        //Example:
        public virtual IDbSet<Asset> Assets { get; set; }
        public virtual IDbSet<Cellar> Cellars { get; set; }
        public virtual IDbSet<Category> Categories { get; set; }
        public virtual IDbSet<Project> Projects { get; set; }
        public virtual IDbSet<OutRequest.OutRequest> OutRequests { get; set; }
        public virtual IDbSet<InRequest.InRequest> InRequests { get; set; }
        public virtual IDbSet<User> Users { get; set; }
        public virtual IDbSet<Stock> Stocks { get; set; }
        public virtual IDbSet<Movement> Movements { get; set; }
        public virtual IDbSet<PriceChange> PriceChanges { get; set; }
        public virtual IDbSet<Detail> Details { get; set; }
        public virtual IDbSet<CustomField> CustomFields { get; set; }
        public virtual IDbSet<Adjustment> Adjustments { get; set; }
        public virtual IDbSet<ToolAsset> ToolAssets { get; set; }
        public virtual IDbSet<Contractor> Contractors { get; set; }
        public virtual IDbSet<HistoryChange> HistoryChanges { get; set; }
        public virtual IDbSet<ResponsiblePerson.ResponsiblePerson> ResponsiblePersons { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public AdministracionActivosSobrantesDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in AdministracionActivosSobrantesDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of AdministracionActivosSobrantesDbContext since ABP automatically handles it.
         */
        public AdministracionActivosSobrantesDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Asset>().ToTable("Asset", schemaName: "UCASCHEMA");
            modelBuilder.Entity<Category>().ToTable("Category", schemaName: "UCASCHEMA");
            modelBuilder.Entity<Cellar>().ToTable("Cellar", schemaName: "UCASCHEMA");
            modelBuilder.Entity<Project>().ToTable("Project", schemaName: "UCASCHEMA");
            modelBuilder.Entity<OutRequest.OutRequest>().ToTable("OutRequest", schemaName: "UCASCHEMA");
            modelBuilder.Entity<InRequest.InRequest>().ToTable("InRequest", schemaName: "UCASCHEMA");
            modelBuilder.Entity<User>().ToTable("User", schemaName: "UCASCHEMA");
            modelBuilder.Entity<Stock>().ToTable("Stock", schemaName: "UCASCHEMA");
            modelBuilder.Entity<Movement>().ToTable("Movement", schemaName: "UCASCHEMA");
            modelBuilder.Entity<PriceChange>().ToTable("PriceChange", schemaName: "UCASCHEMA");
            modelBuilder.Entity<Detail>().ToTable("Detail", schemaName: "UCASCHEMA");
            modelBuilder.Entity<CustomField>().ToTable("CustomField", schemaName: "UCASCHEMA");
            modelBuilder.Entity<Adjustment>().ToTable("Adjustment", schemaName: "UCASCHEMA");
            modelBuilder.Entity<ToolAsset>().ToTable("ToolAsset", schemaName: "UCASCHEMA");
            modelBuilder.Entity<Contractor>().ToTable("Contractor", schemaName: "UCASCHEMA");
            modelBuilder.Entity<HistoryChange>().ToTable("HistoryChange", schemaName: "UCASCHEMA");
            modelBuilder.HasDefaultSchema("UCASCHEMA");
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Filter(FiltersNames.FilterCompany, (ITenantCompanyName entity, string company) => entity.CompanyName == company, "");
        }
    }
}
