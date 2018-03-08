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
using AdministracionActivosSobrantes.Contractors;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.HistoryChanges;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.OutRequest
{
    public class OutRequestManager : DomainService, IOutRequestManager
    {
        public IEventBus EventBus { get; set; }
        private readonly IRepository<OutRequest, Guid> _outRequestRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<Cellar, Guid> _cellarRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<Stock, Guid> _stockRepository;
        private readonly IRepository<Detail, Guid> _detailRepository;
        private readonly IRepository<Movement, Guid> _movementRepository;
        private readonly IRepository<Contractor, Guid> _contractorRepository;
        private readonly IRepository<Asset, Guid> _assetRepository;
        private readonly IRepository<HistoryChange, Guid> _historyChangesRepository;
        private readonly IRepository<ResponsiblePerson.ResponsiblePerson, Guid> _responsibleRepository;
        private readonly IDateTime _dateTime;

        public OutRequestManager(IRepository<OutRequest, Guid> outRequestRepository, IRepository<Project, Guid> projectRepository,
            IRepository<Detail, Guid> detailRepository, IRepository<Cellar, Guid> cellarRepository, IRepository<User, Guid> userRepository,
            IRepository<Stock, Guid> stockRepository, IDateTime dateTime, IRepository<Movement, Guid> movementRepository,
            IRepository<Contractor, Guid> contractorRepository, IRepository<Asset, Guid> assetRepository, IRepository<HistoryChange, Guid> historyChangesRepository,
            IRepository<ResponsiblePerson.ResponsiblePerson, Guid> responsibleRepository)
        {
            _outRequestRepository = outRequestRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _stockRepository = stockRepository;
            _dateTime = dateTime;
            _cellarRepository = cellarRepository;
            _contractorRepository = contractorRepository;
            _detailRepository = detailRepository;
            _movementRepository = movementRepository;
            _assetRepository = assetRepository;
            _historyChangesRepository = historyChangesRepository;
            _responsibleRepository = responsibleRepository;
            EventBus = NullEventBus.Instance;
        }

        public void Create(OutRequest outRequest, IList<Stock> stocks, IList<Detail> details)
        {
            _outRequestRepository.Insert(outRequest);
            foreach (var detail in details)
            {
                _detailRepository.Insert(detail);
            }
            foreach (var stock in stocks)
            {
                _stockRepository.Update(stock);
            }
        }

        public void UpdatePrintedDetails( IList<Detail> details)
        {
            foreach (var detail in details)
            {
                _detailRepository.Insert(detail);
            }
        }

        public void CreateWithMovements(OutRequest outRequest, IList<Stock> stocks, IList<Detail> details, IList<Movement> movements)
        {
            _outRequestRepository.Insert(outRequest);
            foreach (var detail in details)
            {
                detail.OutRequestId = outRequest.Id;
                _detailRepository.Insert(detail);
            }
            foreach (var stock in stocks)
            {
                _stockRepository.Update(stock);
            }
            foreach (var item in movements)
            {
                _movementRepository.Insert(item);
            }
        }

        public int GetNextRequestNumber(string company)
        {
            var requestNumber = _outRequestRepository.GetAllList(e => e.CompanyName.Equals(company)).OrderByDescending(a => a.RequestNumber).FirstOrDefault();
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

        public void AddHistoryChanges(Guid outRequest, IList<HistoryChange> historyChanges)
        {
            foreach (var history in historyChanges)
            {
                history.OutRequestId = outRequest;
                history.Url = "";
                _historyChangesRepository.Insert(history);
            }

        }

        public void Update(OutRequest outRequest, IList<Stock> updateStocks, IList<Stock> removeStocks, IList<Detail> newDetails,
           IList<Detail> removeDetails, IList<Detail> updateDetails)
        {
            _outRequestRepository.Update(outRequest);
            foreach (var detail in updateDetails)
            {
                detail.OutRequestId = outRequest.Id;
                _detailRepository.Update(detail);
            }
            foreach (var detail in newDetails)
            {
                detail.OutRequestId = outRequest.Id;
                _detailRepository.Insert(detail);
            }
            foreach (var detail in removeDetails)
            {
                detail.OutRequestId = outRequest.Id;
                _detailRepository.Update(detail);
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

        public void ChangeStatus(OutRequest outRequest, IList<Stock> updateStocks, IList<Stock> removeStocks, IList<Detail> updateDetails, IList<Movement> movements)
        {
            _outRequestRepository.Update(outRequest);
            foreach (var detail in updateDetails)
            {
                detail.OutRequestId = outRequest.Id;
                _detailRepository.Update(detail);
            }
            foreach (var item in movements)
            {
                item.OutRequestId = outRequest.Id;
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

        public void CloseRequest(OutRequest outRequest, IList<Stock> updateStocks, IList<Movement> movements, IList<Detail> updateDetails)
        {
            _outRequestRepository.Update(outRequest);
            foreach (var item in updateDetails)
            {
                _detailRepository.Update(item);
            }
            foreach (var item in movements)
            {
                _movementRepository.Insert(item);
            }
            foreach (var stock in updateStocks)
            {
                _stockRepository.Update(stock);
            }

        }

        public bool OutRequestExist(string code, Guid id, string company)
        {
            var @entity = _outRequestRepository.FirstOrDefault(e => e.RequestDocumentNumber == code && e.CompanyName.Equals(company) && e.Id != id && e.IsDeleted.Value == false);
            return @entity != null;
        }

        public IList<Project> GetProjectsList(string company)
        {
            var @entities = _projectRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company)).OrderBy(e => e.Name).ToList();
            return @entities;
        }

        public IList<Cellar> GetCellarsList(string company)
        {
            var @entities = _cellarRepository.GetAllList(e => e.IsDeleted.Value == false && e.Active && e.CompanyName.Equals(company)).OrderBy(e => e.Name).ToList();
            return @entities;
        }
        public IList<Cellar> GetSomeAllCellarsList(string company)
        {

            var @entities = _cellarRepository.GetAllList(e => e.IsDeleted.Value == false && e.Active && e.CompanyName.Equals(company) &&
                e.CellarNumber.Equals(20) && e.CompanyName.Equals(company)||
                e.CellarNumber.Equals(21) && e.CompanyName.Equals(company) ||
                e.CellarNumber.Equals(16) && e.CompanyName.Equals(company) ||
                e.CellarNumber.Equals(11) && e.CompanyName.Equals(company) ||
                e.CellarNumber.Equals(6) && e.CompanyName.Equals(company) ||
                e.CellarNumber.Equals(1) && e.CompanyName.Equals(company) ||
                e.CellarNumber.Equals(15) && e.CompanyName.Equals(company) ||
                e.CellarNumber.Equals(10) && e.CompanyName.Equals(company) ||
                e.CellarNumber.Equals(5) && e.CompanyName.Equals(company) ||
                e.CellarNumber.Equals(0) && e.CompanyName.Equals(company)).OrderBy(e => e.Name).ToList();
            return @entities;
        }


        public IList<ResponsiblePerson.ResponsiblePerson> GetResponsiblesList(string company)
        {
            var @entities = _responsibleRepository.GetAllList(e => e.IsDeleted.Value == false  && e.CompanyName.Equals(company)).OrderBy(e => e.Name).ToList();
            return @entities;
        }

        public IList<User> GetUsersList(string company)
        {
            var @entities = _userRepository.GetAllList(e => e.IsDeleted == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public IList<Stock> GetStocksList(string company, Guid cellarId, Guid assetId)
        {
            var @entities = _stockRepository.GetAllList(e => e.IsDeleted.Value == false /*&& e.CompanyName.Equals(company)*/
                && e.CellarId == cellarId && e.AssetId == assetId);
            return @entities;
        }

        public IList<Movement> GetMovementsList(string company)
        {
            var @entities = _movementRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public IList<Contractor> GetContractorList(string company)
        {
            var @entities = _contractorRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
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

        public IList<User> GetUsers(Func<User, bool> predicate)
        {
            var @entities = _userRepository.GetAll().Where(predicate);
            return @entities.ToList();
        }


        public IQueryable<Stock> SearchAsset(string q, Guid? cellarId, Guid? assetId, TypeOutRequest type, string company)
        {
            string query = "";
            if (q != null)
                query = q.ToLower();
            var temp = (int)type;
            var temp1 = (AssetType)temp;
            var outRequests = _stockRepository.GetAll().Where(a => a.CellarId == cellarId && a.Asset.AssetType == temp1 && a.CompanyName.Equals(company));
            outRequests = outRequests.Where(a => a.IsDeleted == false && a.Asset.Name.ToLower().Contains(query) || a.Asset.Name.ToLower().Equals(query) || a.Asset.Code.ToLower().Contains(query) || a.Asset.Code.ToLower().Equals(query));
            return outRequests.Include(a => a.Asset).Include(a => a.Cellar).Include(a => a.Asset.Category).OrderByDescending(a => a.AssetQtyInputs-a.AssetQtyOutputs-a.AssetQtyOutputsBlocked > 0);
        }

        public IQueryable<OutRequest> SearchRequests(string query, Guid? cellarId, Guid? projectId, Guid userId, int? status, string company, DateTime? date1, DateTime? date2)
        {
            var user = GetUser(userId);
            var outRequests = _outRequestRepository.GetAll().Where( a=> a.CompanyName == company);

            if (cellarId != null)
                outRequests = outRequests.Where(a => a.CellarId == cellarId);

            if (projectId != null)
                outRequests = outRequests.Where(a => a.ProjectId == projectId);

            /* if (date1 != null && date2 != null)
                 outRequests = outRequests.Where(a => a.AssetsReturnDate.Value.Year == date1.Value.Year
                             && a.AssetsReturnDate.Value.Month == date.Value.Month
                             && a.AssetsReturnDate.Value.Day == date.Value.Day);*/
             if(date2 == null)
            {
                date2 = System.DateTime.Today.AddDays(+1);
            }
            if(date1 == null)
            {
                date1 = System.DateTime.Today.AddDays(-20);      
            }

            if (date1 != null && date2 != null)
                 outRequests = outRequests.Where(a => a.CreationTime >= date1.Value && a.CreationTime <= date2.Value);
            //DbFunctions.TruncateTime(r.savedDate) >= DbFunctions.TruncateTime(calDate)

            if (status != null)
            {
                OutRequestStatus temp = (OutRequestStatus)status;
                outRequests = outRequests.Where(a => a.Status == temp);
            }

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
                outRequests = outRequests.Where(a => a.Cellar.Name.ToLower().Contains(q) || a.Cellar.Name.ToLower().Equals(q) || a.Project.Name.ToLower().Contains(q) || a.Project.Name.ToLower().Equals(q)
                    || a.RequestDocumentNumber.ToLower().Contains(q) || a.RequestDocumentNumber.ToLower().Equals(q)
                    || a.DeliveredTo.ToLower().Contains(q) || a.DeliveredTo.ToLower().Equals(q));
            }

            if (user.Rol == UserRoles.SuperAdministrador || user.Rol == UserRoles.SupervisorUca)
                outRequests = outRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company)).OrderBy(a => a.Status);
            else if (user.Rol == UserRoles.Solicitante)
                outRequests = outRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company) && a.CreatorUserId == userId).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderBy(a => a.Status);
            else if (user.Rol == UserRoles.AuxiliarUca)
                outRequests = outRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company) &&
                    (a.CreatorUserId == userId && (int)a.Status == (int)OutRequestStatus.Active || a.CreatorUserId == userId && (int)a.Status == (int)OutRequestStatus.WaitApproval)
                    || (int)a.Status == (int)OutRequestStatus.Backorder || (int)a.Status == (int)OutRequestStatus.ProcessedInWareHouse || (int)a.Status == (int)OutRequestStatus.Confirmado ||
                    (int)a.Status == (int)OutRequestStatus.Approved).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderBy(a => a.Status);
            else if (user.Rol == UserRoles.Coordinador)
                outRequests = outRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company) &&
                    (a.CreatorUserId == userId && (int)a.Status == (int)OutRequestStatus.Active || a.CreatorUserId == userId
                    && (int)a.Status == (int)OutRequestStatus.Confirmado || a.CreatorUserId == userId && (int)a.Status == (int)OutRequestStatus.Backorder
                    || a.CreatorUserId == userId && (int)a.Status == (int)OutRequestStatus.ProcessedInWareHouse)
                    || (int)a.Status == (int)OutRequestStatus.WaitApproval || (int)a.Status == (int)OutRequestStatus.Approved).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderBy(a => a.Status);
            else
                outRequests = outRequests.Where(a => a.IsDeleted == false && a.CompanyName.Equals(company)).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderBy(a => a.Status);

            return outRequests;
        }

        public IQueryable<OutRequest> GetEdit(Guid id)
        {

            var outRequests = _outRequestRepository.GetAll().Where(a => a.IsDeleted == false && a.Id == id).Include(a => a.Details).Include(a => a.Cellar).Include(a => a.Project).OrderBy(a => a.RequestNumber);

            return outRequests;
        }

        public IQueryable<Detail> GetEditDetails(Guid id)
        {
            var details = _detailRepository.GetAll().Where(a => a.IsDeleted == false && a.OutRequestId == id).Include(a => a.Asset).OrderBy(a => a.NameAsset);
            return details;
        }


        public void Delete(OutRequest outRequest, IList<Stock> stocks, IList<Detail> details)
        {
            _outRequestRepository.Update(outRequest);
            foreach (var detail in details)
            {
                detail.OutRequestId = outRequest.Id;
                _detailRepository.Update(detail);
            }
            foreach (var stock in stocks)
            {
                _stockRepository.Update(stock);
            }
        }

        public Asset GetAsset(Guid id)
        {
            return _assetRepository.Get(id);
        }

        public Asset GetAssetBarcode(string code, string companyName)
        {
            return _assetRepository.GetAll().Where(a => a.Barcode.Equals(code) && a.CompanyName.Equals(companyName)).FirstOrDefault();
        }

        public Stock GetStockAsset(Guid id, string companyName, Guid cellarId)
        {
            return _stockRepository.GetAll().Where(a => a.AssetId == id && a.CompanyName.Equals(companyName) && a.CellarId == cellarId).FirstOrDefault();
        }

        public void UpdateDetails(IList<Detail> details)
        {
            foreach (var item in details)
            {
                _detailRepository.Update(item);
            }
        }
    }
}
