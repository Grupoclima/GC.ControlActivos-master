using System;
using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.ResponsiblePerson;
using AdministracionActivosSobrantes.Stocks.Dto;

namespace GcErpConnection
{
    public interface IGcErpConnectionService : IApplicationService
    {
        void ConnectErp(string userERP, string pass, string company);
        bool CheckUserDatabase(string userERP, string pass, string company);
        void DisconnectErp();
        IList<string> GetErpCompanies();
        IList<Asset> GetErpAssets(string company, Guid user);
        void InsertAssetsDb(IList<Asset> listAssets, Guid userId, string company);
        IList<Project> GetErpProyects(string company, Guid user);
        void InsertProjectsDb(IList<Project> list);
        IList<StockMap> GetAllStocksByCompany(string company);
        IList<ResponsiblePerson> GetResposiblesPersons(string company, Guid user);
        void InsertResponsiblesDb(IList<ResponsiblePerson> list);
        IList<StocksMapReport> GetAllStocksByCompanyReport(string company);
        string getdep(Guid user);
       
    }
}
