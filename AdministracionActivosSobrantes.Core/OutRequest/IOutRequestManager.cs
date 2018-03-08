using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Contractors;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.HistoryChanges;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.OutRequest
{
    public interface IOutRequestManager : IDomainService
    {
        IList<Project> GetProjectsList(string company);
        IList<Cellar> GetCellarsList(string company);
        IList<ResponsiblePerson.ResponsiblePerson> GetResponsiblesList(string company);
        IList<User> GetUsersList(string company);
        IList<User> GetUsers(Func<User, bool> predicate);
        IList<Stock> GetStocksList(string company, Guid cellarId, Guid assetId);
        IList<Movement> GetMovementsList(string company);
        IList<Contractor> GetContractorList(string company);
        bool OutRequestExist(string code, Guid id, string company);
        int GetNextRequestNumber(string company);
        int GetNextMovementNumber(string company);
        User GetUser(Guid? id);
        IQueryable<Stock> SearchAsset(string query, Guid? cellarId, Guid? assetId, TypeOutRequest type, string company);
        IQueryable<OutRequest> SearchRequests(string query, Guid? cellarId, Guid? project, Guid userId, int? status, string company, DateTime? date, DateTime? date2);
        IQueryable<OutRequest> GetEdit(Guid id);
        IQueryable<Detail> GetEditDetails(Guid id);
        void Create(OutRequest asset, IList<Stock> stocks, IList<Detail> details);
        void CreateWithMovements(OutRequest outRequest, IList<Stock> stocks, IList<Detail> detailsIList, IList<Movement> movements);
        void Delete(OutRequest outRequest, IList<Stock> stocks, IList<Detail> details);
        void Update(OutRequest asset, IList<Stock> updateStocks, IList<Stock> removeStocks, IList<Detail> newDetails,
            IList<Detail> removeDetails, IList<Detail> updateDetails);
        void CloseRequest(OutRequest asset, IList<Stock> updateStocks, IList<Movement> movements, IList<Detail> updateDetails);
        void ChangeStatus(OutRequest outRequest, IList<Stock> updateStocks, IList<Stock> removeStocks,
            IList<Detail> updateDetails, IList<Movement> movements);
        void UpdatePrintedDetails(IList<Detail> details);
        Asset GetAsset(Guid id);
        void AddHistoryChanges(Guid outRequest, IList<HistoryChange> historyChanges);
        Asset GetAssetBarcode(string code, string companyName);
        Stock GetStockAsset(Guid id, string companyName, Guid cellarId);
        void UpdateDetails(IList<Detail> details);
        IList<Cellar> GetSomeAllCellarsList(string company);
    }
}
