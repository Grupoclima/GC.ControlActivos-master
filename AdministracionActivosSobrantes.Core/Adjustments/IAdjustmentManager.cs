using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Users;
using AdministracionActivosSobrantes.PriceChanges;

namespace AdministracionActivosSobrantes.Adjustments
{
    public interface IAdjustmentManager : IDomainService
    {
        IList<Cellar> GetCellarsList(string companyName);
        IList<User> GetUsersList(string companyName);
        IList<Stock> GetStocksList(string companyName);
        IList<Movement> GetMovementsList(string companyName);
        bool AdjustmentExist(string code, Guid id, string companyName);
        int GetNextRequestNumber(string companyName);
        int GetNextMovementNumber(string companyName);
        User GetUser(Guid? id);
        IQueryable<Stock> SearchAsset(string query, Guid? cellarId, TypeAdjustment? typeAdjustment, string companyName);
        IQueryable<Asset> SearchAsset(string query, string companyName);
        IQueryable<Adjustment> SearchRequests(string query, Guid? cellarId, Guid? project, Guid userId, string companyName);
        IQueryable<Adjustment> GetEdit(Guid id);
        IQueryable<Detail> GetEditDetails(Guid id);
        void Create(Adjustment asset, IList<Detail> details);
        void Delete(Adjustment outRequest, IList<Stock> stocks, IList<Detail> details);
        void Update(Adjustment asset, IList<Detail> newDetails,IList<Detail> updateDetails);
        void ChangeStatus(Adjustment adjustment, IList<Detail> updateDetails, IList<Stock> updatestocks, IList<Movement> movements, IList<PriceChange> pricechanges);
        IList<Stock> GetStocksList(string company, Guid cellarId, Guid assetId);
        Asset GetAssetId(Guid id);
    }
}
