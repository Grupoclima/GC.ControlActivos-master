namespace AdministracionActivosSobrantes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameSignatureData : DbMigration
    {
        public override void Up()
        {
            AddColumn("UCASCHEMA.InRequest", "SignatureData", c => c.String());
            DropColumn("UCASCHEMA.InRequest", "SignatureData2");
        }
        
        public override void Down()
        {
            AddColumn("UCASCHEMA.InRequest", "SignatureData2", c => c.String());
            DropColumn("UCASCHEMA.InRequest", "SignatureData");
        }
    }
}
