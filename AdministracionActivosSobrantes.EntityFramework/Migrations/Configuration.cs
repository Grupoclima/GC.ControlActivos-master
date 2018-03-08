using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.EntityFramework;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AdministracionActivosSobrantesDbContext>
    {
        private const string _companyName = "TRANS1";
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "AdministracionActivosSobrantes";
        }

        protected override void Seed(AdministracionActivosSobrantes.EntityFramework.AdministracionActivosSobrantesDbContext context)
        {
            // This method will be called every time after migrating to the latest version.
            // You can add any seed data here...
            User saUser = new User();
            saUser.UserCode = "1-sauser";
            saUser.CompleteName = "Daniel Viquez";
            saUser.Id = new Guid();
            saUser.UserName = "saUser";
            saUser.Password = "admin123";
            saUser.Rol = UserRoles.SuperAdministrador;
            saUser.CompanyName = _companyName;
            context.Users.Add(saUser);

            User adminUser = new User();
            adminUser.UserCode = "1-admin";
            adminUser.CompleteName = "Daniel Viquez";
            adminUser.Id = new Guid();
            adminUser.UserName = "adminUser";
            adminUser.Password = "admin123";
            adminUser.Rol = UserRoles.SupervisorUca;
            adminUser.CompanyName = _companyName;
            context.Users.Add(adminUser);

            User bodUser = new User();
            bodUser.UserCode = "1-auxiliar";
            bodUser.CompleteName = "Daniel Viquez";
            bodUser.Id = new Guid();
            bodUser.UserName = "AuxiliarUca";
            bodUser.Password = "admin123";
            bodUser.Rol = UserRoles.AuxiliarUca;
            bodUser.CompanyName = _companyName;
            context.Users.Add(bodUser);

            User coorUser = new User();
            coorUser.UserCode = "1-dviquez";
            coorUser.CompleteName = "Daniel Viquez";
            coorUser.Id = new Guid();
            coorUser.UserName = "SolicitantePrivilegio";
            coorUser.Password = "admin123";
            coorUser.Rol = UserRoles.Coordinador;
            coorUser.CompanyName = _companyName;
            context.Users.Add(coorUser);

            User techUser = new User();
            techUser.UserCode = "1-solicitante";
            techUser.CompleteName = "Daniel Viquez";
            techUser.Id = new Guid();
            techUser.UserName = "Solicitante";
            techUser.Password = "admin123";
            techUser.Rol = UserRoles.Solicitante;
            techUser.CompanyName = _companyName;
            context.Users.Add(techUser);

            IList<Cellar> cellarList = new List<Cellar>();
            IList<Category> categoryList = new List<Category>();
            for (int i = 1; i < 2; i++)
            {
                Cellar cellar = Cellar.Create("Bodega" + i, "Direccion" + i, "8888888" + i, bodUser.Id, "", "", saUser.Id, DateTime.Now, "Centro Costo " + i, _companyName);
                context.Cellars.Add(cellar);
                cellarList.Add(cellar);
            }
           
            for (int i = 1; i < 11; i++)
            {
                Category category = Category.Create("Categoria " + i, "Descripción" + i, saUser.Id, DateTime.Now, _companyName);
                context.Categories.Add(category);
                categoryList.Add(category);
            }
            int index = 0;
            for (int i = 1; i < 11; i++)
            {
                Project project = Project.Create("Proyecto " + i, "Code" + i, "Descripción" + i, DateTime.Now, DateTime.Now, EstadoProyecto.Active, saUser.Id, "Centro Costo " + i, _companyName);
                context.Projects.Add(project);
            }
            index = 0;
            for (int i = 11; i < 22; i++)
            {
                Asset asset = Asset.Create("Clavos " + i, "Descripción 1" + i, "Code" + i,  "Code" + i, DateTime.Now, null, 1000,10,AssetType.Asset, categoryList[0].Id, saUser.Id, DateTime.Now,"", "", "", "", false,_companyName);
                context.Assets.Add(asset);
                Stock stock = Stock.Create(cellarList[0].Id, asset.Id, 10, 1000,saUser.Id, DateTime.Now,_companyName);
                context.Stocks.Add(stock);
                index++;
            }
            index = 0;
            for (int i = 1; i < 11; i++)
            {
                Asset asset = Asset.Create("Martillos " + i, "Descripción 222" + i, "Code" + i, "Code" + i, DateTime.Now, null, 2000, 20, AssetType.Asset, categoryList[1].Id, saUser.Id, DateTime.Now, "", "", "", "", false,_companyName);
                context.Assets.Add(asset);
                Stock stock = Stock.Create(cellarList[0].Id, asset.Id, 10, 1000, saUser.Id, DateTime.Now,_companyName);
                context.Stocks.Add(stock);
                index++;
            }
            context.SaveChanges();
        }
    }
}
