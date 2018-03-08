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
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;

namespace AdministracionActivosSobrantes.InRequest
{
    public class InRequestManager : DomainService, IInRequestManager
    {
        public IEventBus EventBus { get; set; }
        private readonly IRepository<InRequest, Guid> _inRequestRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<Cellar, Guid> _cellarRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<Stock, Guid> _stockRepository;
        private readonly IRepository<Detail, Guid> _detailRepository;
        private readonly IRepository<Movement, Guid> _movementRepository;
        private readonly IRepository<Asset, Guid> _assetRepository;
        private readonly IRepository<PriceChange, Guid> _priceChangeRepository;
        private readonly IDateTime _dateTime;

        public InRequestManager(IRepository<InRequest, Guid> inRequestRepository, IRepository<Project, Guid> projectRepository,
            IRepository<Detail, Guid> detailRepository, IRepository<Cellar, Guid> cellarRepository, IRepository<User, Guid> userRepository,
            IRepository<Stock, Guid> stockRepository, IDateTime dateTime, IRepository<Movement, Guid> movementRepository,
            IRepository<Asset, Guid> assetRepository, IRepository<PriceChange, Guid> priceChangeRepository)
        {
            _inRequestRepository = inRequestRepository;
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

        public IList<Cellar> GetCellarsList(string company)
      {
            var @entities = _cellarRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company) && e.Active).OrderBy(e=>e.Name).ToList();;
            return @entities;
        }

