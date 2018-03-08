namespace AdministracionActivosSobrantes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SignatureData : DbMigration
    {
        public override void Up()
        {
            DropColumn("UCASCHEMA.InRequest", "SignatureData");
        }
        
        public override void Down()
        {
            AddColumn("UCASCHEMA.InRequest", "SignatureData", c => c.String(maxLength: 1024));
        }
    }
}
