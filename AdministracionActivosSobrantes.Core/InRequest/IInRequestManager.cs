using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.PriceChanges;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.InRequest
{
    public interface IInRequestManager : IDomainService
    {
        IList<Cellar> GetCellarsList(string company);
        IList<User> GetUsersList(string company);
        IList<Stock> GetStocksList(string company);
        IList<Movement> GetMovementsList(string company);
        bool InRequestExist(string code, Guid id, string company);
        int GetNextRequestNumber(string company);
        int GetNextMovementNumber(string company);
        User GetUser(Guid? id);
        IQueryable<Stock> SearchAsset(string query, Guid? cellarId, Guid? assetId, string company);
        IQueryable<InRequest> SearchRequests(string query, Guid? cellarId, Guid userId, string company);
        IQueryable<InRequest> GetEdit(Guid id);
        IQueryable<Detail> GetEditDetails(Guid id);
        IQueryable<Asset> SearchAsset(string query, TypeInRequest type, string company);
        void Create(InRequest inRequest, IList<Detail> details);
        void CreateWithMovements(InRequest inRequest, IList<Stock> newstocks, IList<Stock> updatestocks, IList<Detail> detailsIList, IList<Movement> movements, IList<PriceChange> pricechanges);
        void Delete(InRequest inRequest, IList<Detail> details);
        void Update(InRequest inRequest,IList<Detail> newDetails, IList<Detail> updateDetails);
        void ChangeStatus(InRequest inRequest, IList<Detail> updateDetails, IList<Stock> newstocks,IList<Stock> updatestocks, IList<Movement> movements, IList<PriceChange> pricechanges);
        IList<Stock> GetStocksList(string company, Guid cellarId, Guid assetId);
        Asset GetAssetId(Guid id);
        Asset GetAssetBarcode(string code, string companyName);
        Stock GetStockAsset(Guid id, string companyName);
    }
}