        public IList<User> GetUsersList(string company)
        {
            var @entities = _userRepository.GetAllList(e => e.IsDeleted == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public IList<Stock> GetStocksList(string company)
        {
            var @entities = _stockRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
            return @entities;
        }

        
        public IList<Movement> GetMovementsList(string company)
        {
            var @entities = _movementRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public bool InRequestExist(string code, Guid id, string company)
        {
            var @entity = _inRequestRepository.FirstOrDefault(e => e.RequestDocumentNumber == code && e.CompanyName.Equals(company) && e.Id != id && e.IsDeleted.Value == false);
            return @entity != null;
        }

        public int GetNextRequestNumber(string company)
        {
            var requestNumber = _inRequestRepository.GetAllList(e=>e.CompanyName.Equals(company)).OrderByDescending(a => a.RequestNumber).FirstOrDefault();
            if (requestNumber == null)
                return 1;

            return requestNumber.RequestNumber + 1;
        }

        public int GetNextMovementNumber(string company)
        {
            var requestNumber = _movementRepository.GetAllList(e => e.CompanyName.Equals(company)).OrderByDescending(a => a.MovementNumber).FirstOrDefault();
            if (requestNumber == null)
                return 1;

            return requestNumber.MovementNumber + 1;
        }

        public User GetUser(Guid? id)
        {
            if (id == null)
                return null;

            Guid idTemp = id.Value;
            var @entity = _userRepository.Get(idTemp);
            return @entity;
        }

        public IQueryable<Stock> SearchAsset(string q, Guid? cellarId, Guid? assetId, string company)
        {
            string query = "";
            if (q != null)
                query = q.ToLower();

            var outRequests = _stockRepository.GetAll().Where(a => a.CellarId == cellarId);
            outRequests = outRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company) 
                && a.Asset.Name.ToLower().Contains(query) || a.Asset.Name.ToLower().Equals(query) || 
                a.Asset.Code.ToLower().Contains(query) || a.Asset.Code.ToLower().Equals(query));
            return outRequests.Include(a => a.Asset).Include(a => a.Cellar).Include(a => a.Asset.Category).OrderBy(a => a.Asset.Name);
        }

        public IQueryable<InRequest> SearchRequests(string query, Guid? cellarId, Guid userId, string company)
        {
            var inRequests = _inRequestRepository.GetAll();
            var user = GetUser(userId);

            if (cellarId != null)
                inRequests = inRequests.Where(a => a.CellarId == cellarId);

            int? noOrden;
            try
            {
                noOrden = Convert.ToInt32(query);
                if (noOrden != 0)
                    inRequests = inRequests.Where(a => a.RequestNumber == noOrden);
            }
            catch (Exception e)
            {
                noOrden = null;
            }
            if (noOrden == null)
            {
                string q = query.ToLower();
                inRequests = inRequests.Where(a => a.Cellar.Name.ToLower().Contains(q) || a.Cellar.Name.ToLower().Equals(q) || a.Project.Name.ToLower().Contains(q) || a.Project.Name.ToLower().Equals(q)
                    || a.RequestDocumentNumber.ToLower().Contains(q) || a.RequestDocumentNumber.ToLower().Equals(q));
            }

            if (user.Rol == UserRoles.SuperAdministrador || user.Rol == UserRoles.SupervisorUca)
                inRequests = inRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company)).OrderByDescending(a=> a.RequestNumber);
            else if (user.Rol == UserRoles.Solicitante)
                inRequests = inRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company) && a.CreatorUserId == userId).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderBy(a => a.RequestNumber);
            else
                inRequests = inRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company)).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderByDescending(a => a.RequestNumber);
            //inRequests = inRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company)).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderBy(a => a.RequestNumber); ;

            return inRequests;
        }

        public IQueryable<InRequest> GetEdit(Guid id)
        {
            var outRequests = _inRequestRepository.GetAll().Where(a => a.IsDeleted == false && a.Id == id).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderBy(a => a.RequestNumber);

            return outRequests;
        }

        public IQueryable<Detail> GetEditDetails(Guid id)
        {
            var details = _detailRepository.GetAll().Where(a => a.IsDeleted == false && a.InRequestId == id).Include(a => a.Asset).OrderBy(a => a.NameAsset);
            return details;
        }

        //antes de stock decia asset
        public IQueryable<Asset> SearchAsset(string q, TypeInRequest type, string company)
        {
            string query = "";
            if (q != null)
                query = q.ToLower();
           
            var temp = (int)type;
            var temp1 = (AssetType)temp;
            var assets = _assetRepository.GetAll().Where(a => a.AssetType == temp1 && a.CompanyName.Equals(company));
            //assets = assets._stockRepository.GetAll().Where(b => b.AssetQtyInputs - b.AssetQtyOutputs - b.AssetQtyOutputsBlocked == 0);
            assets = assets.Where(a => a.IsDeleted == false && a.Name.ToLower().Contains(query) || a.Name.ToLower().Equals(query) || a.Code.ToLower().Contains(query) || a.Code.ToLower().Equals(query));
            return assets.Include(a => a.Category).Include(a => a.Stocks).OrderBy(a => a.Name);
        }

        public void Create(InRequest inRequest, IList<Detail> details)
        {
            _inRequestRepository.Insert(inRequest);
            foreach (var detail in details)
            {
                _detailRepository.Insert(detail);
            }
        }

        public void CreateWithMovements(InRequest inRequest, IList<Stock> newstocks, IList<Stock> updatestocks, IList<Detail> detailsIList, IList<Movement> movements, IList<PriceChange> pricechanges)
        {
            _inRequestRepository.Insert(inRequest);
            foreach (var detail in detailsIList)
            {
                detail.InRequestId = inRequest.Id;
                _detailRepository.Insert(detail);
            }
            foreach (var stock in updatestocks)
            {
                _stockRepository.Update(stock);
            }
            foreach (var stock in newstocks)
            {
                _stockRepository.Insert(stock);
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

        public void Delete(InRequest inRequest, IList<Detail> details)
        {
            _inRequestRepository.Update(inRequest);
            foreach (var detail in details)
            {
                detail.InRequestId = inRequest.Id;
                _detailRepository.Update(detail);
            }
        }

        public void Update(InRequest inRequest, IList<Detail> newDetails, IList<Detail> updateDetails)
        {
            _inRequestRepository.Update(inRequest);
            foreach (var detail in updateDetails)
            {
                detail.InRequestId = inRequest.Id;
                _detailRepository.Update(detail);
            }
            foreach (var detail in newDetails)
            {
                detail.InRequestId = inRequest.Id;
                _detailRepository.Insert(detail);
            }
        }

        public void ChangeStatus(InRequest inRequest, IList<Detail> updateDetails, IList<Stock> newstocks, IList<Stock> updatestocks, IList<Movement> movements, IList<PriceChange> pricechanges)
        {
            _inRequestRepository.Update(inRequest);
            foreach (var detail in updateDetails)
            {
                detail.InRequestId = inRequest.Id;
                _detailRepository.Update(detail);
            }
            foreach (var stock in updatestocks)
            {
                _stockRepository.Update(stock);
            }
            foreach (var stock in newstocks)
            {
                _stockRepository.Insert(stock);
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
        public Stock GetStockAsset(Guid id, string companyName)
        {
            return _stockRepository.GetAll().Where(a => a.AssetId == id && a.CompanyName.Equals(companyName)).FirstOrDefault();
        }

        public Asset GetAssetBarcode(string code, string companyName)
        {
            return _assetRepository.GetAll().Where(a => a.Barcode.Equals(code) && a.CompanyName.Equals(companyName)).FirstOrDefault();
        }

    }
}
