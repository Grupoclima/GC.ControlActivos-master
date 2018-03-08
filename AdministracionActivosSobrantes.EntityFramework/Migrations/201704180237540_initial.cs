namespace AdministracionActivosSobrantes.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "UCASCHEMA.Adjustment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RequestNumber = c.Decimal(nullable: false, precision: 10, scale: 0),
                        RequestDocumentNumber = c.String(maxLength: 50),
                        TypeAdjustment = c.Decimal(nullable: false, precision: 10, scale: 0),
                        Status = c.Decimal(nullable: false, precision: 10, scale: 0),
                        Notes = c.String(maxLength: 256),
                        PersonInCharge = c.String(nullable: false, maxLength: 256),
                        Comment = c.String(maxLength: 512),
                        ProcessedDate = c.DateTime(),
                        CellarId = c.Guid(),
                        ApprovalUserId = c.Guid(),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.User", t => t.ApprovalUserId)
                .ForeignKey("UCASCHEMA.Cellar", t => t.CellarId)
                .Index(t => t.CellarId)
                .Index(t => t.ApprovalUserId);
            
            CreateTable(
                "UCASCHEMA.User",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserCode = c.String(nullable: false, maxLength: 64),
                        UserName = c.String(nullable: false, maxLength: 64),
                        Password = c.String(nullable: false, maxLength: 128),
                        CompleteName = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        Phone = c.String(maxLength: 48),
                        Rol = c.Decimal(nullable: false, precision: 10, scale: 0),
                        IsDeleted = c.Decimal(nullable: false, precision: 1, scale: 0),
                        DeleterUserId = c.Decimal(precision: 19, scale: 0),
                        DeletionTime = c.DateTime(),
                        CompanyName = c.String(maxLength: 250),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Decimal(precision: 19, scale: 0),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Decimal(precision: 19, scale: 0),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_User_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "UCASCHEMA.Cellar",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CellarNumber = c.Decimal(nullable: false, precision: 10, scale: 0),
                        Name = c.String(nullable: false, maxLength: 350),
                        Address = c.String(maxLength: 1000),
                        CostCenter = c.String(maxLength: 150),
                        Phone = c.String(maxLength: 20),
                        WareHouseManagerId = c.Guid(nullable: false),
                        Active = c.Decimal(nullable: false, precision: 1, scale: 0),
                        Latitude = c.String(maxLength: 50),
                        Longitude = c.String(maxLength: 50),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                        WareHouseManagerUser_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.User", t => t.WareHouseManagerUser_Id)
                .Index(t => t.WareHouseManagerUser_Id);
            
            CreateTable(
                "UCASCHEMA.Stock",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AssetQtyOutputs = c.Double(nullable: false),
                        AssetQtyInputs = c.Double(nullable: false),
                        AssetQtyOutputsBlocked = c.Double(nullable: false),
                        CellarId = c.Guid(nullable: false),
                        AssetId = c.Guid(nullable: false),
                        AmountOutput = c.Double(nullable: false),
                        AmountInput = c.Double(nullable: false),
                        AmountBlockedOutput = c.Double(nullable: false),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Asset", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("UCASCHEMA.Cellar", t => t.CellarId, cascadeDelete: true)
                .Index(t => t.CellarId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "UCASCHEMA.Asset",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(nullable: false, maxLength: 256),
                        CategoryStr = c.String(maxLength: 256),
                        ResponsiblePersonStr = c.String(maxLength: 256),
                        Name = c.String(nullable: false, maxLength: 256),
                        Barcode = c.String(maxLength: 256),
                        Description = c.String(maxLength: 1024),
                        Brand = c.String(maxLength: 256),
                        ModelStr = c.String(maxLength: 256),
                        Series = c.String(maxLength: 256),
                        Plate = c.String(maxLength: 256),
                        Model = c.DateTime(),
                        PurchaseDate = c.DateTime(),
                        AdmissionDate = c.DateTime(nullable: false),
                        ImagePath = c.String(maxLength: 256),
                        DepreciationMonthsQty = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        WarrantyPeriod = c.Decimal(precision: 10, scale: 0),
                        AssetType = c.Decimal(nullable: false, precision: 10, scale: 0),
                        IsAToolInKit = c.Decimal(nullable: false, precision: 1, scale: 0),
                        CategoryId = c.Guid(),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Category", t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "UCASCHEMA.Category",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 1024),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "UCASCHEMA.CustomField",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 350),
                        Value = c.String(maxLength: 256),
                        ValueDate = c.DateTime(),
                        ValueString = c.String(maxLength: 256),
                        ValueInt = c.Decimal(precision: 10, scale: 0),
                        ValueDouble = c.Double(),
                        AssetId = c.Guid(nullable: false),
                        CustomFieldType = c.Decimal(nullable: false, precision: 10, scale: 0),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Asset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AssetId);
            
            CreateTable(
                "UCASCHEMA.ToolAsset",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(nullable: false, maxLength: 256),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 1024),
                        AdmissionDate = c.DateTime(nullable: false),
                        Quatity = c.Double(nullable: false),
                        AssetId = c.Guid(nullable: false),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Asset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AssetId);
            
            CreateTable(
                "UCASCHEMA.Detail",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InRequestId = c.Guid(),
                        AdjustmentId = c.Guid(),
                        OutRequestId = c.Guid(),
                        AssetId = c.Guid(nullable: false),
                        NameAsset = c.String(maxLength: 100),
                        StockAsset = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        AssetReturnQty = c.Double(),
                        AssetReturnPartialQty = c.Double(),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        Status = c.Decimal(nullable: false, precision: 10, scale: 0),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Adjustment", t => t.AdjustmentId)
                .ForeignKey("UCASCHEMA.Asset", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("UCASCHEMA.InRequest", t => t.InRequestId)
                .ForeignKey("UCASCHEMA.OutRequest", t => t.OutRequestId)
                .Index(t => t.InRequestId)
                .Index(t => t.AdjustmentId)
                .Index(t => t.OutRequestId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "UCASCHEMA.InRequest",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RequestNumber = c.Decimal(nullable: false, precision: 10, scale: 0),
                        RequestDocumentNumber = c.String(maxLength: 50),
                        PurchaseOrderNumber = c.String(maxLength: 100),
                        PersonInCharge = c.String(nullable: false, maxLength: 256),
                        Comment = c.String(maxLength: 512),
                        UrlPhisicalInRequest = c.String(),
                        CellarId = c.Guid(nullable: false),
                        ProjectId = c.Guid(),
                        UserId = c.Guid(),
                        Username = c.String(maxLength: 20),
                        Notes = c.String(maxLength: 256),
                        Status = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TypeInRequest = c.Decimal(nullable: false, precision: 10, scale: 0),
                        ActivationDate = c.DateTime(),
                        ProcessedDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        ImagePath1 = c.String(maxLength: 1024),
                        ImagePath2 = c.String(maxLength: 1024),
                        SignatureData = c.String(maxLength: 1024),
                        ResponsiblePersonId = c.Guid(),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Cellar", t => t.CellarId, cascadeDelete: true)
                .ForeignKey("UCASCHEMA.Project", t => t.ProjectId)
                .ForeignKey("UCASCHEMA.ResponsiblePerson", t => t.ResponsiblePersonId)
                .ForeignKey("UCASCHEMA.User", t => t.UserId)
                .Index(t => t.CellarId)
                .Index(t => t.ProjectId)
                .Index(t => t.UserId)
                .Index(t => t.ResponsiblePersonId);
            
            CreateTable(
                "UCASCHEMA.Project",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                        Code = c.String(nullable: false, maxLength: 256),
                        CostCenter = c.String(maxLength: 150),
                        Description = c.String(maxLength: 1024),
                        EstadoProyecto = c.Decimal(nullable: false, precision: 10, scale: 0),
                        StartDate = c.DateTime(),
                        FinalDate = c.DateTime(),
                        UserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeleterUserId = c.Guid(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "UCASCHEMA.Movement",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MovementNumber = c.Decimal(nullable: false, precision: 10, scale: 0),
                        StockMovement = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        ApplicationDateTime = c.DateTime(nullable: false),
                        Status = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TypeMovement = c.Decimal(nullable: false, precision: 10, scale: 0),
                        MovementCategory = c.Decimal(nullable: false, precision: 10, scale: 0),
                        PreviosCellarQty = c.Double(nullable: false),
                        PreviousCellarAmount = c.Double(nullable: false),
                        PreviousCellarStockQtyInv = c.Double(nullable: false),
                        PreviousCellarStockAmountInv = c.Double(nullable: false),
                        PreviousGeneralInvAmount = c.Double(nullable: false),
                        PreviousGeneralStockAmount = c.Double(nullable: false),
                        CellarId = c.Guid(nullable: false),
                        AssetId = c.Guid(nullable: false),
                        UserId = c.Guid(),
                        InRequestId = c.Guid(),
                        OutRequestId = c.Guid(),
                        AdjustmentId = c.Guid(),
                        ProjectId = c.Guid(),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                        Impreso = c.String(maxLength: 10),
                     })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Adjustment", t => t.AdjustmentId)
                .ForeignKey("UCASCHEMA.Asset", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("UCASCHEMA.Cellar", t => t.CellarId, cascadeDelete: true)
                .ForeignKey("UCASCHEMA.InRequest", t => t.InRequestId)
                .ForeignKey("UCASCHEMA.OutRequest", t => t.OutRequestId)
                .ForeignKey("UCASCHEMA.Project", t => t.ProjectId)
                .ForeignKey("UCASCHEMA.User", t => t.UserId)
                .Index(t => t.CellarId)
                .Index(t => t.AssetId)
                .Index(t => t.UserId)
                .Index(t => t.InRequestId)
                .Index(t => t.OutRequestId)
                .Index(t => t.AdjustmentId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "UCASCHEMA.OutRequest",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RequestNumber = c.Decimal(nullable: false, precision: 10, scale: 0),
                        RequestDocumentNumber = c.String(maxLength: 50),
                        Status = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TypeOutRequest = c.Decimal(nullable: false, precision: 10, scale: 0),
                        SignatureData = c.String(),
                        Description = c.String(nullable: false, maxLength: 150),
                        DeliveredTo = c.String(maxLength: 256),
                        Comment = c.String(maxLength: 512),
                        Notes = c.String(maxLength: 256),
                        DeliverDate = c.DateTime(),
                        AssetsReturnDate = c.DateTime(),
                        AprovedDate = c.DateTime(),
                        IsDelayed = c.Decimal(nullable: false, precision: 1, scale: 0),
                        ProjectId = c.Guid(),
                        CellarId = c.Guid(),
                        ResponsiblePersonId = c.Guid(),
                        WareHouseManId = c.Guid(),
                        ContractorId = c.Guid(),
                        ApprovalUserId = c.Guid(),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        ImagePath1 = c.String(maxLength: 1024),
                        ImagePath2 = c.String(maxLength: 1024),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.User", t => t.ApprovalUserId)
                .ForeignKey("UCASCHEMA.Cellar", t => t.CellarId)
                .ForeignKey("UCASCHEMA.Contractor", t => t.ContractorId)
                .ForeignKey("UCASCHEMA.Project", t => t.ProjectId)
                .ForeignKey("UCASCHEMA.ResponsiblePerson", t => t.ResponsiblePersonId)
                .ForeignKey("UCASCHEMA.User", t => t.WareHouseManId)
                .Index(t => t.ProjectId)
                .Index(t => t.CellarId)
                .Index(t => t.ResponsiblePersonId)
                .Index(t => t.WareHouseManId)
                .Index(t => t.ContractorId)
                .Index(t => t.ApprovalUserId);
            
            CreateTable(
                "UCASCHEMA.Contractor",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractorCode = c.String(nullable: false, maxLength: 64),
                        CompleteName = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        Phone = c.String(maxLength: 48),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "UCASCHEMA.ResponsiblePerson",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Responsable = c.String(maxLength: 350),
                        Name = c.String(maxLength: 350),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "UCASCHEMA.HistoryChange",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InRequestId = c.Guid(),
                        AdjustmentId = c.Guid(),
                        OutRequestId = c.Guid(),
                        AssetId = c.Guid(nullable: false),
                        NameAsset = c.String(maxLength: 100),
                        StockAsset = c.Double(nullable: false),
                        PreviousQty = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Url = c.String(maxLength: 800),
                        OutRequestStatus = c.Decimal(precision: 10, scale: 0),
                        InRequestStatus = c.Decimal(precision: 10, scale: 0),
                        AdjustmentStatus = c.Decimal(precision: 10, scale: 0),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        Status = c.Decimal(nullable: false, precision: 10, scale: 0),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Adjustment", t => t.AdjustmentId)
                .ForeignKey("UCASCHEMA.Asset", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("UCASCHEMA.InRequest", t => t.InRequestId)
                .ForeignKey("UCASCHEMA.OutRequest", t => t.OutRequestId)
                .Index(t => t.InRequestId)
                .Index(t => t.AdjustmentId)
                .Index(t => t.OutRequestId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "UCASCHEMA.PriceChange",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AssetId = c.Guid(nullable: false),
                        OldPrice = c.Double(nullable: false),
                        NewPrice = c.Double(nullable: false),
                        DateChange = c.DateTime(nullable: false),
                        DeleterUserId = c.Guid(),
                        IsDeleted = c.Decimal(precision: 1, scale: 0),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Guid(nullable: false),
                        CompanyName = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("UCASCHEMA.Asset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AssetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("UCASCHEMA.PriceChange", "AssetId", "UCASCHEMA.Asset");
            DropForeignKey("UCASCHEMA.HistoryChange", "OutRequestId", "UCASCHEMA.OutRequest");
            DropForeignKey("UCASCHEMA.HistoryChange", "InRequestId", "UCASCHEMA.InRequest");
            DropForeignKey("UCASCHEMA.HistoryChange", "AssetId", "UCASCHEMA.Asset");
            DropForeignKey("UCASCHEMA.HistoryChange", "AdjustmentId", "UCASCHEMA.Adjustment");
            DropForeignKey("UCASCHEMA.InRequest", "UserId", "UCASCHEMA.User");
            DropForeignKey("UCASCHEMA.InRequest", "ResponsiblePersonId", "UCASCHEMA.ResponsiblePerson");
            DropForeignKey("UCASCHEMA.InRequest", "ProjectId", "UCASCHEMA.Project");
            DropForeignKey("UCASCHEMA.Project", "UserId", "UCASCHEMA.User");
            DropForeignKey("UCASCHEMA.Movement", "UserId", "UCASCHEMA.User");
            DropForeignKey("UCASCHEMA.Movement", "ProjectId", "UCASCHEMA.Project");
            DropForeignKey("UCASCHEMA.Movement", "OutRequestId", "UCASCHEMA.OutRequest");
            DropForeignKey("UCASCHEMA.OutRequest", "WareHouseManId", "UCASCHEMA.User");
            DropForeignKey("UCASCHEMA.OutRequest", "ResponsiblePersonId", "UCASCHEMA.ResponsiblePerson");
            DropForeignKey("UCASCHEMA.OutRequest", "ProjectId", "UCASCHEMA.Project");
            DropForeignKey("UCASCHEMA.Detail", "OutRequestId", "UCASCHEMA.OutRequest");
            DropForeignKey("UCASCHEMA.OutRequest", "ContractorId", "UCASCHEMA.Contractor");
            DropForeignKey("UCASCHEMA.OutRequest", "CellarId", "UCASCHEMA.Cellar");
            DropForeignKey("UCASCHEMA.OutRequest", "ApprovalUserId", "UCASCHEMA.User");
            DropForeignKey("UCASCHEMA.Movement", "InRequestId", "UCASCHEMA.InRequest");
            DropForeignKey("UCASCHEMA.Movement", "CellarId", "UCASCHEMA.Cellar");
            DropForeignKey("UCASCHEMA.Movement", "AssetId", "UCASCHEMA.Asset");
            DropForeignKey("UCASCHEMA.Movement", "AdjustmentId", "UCASCHEMA.Adjustment");
            DropForeignKey("UCASCHEMA.Detail", "InRequestId", "UCASCHEMA.InRequest");
            DropForeignKey("UCASCHEMA.InRequest", "CellarId", "UCASCHEMA.Cellar");
            DropForeignKey("UCASCHEMA.Detail", "AssetId", "UCASCHEMA.Asset");
            DropForeignKey("UCASCHEMA.Detail", "AdjustmentId", "UCASCHEMA.Adjustment");
            DropForeignKey("UCASCHEMA.Adjustment", "CellarId", "UCASCHEMA.Cellar");
            DropForeignKey("UCASCHEMA.Cellar", "WareHouseManagerUser_Id", "UCASCHEMA.User");
            DropForeignKey("UCASCHEMA.Stock", "CellarId", "UCASCHEMA.Cellar");
            DropForeignKey("UCASCHEMA.ToolAsset", "AssetId", "UCASCHEMA.Asset");
            DropForeignKey("UCASCHEMA.Stock", "AssetId", "UCASCHEMA.Asset");
            DropForeignKey("UCASCHEMA.CustomField", "AssetId", "UCASCHEMA.Asset");
            DropForeignKey("UCASCHEMA.Asset", "CategoryId", "UCASCHEMA.Category");
            DropForeignKey("UCASCHEMA.Adjustment", "ApprovalUserId", "UCASCHEMA.User");
            DropIndex("UCASCHEMA.PriceChange", new[] { "AssetId" });
            DropIndex("UCASCHEMA.HistoryChange", new[] { "AssetId" });
            DropIndex("UCASCHEMA.HistoryChange", new[] { "OutRequestId" });
            DropIndex("UCASCHEMA.HistoryChange", new[] { "AdjustmentId" });
            DropIndex("UCASCHEMA.HistoryChange", new[] { "InRequestId" });
            DropIndex("UCASCHEMA.OutRequest", new[] { "ApprovalUserId" });
            DropIndex("UCASCHEMA.OutRequest", new[] { "ContractorId" });
            DropIndex("UCASCHEMA.OutRequest", new[] { "WareHouseManId" });
            DropIndex("UCASCHEMA.OutRequest", new[] { "ResponsiblePersonId" });
            DropIndex("UCASCHEMA.OutRequest", new[] { "CellarId" });
            DropIndex("UCASCHEMA.OutRequest", new[] { "ProjectId" });
            DropIndex("UCASCHEMA.Movement", new[] { "ProjectId" });
            DropIndex("UCASCHEMA.Movement", new[] { "AdjustmentId" });
            DropIndex("UCASCHEMA.Movement", new[] { "OutRequestId" });
            DropIndex("UCASCHEMA.Movement", new[] { "InRequestId" });
            DropIndex("UCASCHEMA.Movement", new[] { "UserId" });
            DropIndex("UCASCHEMA.Movement", new[] { "AssetId" });
            DropIndex("UCASCHEMA.Movement", new[] { "CellarId" });
            DropIndex("UCASCHEMA.Project", new[] { "UserId" });
            DropIndex("UCASCHEMA.InRequest", new[] { "ResponsiblePersonId" });
            DropIndex("UCASCHEMA.InRequest", new[] { "UserId" });
            DropIndex("UCASCHEMA.InRequest", new[] { "ProjectId" });
            DropIndex("UCASCHEMA.InRequest", new[] { "CellarId" });
            DropIndex("UCASCHEMA.Detail", new[] { "AssetId" });
            DropIndex("UCASCHEMA.Detail", new[] { "OutRequestId" });
            DropIndex("UCASCHEMA.Detail", new[] { "AdjustmentId" });
            DropIndex("UCASCHEMA.Detail", new[] { "InRequestId" });
            DropIndex("UCASCHEMA.ToolAsset", new[] { "AssetId" });
            DropIndex("UCASCHEMA.CustomField", new[] { "AssetId" });
            DropIndex("UCASCHEMA.Asset", new[] { "CategoryId" });
            DropIndex("UCASCHEMA.Stock", new[] { "AssetId" });
            DropIndex("UCASCHEMA.Stock", new[] { "CellarId" });
            DropIndex("UCASCHEMA.Cellar", new[] { "WareHouseManagerUser_Id" });
            DropIndex("UCASCHEMA.Adjustment", new[] { "ApprovalUserId" });
            DropIndex("UCASCHEMA.Adjustment", new[] { "CellarId" });
            DropTable("UCASCHEMA.PriceChange");
            DropTable("UCASCHEMA.HistoryChange");
            DropTable("UCASCHEMA.ResponsiblePerson");
            DropTable("UCASCHEMA.Contractor");
            DropTable("UCASCHEMA.OutRequest");
            DropTable("UCASCHEMA.Movement");
            DropTable("UCASCHEMA.Project");
            DropTable("UCASCHEMA.InRequest");
            DropTable("UCASCHEMA.Detail");
            DropTable("UCASCHEMA.ToolAsset");
            DropTable("UCASCHEMA.CustomField");
            DropTable("UCASCHEMA.Category");
            DropTable("UCASCHEMA.Asset");
            DropTable("UCASCHEMA.Stock");
            DropTable("UCASCHEMA.Cellar");
            DropTable("UCASCHEMA.User",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_User_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("UCASCHEMA.Adjustment");
        }
    }
}
