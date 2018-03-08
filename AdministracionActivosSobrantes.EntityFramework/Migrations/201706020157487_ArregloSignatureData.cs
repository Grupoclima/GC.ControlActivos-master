namespace AdministracionActivosSobrantes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArregloSignatureData : DbMigration
    {
        public override void Up()
        {
            AddColumn("UCASCHEMA.InRequest", "SignatureData2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("UCASCHEMA.InRequest", "SignatureData2");
        }
    }
}
