using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.PriceChanges;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Adjustments
{
    public class AdjustmentManager : DomainService, IAdjustmentManager
    {
        public IEventBus EventBus { get; set; }
        private readonly IRepository<Adjustment, Guid> _adjustmentRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<Cellar, Guid> _cellarRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<Stock, Guid> _stockRepository;
        private readonly IRepository<Detail, Guid> _detailRepository;
        private readonly IRepository<Movement, Guid> _movementRepository;
        private readonly IRepository<Asset, Guid> _assetRepository;
        private readonly IRepository<PriceChange, Guid> _priceChangeRepository;
        private readonly IDateTime _dateTime;

        public AdjustmentManager(IRepository<Adjustment, Guid> outRequestRepository, IRepository<Project, Guid> projectRepository,
            IRepository<Detail, Guid> detailRepository, IRepository<Cellar, Guid> cellarRepository, IRepository<User, Guid> userRepository,
            IRepository<Stock, Guid> stockRepository, IDateTime dateTime, IRepository<Movement, Guid> movementRepository,
            IRepository<Asset, Guid> assetRepository, IRepository<PriceChange, Guid> priceChangeRepository)
        {
            _adjustmentRepository = outRequestRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _stockRepository = stockRepository;
            _dateTime = dateTime;
            _cellarRepository = cellarRepository;
            _detailRepository = detailRepository;
            _movementRepository = movementRepository;
            _assetRepository = assetRepository;
            _priceChangeRepository = priceChangeRepository;
            EventBus = NullEventBus.Instance;
        }

        public void Create(Adjustment outRequest, IList<Detail> details)
        {
            _adjustmentRepository.Insert(outRequest);
            foreach (var detail in details)
            {
                _detailRepository.Insert(detail);
            }
        }

        public void ChangeStatus(Adjustment adjustment, IList<Detail> updateDetails, IList<Stock> updatestocks, IList<Movement> movements, IList<PriceChange> pricechanges)
        {
            _adjustmentRepository.Update(adjustment);
            foreach (var detail in updateDetails)
            {
                _detailRepository.Update(detail);
            }
            foreach (var stock in updatestocks)
            {
                _stockRepository.Update(stock);
            }
            foreach (var item in movements)
            {
                _movementRepository.Insert(item);
            }
            foreach (var item in pricechanges)
            {
                _priceChangeRepository.Insert(item);
            }
        }

        public int GetNextRequestNumber(string companyName)
        {
            var requestNumber = _adjustmentRepository.GetAllList(a => a.CompanyName.Equals(companyName)).OrderByDescending(a => a.RequestNumber).FirstOrDefault();
            if (requestNumber == null)
                return 1;

            return requestNumber.RequestNumber + 1;
        }

        public int GetNextMovementNumber(string companyName)
        {
            var requestNumber = _movementRepository.GetAllList(a=>a.CompanyName.Equals(companyName)).OrderByDescending(a => a.MovementNumber).FirstOrDefault();
            if (requestNumber == null)
                return 1;

            return requestNumber.MovementNumber + 1;
        }

        public void Update(Adjustment adjustment, IList<Detail> newDetails, IList<Detail> updateDetails)
        {
            _adjustmentRepository.Update(adjustment);
            foreach (var detail in updateDetails)
            {
                detail.AdjustmentId = adjustment.Id;
                _detailRepository.Update(detail);
            }
            foreach (var detail in newDetails)
            {
                detail.AdjustmentId = adjustment.Id;
                _detailRepository.Insert(detail);
            }
        }

        public void ChangeStatus(Adjustment adjustment, IList<Stock> updateStocks, IList<Stock> removeStocks, IList<Detail> updateDetails, IList<Movement> movements)
        {
            _adjustmentRepository.Update(adjustment);
            foreach (var detail in updateDetails)
            {
                detail.AdjustmentId = adjustment.Id;
                _detailRepository.Update(detail);
            }
            foreach (var item in movements)
            {
                item.AdjustmentId = adjustment.Id;
                _movementRepository.Insert(item);
            }
            foreach (var stock in updateStocks)
            {
                _stockRepository.Update(stock);
            }
            foreach (var stock in removeStocks)
            {
                _stockRepository.Update(stock);
            }
        }

        public bool AdjustmentExist(string code, Guid id, string companyName)
        {
            var @entity = _adjustmentRepository.FirstOrDefault(e => e.RequestDocumentNumber == code && e.Id != id && e.IsDeleted.Value == false && e.CompanyName.Equals(companyName));
            return @entity != null;
        }

        public IList<Project> GetProjectsList(string companyName)
        {
            var @entities = _projectRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(companyName)).OrderBy(e => e.Name).ToList();
            return @entities;
        }

        public IList<Cellar> GetCellarsList(string companyName)
        {
            var @entities = _cellarRepository.GetAllList(e => e.IsDeleted.Value == false && e.Active && e.CompanyName.Equals(companyName)).OrderBy(e => e.Name).ToList();
            return @entities;
        }

        public IList<User> GetUsersList(string companyName)
        {
            var @entities = _userRepository.GetAllList(e => e.IsDeleted == false && e.CompanyName.Equals(companyName));
            return @entities;
        }

        public IList<Stock> GetStocksList(string companyName)
        {
            var @entities = _stockRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(companyName));
            return @entities;
        }

        public IList<Movement> GetMovementsList(string companyName)
        {
            var @entities = _movementRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(companyName));
            return @entities;
        }

        public User GetUser(Guid? id)
        {
            if (id == null)
                return null;

            Guid idTemp = id.Value;
            var @entity = _userRepository.Get(idTemp);
            return @entity;
        }

        public IQueryable<Stock> SearchAsset(string q, Guid? cellarId, TypeAdjustment? typeAdjustment, string companyName)
        {
            string query = "";
            if (q != null)
                query = q.ToLower();
            var temp = (int)typeAdjustment;
            var temp1 = (AssetType)temp;
            var outRequests = _stockRepository.GetAll().Where(a => a.CellarId == cellarId && a.Asset.AssetType == temp1);
            outRequests = outRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(companyName) && a.Asset.Name.ToLower().Contains(query) || a.Asset.Name.ToLower().Equals(query) || a.Asset.Code.ToLower().Contains(query) || a.Asset.Code.ToLower().Equals(query));
            return outRequests.Include(a => a.Asset).Include(a => a.Cellar).Include(a => a.Asset.Category).OrderBy(a => a.Asset.Name);
        }

        public IQueryable<Asset> SearchAsset(string q, string companyName)
        {
            string query = "";
            if (q != null)
                query = q.ToLower();

            var assets = _assetRepository.GetAll();
            assets = assets.Where(a => a.IsDeleted == false && a.CompanyName.Equals(companyName) && a.Name.ToLower().Contains(query) || a.Name.ToLower().Equals(query) || a.Code.ToLower().Contains(query) || a.Code.ToLower().Equals(query));
            return assets.Include(a => a.Category).OrderBy(a => a.Name);
        }

        public IQueryable<Adjustment> SearchRequests(string query, Guid? cellarId, Guid? projectId, Guid userId, string companyName)
        {
            var user = GetUser(userId);
            var outRequests = _adjustmentRepository.GetAll();
            outRequests = outRequests.Where(a => a.CompanyName.Equals(companyName));

            if (cellarId != null)
                outRequests = outRequests.Where(a => a.CellarId == cellarId);

            int? noOrden;
            try
            {
                noOrden = Convert.ToInt32(query);
                if (noOrden != 0)
                    outRequests = outRequests.Where(a => a.RequestNumber == noOrden);
            }
            catch (Exception e)
            {
                noOrden = null;
            }
            if (noOrden == null)
            {
                string q = query.ToLower();
                outRequests = outRequests.Where(a => a.Cellar.Name.ToLower().Contains(q) || a.Cellar.Name.ToLower().Equals(q)
                    || a.RequestDocumentNumber.ToLower().Contains(q) || a.RequestDocumentNumber.ToLower().Equals(q));
            }

            if (user.Rol == UserRoles.SuperAdministrador || user.Rol == UserRoles.SupervisorUca)
                outRequests = outRequests.Where(a => a.IsDeleted == false);
            else if (user.Rol == UserRoles.Solicitante)
                outRequests = outRequests.Where(a => a.IsDeleted == false && a.CreatorUserId == userId).Include(a => a.Details).Include(a => a.Cellar).OrderBy(a => a.RequestNumber);
            else if (user.Rol == UserRoles.AuxiliarUca)
                outRequests = outRequests.Where(a => a.IsDeleted == false).Include(a => a.Details).Include(a => a.Cellar).OrderBy(a => a.RequestNumber);
            else if (user.Rol == UserRoles.Coordinador)
                outRequests = outRequests.Where(a => a.IsDeleted == false).Include(a => a.Details).Include(a => a.Cellar).OrderBy(a => a.RequestNumber);

            return outRequests;
        }

        public IQueryable<Adjustment> GetEdit(Guid id)
        {
            var outRequests = _adjustmentRepository.GetAll().Where(a => a.IsDeleted == false && a.Id == id).Include(a => a.Details).Include(a => a.Cellar).OrderBy(a => a.RequestNumber);
            return outRequests;
        }

        public IQueryable<Detail> GetEditDetails(Guid id)
        {
            var details = _detailRepository.GetAll().Where(a => a.IsDeleted == false && a.AdjustmentId == id).Include(a => a.Asset).OrderBy(a => a.NameAsset);
            return details;
        }


        public void Delete(Adjustment outRequest, IList<Stock> stocks, IList<Detail> details)
        {
            _adjustmentRepository.Update(outRequest);
            foreach (var detail in details)
            {
                detail.AdjustmentId = outRequest.Id;
                _detailRepository.Update(detail);
            }
            foreach (var stock in stocks)
            {
                _stockRepository.Update(stock);
            }
        }

        public IList<Stock> GetStocksList(string company, Guid cellarId, Guid assetId)
        {
            var @entities = _stockRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company)
                && e.CellarId == cellarId && e.AssetId == assetId);
            return @entities;
        }

        public Asset GetAssetId(Guid id)
        {
            var asset = _assetRepository.Get(id);
            return asset;
        }
    }
}
