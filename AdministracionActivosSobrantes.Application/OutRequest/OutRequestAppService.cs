using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Assets.Dto;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.OutRequest.Dto;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Contractors;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.HistoryChanges;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Projects.Dto;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;
using AdministracionActivosSobrantes.Users;
using AutoMapper;
using GCMvcMailer;
using MvcPaging;

namespace AdministracionActivosSobrantes.OutRequest
{
    class OutRequestAppService : ApplicationService, IOutRequestAppService
    {
        private readonly IRepository<OutRequest, Guid> _outRequestRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<Asset, Guid> _assetRepository;
        private readonly IOutRequestManager _outRequestManager;
        //private readonly IGcErpConnectionService _erpConnection;
        private readonly IDateTime _dateTime;
        private IMailNotificationService _mailSenderService;
        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public OutRequestAppService(IRepository<OutRequest, Guid> outRequestRepository, IOutRequestManager outRequestManager,
            IDateTime dateTime, IMailNotificationService mailSenderService,
            IRepository<Project, Guid> projectRepository, IRepository<Asset, Guid> assetRepository)
        {
            _outRequestRepository = outRequestRepository;
            _outRequestManager = outRequestManager;
            _dateTime = dateTime;
            _mailSenderService = mailSenderService;
            _projectRepository = projectRepository;
            _assetRepository = assetRepository;
        }

        public IPagedList<OutRequestDto> SearchOutRequest(SearchOutRequestInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;
            var @entities = _outRequestManager.SearchRequests(searchInput.Query, searchInput.CellarId, searchInput.ProjectId, searchInput.UserId.Value, searchInput.Status, searchInput.CompanyName, searchInput.DateSearch,searchInput.DateSearch2).ToList();
            return @entities.MapTo<List<OutRequestDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public ProjectDto GetProject(Guid id)
        {
            var @entity = _projectRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }
            return Mapper.Map<ProjectDto>(@entity);
        }

        public Stock GetStockAsset(Guid id, string companyName, Guid cellarId)
        {
            var stock = _outRequestManager.GetStockAsset(id, companyName, cellarId);
            return stock;
        }


        public IPagedList<ProjectDto> SearchProject(SearchProjectsInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;

            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();


            var @entities = _projectRepository.GetAll();
            int totalCount = @entities.Count();

            @entities = @entities.Where(c => c.IsDeleted != null && c.IsDeleted.Value == false && c.CompanyName.Equals(searchInput.CompanyName)
           && (c.Name.ToLower().Contains(searchInput.Query) || c.Name.ToLower().Equals(searchInput.Query) ||
           c.Code.ToLower().Contains(searchInput.Query) || c.Code.ToLower().Equals(searchInput.Query)
           || c.Description.Contains(searchInput.Query)));

            return @entities.OrderByDescending(p => p.Code).Skip(currentPageIndex * searchInput.MaxResultCount).Take(searchInput.MaxResultCount).MapTo<List<ProjectDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount, totalCount);
        }

        public IPagedList<AssetDto> SearchAssets(SearchAssetsInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;

            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();


            var @entities = _assetRepository.GetAll();
            int totalCount = @entities.Count();

            @entities = @entities.Where(c => c.IsDeleted != null && c.IsDeleted.Value == false && c.CompanyName.Equals(searchInput.CompanyName) &&
            ((c.Name.ToLower().Contains(searchInput.Query) || c.Code.ToLower().Contains(searchInput.Query)) ||
                c.Brand.ToLower().Contains(searchInput.Query) || c.Brand.ToLower().Equals(searchInput.Query) ||
                c.Category.Name.ToLower().Contains(searchInput.Query) || c.Category.Name.ToLower().Equals(searchInput.Query) ||
                c.ModelStr.ToLower().Contains(searchInput.Query) || c.ModelStr.ToLower().Equals(searchInput.Query) ||
                c.Plate.ToLower().Contains(searchInput.Query) || c.Plate.ToLower().Equals(searchInput.Query) ||
                c.Series.ToLower().Contains(searchInput.Query) || c.Series.ToLower().Equals(searchInput.Query)));

            return @entities.OrderByDescending(p => p.Stocks).Skip(currentPageIndex * searchInput.MaxResultCount).Take(searchInput.MaxResultCount).MapTo<List<AssetDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount, totalCount);
        }

        public DetailOutRequestOutput Get(Guid id)
        {
            var @entity = _outRequestRepository.Get(id);
            @entity.Details = _outRequestManager.GetEditDetails(id).ToList();
            @entity.Details = @entity.Details.Where(a => a.IsDeleted == false).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            return Mapper.Map<DetailOutRequestOutput>(@entity);
        }

        public CreateOutRequestInput GetEdit(Guid id)
        {
            var @entity = _outRequestManager.GetEdit(id).FirstOrDefault();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Activo, fue borrado o no existe.");
            }
            return Mapper.Map<CreateOutRequestInput>(@entity);
        }

        public User GetUser(Guid userId)
        {
            var @entity = _outRequestManager.GetUser(userId);
            return @entity;
        }
        public DetailOutRequestOutput GetDetail(Guid id)
        {
            var @entity = _outRequestRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            return Mapper.Map<DetailOutRequestOutput>(@entity);
        }

        public IList<Project> GetAllProjects(string company)
        {
            return _outRequestManager.GetProjectsList(company);
        }

        public IList<Cellar> GetAllCellars(string company)
        {
            return _outRequestManager.GetCellarsList(company);
        }
        public IList<Cellar> GetSomeAllCellars(string company)
        {
            return _outRequestManager.GetSomeAllCellarsList(company);
        }

        public IList<ResponsiblePerson.ResponsiblePerson> GetAllResponsibles(string company)
        {
            return _outRequestManager.GetResponsiblesList(company);
        }

        public IList<User> GetAllUsers(string company)
        {
            return _outRequestManager.GetUsersList(company);
        }
        public IList<DetailAssetOutRequestDto> GetEditDetails(Guid outRequestId, Guid cellarId, string company,int impresoSI)
        {
            //var stocksList = _outRequestManager.GetStocksList(company);
            //var stockListCellar = stocksList.Where(a => a.CellarId == cellarId && a.CompanyName.Equals(company));
            var outRequest = _outRequestRepository.Get(outRequestId);
            IList<DetailAssetOutRequestDto> detail = new List<DetailAssetOutRequestDto>();
            var @entity = _outRequestManager.GetEditDetails(outRequestId).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            int index = 0;
            foreach (var item in @entity)
            {
                var stockListCellar = _outRequestManager.GetStocksList(company, outRequest.CellarId.Value, item.AssetId);
                DetailAssetOutRequestDto detailItem = new DetailAssetOutRequestDto();
                detailItem.Asset = item.Asset;
                detailItem.NameAsset = item.NameAsset;
                detailItem.OutRequestId = item.OutRequestId;
                detailItem.StockAsset = item.StockAsset;
                detailItem.AssetId = item.AssetId;
                detailItem.AssetCode = item.Asset.Code;
                detailItem.Price = item.Price.ToString();
                detailItem.Index = index;
                detailItem.Update = 1;
                detailItem.Saved = 1;
                detailItem.PreviousQty = item.StockAsset;
                detailItem.Delete = 0;
                detailItem.ErrorCode = 0;
                detailItem.ErrorDescription = "";
                detailItem.HistroryStatus = HistoryStatus.WithoutChanges;
                detailItem.Status = item.Status;
                detailItem.AssetAvailability = stockListCellar.Where(a => a.AssetId == item.AssetId && a.CompanyName == item.CompanyName).FirstOrDefault().GetStockItemsQty();
                if (impresoSI == 1 && item.Status == Status.Delivered)
                {   
                    detailItem.Impress = Details.Impress.Impreso;
                }
                else
                {
                    detailItem.Impress = Details.Impress.Noimpreso;
                }
                detail.Add(detailItem);
                index++;
            }
            return detail;
        }

        public IList<DetailAssetCloseRequest> GetCloseRequestDetails(Guid outRequestId, Guid cellarId, string company)
        {
            //var stocksList = _outRequestManager.GetStocksList(company);
            //var stockListCellar = stocksList.Where(a => a.CellarId == cellarId && a.CompanyName.Equals(company));
            IList<DetailAssetCloseRequest> detail = new List<DetailAssetCloseRequest>();
            var @entity = _outRequestManager.GetEditDetails(outRequestId).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            int index = 0;
            foreach (var item in @entity)
            {
                DetailAssetCloseRequest detailItem = new DetailAssetCloseRequest();
                detailItem.Asset = item.Asset;
                detailItem.NameAsset = item.NameAsset;
                detailItem.OutRequestId = item.OutRequestId;
                detailItem.StockAsset = item.StockAsset;
                detailItem.AssetId = item.AssetId;
                detailItem.Price = item.Price;
                detailItem.Index = index;
                detailItem.Status = item.Status;
                if (item.AssetReturnQty != null)
                    detailItem.ReturnAssetQty = item.AssetReturnQty.Value;
                else
                    detailItem.ReturnAssetQty = item.StockAsset;
                detailItem.LeftOver = 0;
                detailItem.AssetReturnPartialQty = item.AssetReturnPartialQty;
                detail.Add(detailItem);
                index++;
            }
            return detail;
        }

        public void Update(CreateOutRequestInput input)
        {
            var @entity = _outRequestRepository.Get(input.Id);
            var @entityDetail = _outRequestManager.GetEditDetails(input.Id).ToList();

            //var stocksList = _outRequestManager.GetStocksList(input.CompanyName);
            //var stockListCellar = stocksList.Where(a => a.CellarId == input.CellarId);

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            //if (_outRequestManager.OutRequestExist(@entity.RequestDocumentNumber, input.Id, input.CompanyName))
            //{
            //    throw new UserFriendlyException("Existe una Ubicación con el mismo Nombre.");
            //}
            IList<Detail> newDetails = new List<Detail>();
            IList<Detail> updateDetails = new List<Detail>();
            IList<Detail> deleteDetails = new List<Detail>();

            IList<Stock> updateStock = new List<Stock>();
            IList<Stock> removeStock = new List<Stock>();

            //@entity.RequestDocumentNumber = input.RequestDocumentNumber;
            @entity.AssetsReturnDate = input.AssetsReturnDate;
            @entity.TypeOutRequest = (TypeOutRequest)input.TypeOutRequestValue;
            @entity.Status = (OutRequestStatus)input.StateRequest;
            @entity.ProjectId = input.ProjectId;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.CreatorUserId;
            @entity.ResponsiblePersonId = input.ResponsiblePersonId;
            @entity.Notes = input.Notes;
            @entity.Comment = input.Comment;
            @entity.DepreciableCost = input.DepreciableCost;
            @entity.DeliveredTo = input.DeliveredTo;

            foreach (var item in input.DetailsRequest)
            {
                if (@entityDetail.Exists(a => a.AssetId == item.AssetId))
                {
                    var @updateEntity = @entityDetail.Where(a => a.AssetId == item.AssetId).FirstOrDefault();
                    if (item.Delete == 1)
                    {
                        if (item.Saved == 1)
                        {
                            @updateEntity.IsDeleted = true;
                            deleteDetails.Add(@updateEntity);
                            var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, input.CellarId, item.AssetId);
                            var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                            @stockUpdate.AssetId = item.AssetId;
                            @stockUpdate.CellarId = input.CellarId;
                            @stockUpdate.RemoveFromBlockedStock(item.StockAsset, Double.Parse(item.Price));
                            removeStock.Add(@stockUpdate);
                        }
                    }
                    else if (item.Update == 1)
                    {
                        double stockP = @updateEntity.StockAsset;
                        @updateEntity.Price = Double.Parse(item.Price);
                        @updateEntity.NameAsset = item.NameAsset;
                        @updateEntity.StockAsset = item.StockAsset;
                        @updateEntity.LastModificationTime = _dateTime.Now;
                        @updateEntity.LastModifierUserId = input.CreatorUserId;
                        updateDetails.Add(@updateEntity);
                        var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, input.CellarId, item.AssetId);
                        var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                        @stockUpdate.AssetId = item.AssetId;
                        @stockUpdate.CellarId = input.CellarId;
                        if (stockP > item.StockAsset)
                        {
                            double resta = stockP - item.StockAsset;
                            @stockUpdate.RemoveFromBlockedStock(resta, Double.Parse(item.Price));
                        }
                        else if (stockP < item.StockAsset)
                        {
                            double sum = item.StockAsset - stockP;
                            @stockUpdate.AddToBlockedQty(sum, Double.Parse(item.Price));
                        }
                        updateStock.Add(@stockUpdate);
                    }
                }
                else
                {
                    var @createDetail = Detail.Create(null, input.Id, null, item.AssetId, item.NameAsset, item.StockAsset, Double.Parse(item.Price),
                    input.CreatorUserId, _dateTime.Now, input.CompanyName);
                    newDetails.Add(@createDetail);
                    var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, input.CellarId, item.AssetId);
                    var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                    @stockUpdate.AssetId = item.AssetId;
                    @stockUpdate.CellarId = input.CellarId;
                    @stockUpdate.AddToBlockedQty(item.StockAsset, Double.Parse(item.Price));
                    updateStock.Add(@stockUpdate);
                }
            }

            if (HasFile(input.Image1))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image1.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath1 = folderPathRelative + fileName;
                input.Image1.SaveAs(mappedPath);

                @entity.SetImage1(input.ImagePath1);
            }

            if (HasFile(input.Image2))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image2.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath2 = folderPathRelative + fileName;
                input.Image2.SaveAs(mappedPath);

                @entity.SetImage2(input.ImagePath2);
            }
            if (HasFile(input.Image3))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image3.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath3 = folderPathRelative + fileName;
                input.Image3.SaveAs(mappedPath);

                @entity.SetImage3(input.ImagePath3);
            }
            if (HasFile(input.Image4))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image4.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath4 = folderPathRelative + fileName;
                input.Image4.SaveAs(mappedPath);

                @entity.SetImage4(input.ImagePath4);
            }
            if (!string.IsNullOrEmpty(input.SignatureData))
            {
                @entity.SetSignatureData(input.SignatureData);
            }

            _outRequestManager.Update(@entity, updateStock, removeStock, newDetails, deleteDetails, updateDetails);
        }

        public void DeliverRequest(CreateOutRequestInput input, IList<StockMap> list, string url)
        {
            var @entity = _outRequestRepository.Get(input.Id);
            @entity.WareHouseManId = input.CreatorUserId;
            @entity.DeliverDate = _dateTime.Now;
            @entity.Comment = input.Comment;

            var @entityDetails = _outRequestManager.GetEditDetails(@entity.Id).ToList();

            IList<Movement> @movements = new List<Movement>();
            IList<Stock> @removeBlockedStocks = new List<Stock>();
            IList<Stock> @updateStocks = new List<Stock>();
            IList<Detail> @updateDetails = new List<Detail>();


            int countStatus = 0;

            foreach (var item in input.DetailsRequest)
            {
                if (item.Status == Status.Delivered)
                {
                    var detail = @entityDetails.FirstOrDefault(a => a.AssetId == item.AssetId);
                    detail.Status = Status.Delivered;
                    var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, entity.CellarId.Value, item.AssetId);
                    var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                    @stockUpdate.AssetId = item.AssetId;
                    @stockUpdate.CellarId = @entity.CellarId.Value;
                    @stockUpdate.RemoveFromBlockedStock(item.StockAsset, Double.Parse(item.Price));
                    @removeBlockedStocks.Add(@stockUpdate);
                    @updateDetails.Add(detail);
                    countStatus++;
                }
            }           

            if (countStatus == input.DetailsRequest.Count)
                @entity.Status = OutRequestStatus.ProcessedInWareHouse;
            else
                @entity.Status = OutRequestStatus.Backorder;
        
            @movements = Movements(@updateDetails, list, @entity.CellarId.Value, input.CreatorUserId, @entity.Id, @entity.ProjectId, TypeMovement.Output, input.CompanyName);

            foreach (var item in @updateDetails)
            {
                var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, entity.CellarId.Value, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @stockUpdate.RemoveFromStock(item.StockAsset, item.Price);
                @updateStocks.Add(@stockUpdate);
            }

            if (HasFile(input.Image1))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image1.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath1 = folderPathRelative + fileName;
                input.Image1.SaveAs(mappedPath);

                @entity.SetImage1(input.ImagePath1);
            }

            if (HasFile(input.Image2))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image2.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath2 = folderPathRelative + fileName;
                input.Image2.SaveAs(mappedPath);

                @entity.SetImage2(input.ImagePath2);
            }
            if (HasFile(input.Image3))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image3.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath3 = folderPathRelative + fileName;
                input.Image3.SaveAs(mappedPath);

                @entity.SetImage3(input.ImagePath3);
            }
            if (HasFile(input.Image4))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image4.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath4 = folderPathRelative + fileName;
                input.Image4.SaveAs(mappedPath);

                @entity.SetImage4(input.ImagePath4);
            }
            if (!string.IsNullOrEmpty(input.SignatureData))
            {
                @entity.SetSignatureData(input.SignatureData);
            }

            _outRequestManager.ChangeStatus(@entity, @updateStocks, @removeBlockedStocks, @updateDetails, @movements);

            SendOutRequestMailNotification(url, entity, input.CompanyName);
        }


        public void UpdateEditDetailChangeStatus(CreateOutRequestInput input)
        {
            var @entity = _outRequestRepository.Get(input.Id);
            var @entityDetail = _outRequestManager.GetEditDetails(input.Id).ToList();


            //var stocksList = _outRequestManager.GetStocksList(input.CompanyName);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId);

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            if (_outRequestManager.OutRequestExist(@entity.RequestDocumentNumber, input.Id, input.CompanyName))
            {
                throw new UserFriendlyException("Existe una Ubicación con el mismo Nombre.");
            }
            IList<Detail> newDetails = new List<Detail>();
            IList<Detail> updateDetails = new List<Detail>();
            IList<Detail> deleteDetails = new List<Detail>();

            IList<HistoryChange> historyChanges = new List<HistoryChange>();

            IList<Stock> updateStock = new List<Stock>();
            IList<Stock> removeStock = new List<Stock>();

            @entity.Status = (OutRequestStatus)input.StateRequest;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.CreatorUserId;

            foreach (var item in input.DetailsRequest)
            {
                if (@entityDetail.Exists(a => a.AssetId == item.AssetId))
                {

                    var @updateEntity = @entityDetail.Where(a => a.AssetId == item.AssetId).FirstOrDefault();
                    if (item.Delete == 1)
                    {
                        if (item.Saved == 1)
                        {
                            @updateEntity.IsDeleted = true;
                            @updateEntity.Status = Status.Rejected;
                            deleteDetails.Add(@updateEntity);
                            var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, @entity.CellarId.Value, item.AssetId);
                            var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                            @stockUpdate.AssetId = item.AssetId;
                            @stockUpdate.CellarId = @entity.CellarId.Value;
                            @stockUpdate.RemoveFromBlockedStock(item.StockAsset, Double.Parse(item.Price));
                            removeStock.Add(@stockUpdate);
                        }
                    }
                    else if (item.Update == 2)
                    {
                        double stockP = @updateEntity.StockAsset;
                        @updateEntity.Price = Double.Parse(item.Price);
                        @updateEntity.NameAsset = item.NameAsset;
                        @updateEntity.Status = Status.Approved;
                        @updateEntity.StockAsset = item.StockAsset;
                        @updateEntity.LastModificationTime = _dateTime.Now;
                        @updateEntity.LastModifierUserId = input.CreatorUserId;
                        updateDetails.Add(@updateEntity);
                        var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, @entity.CellarId.Value,
                            item.AssetId);
                        var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                        @stockUpdate.AssetId = item.AssetId;
                        @stockUpdate.CellarId = @entity.CellarId.Value;
                        if (stockP > item.StockAsset)
                        {
                            double resta = stockP - item.StockAsset;
                            @stockUpdate.RemoveFromBlockedStock(resta, Double.Parse(item.Price));
                        }
                        else if (stockP < item.StockAsset)
                        {
                            double sum = item.StockAsset - stockP;
                            @stockUpdate.AddToBlockedQty(sum, Double.Parse(item.Price));
                        }
                        updateStock.Add(@stockUpdate);
                    }
                    else
                    {
                        @updateEntity.Status = Status.Approved;
                    }
                    if (item.PreviousQty == null)
                        item.PreviousQty = 0;

                    var @createHistoryChange = HistoryChange.Create(null, @entity.Id, null, item.AssetId, item.NameAsset, item.StockAsset, Double.Parse(item.Price),
                   input.CreatorUserId, _dateTime.Now, input.CompanyName, item.PreviousQty.Value, item.HistroryStatus);
                    historyChanges.Add(@createHistoryChange);
                }

            }
            _outRequestManager.AddHistoryChanges(@entity.Id, historyChanges);
            _outRequestManager.Update(@entity, updateStock, removeStock, newDetails, deleteDetails, updateDetails);
            if (@entity.Status == OutRequestStatus.Backorder)
                ProcessedBackOrderStatus(@entity.Id, input.CreatorUserId, input.UrlEmail, input.CompanyName, input.StockMaps);
        }


        public void CloseRequest(CloseOutRequestInput input, IList<StockMap> list)
        {
            var @entity = _outRequestRepository.Get(input.Id);
            var @entityDetail = _outRequestManager.GetEditDetails(input.Id).ToList();
            //var stocksList = _outRequestManager.GetStocksList(input.CompanyName);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId).ToList();

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }

            IList<Stock> @updatestocks = new List<Stock>();
            IList<Movement> @movements = new List<Movement>();
            IList<Detail> @updateDetail = new List<Detail>();

            @entity.Status = OutRequestStatus.Confirmado;
            double leftOver = 0;
            double closeRequestQty = 0;
            foreach (var item in input.DetailsRequest)
            {
                if (@entityDetail.Exists(a => a.AssetId == item.AssetId))
                {
                    var @updateEntity = @entityDetail.Where(a => a.AssetId == item.AssetId).FirstOrDefault();

                    if (@updateEntity.AssetReturnPartialQty != null)
                        leftOver = @updateEntity.StockAsset - @updateEntity.AssetReturnPartialQty.Value;
                    else
                        leftOver = @updateEntity.StockAsset;

                    if (leftOver != item.ReturnAssetQty && @entity.TypeOutRequest == TypeOutRequest.Asset)
                        throw new UserFriendlyException("Faltan algunos activos por ingresar, por favor verifique los datos.");

                    @updateEntity.AssetReturnPartialQty = item.StockAsset;
                    @updateEntity.AssetReturnQty = item.ReturnAssetQty;
                    @updateEntity.LastModifierUserId = input.CreatorUserId;
                    @updateEntity.LastModificationTime = _dateTime.Now;
                    @updateEntity.Status = Status.Closed;
                    @updateDetail.Add(@updateEntity);
                }
            }

            @movements = Movements(@updateDetail, list, @entity.CellarId.Value, input.CreatorUserId, @entity.Id, @entity.ProjectId, TypeMovement.Input, input.CompanyName);

            foreach (var item in input.DetailsRequest)
            {
                if (@entityDetail.Exists(a => a.AssetId == item.AssetId))
                {
                    var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, input.CellarId, item.AssetId);
                    var @stockUpdate = stockListCellar.Where(a => a.AssetId == item.AssetId).FirstOrDefault();
                    if (@stockUpdate != null)
                    {
                        @stockUpdate.AssetId = item.AssetId;
                        @stockUpdate.CellarId = @entity.CellarId.Value;
                        @stockUpdate.AddToStock(item.StockAsset, item.Price);
                        @updatestocks.Add(@stockUpdate);
                    }
                }
            }
            _outRequestManager.CloseRequest(@entity, @updatestocks, @movements, @updateDetail);

          //  SendOutRequestMailNotification(input.UrlAction, entity, input.CompanyName);
        }

        public void Create(CreateOutRequestInput input)
        {
            //if (_outRequestManager.OutRequestExist(input.RequestDocumentNumber, input.Id, input.CompanyName))
            //{
            //    throw new UserFriendlyException("Existe una solicitud con el mismo numero de Solicitud Fisica.");
            //}
            TypeOutRequest temp = (TypeOutRequest)input.TypeOutRequestValue;
            OutRequestStatus temp1;
            temp1 = OutRequestStatus.Active;

            var requestNumber = GetNextRequestNumber(input.CompanyName);
           // var departamento = 
            //var stocksList = _outRequestManager.GetStocksList(input.CompanyName);

            //var stockListCellar = stocksList.Where(a => a.CellarId == input.CellarId).ToList();

            var @entityOutRequest = OutRequest.Create(requestNumber, requestNumber.ToString(), "Descripción", input.Notes, input.ProjectId,
                input.CellarId, input.AssetsReturnDate, temp1, temp, input.ResponsiblePersonId.Value, input.DeliveredTo, input.ContractorId, input.CreatorGuidId.Value, _dateTime.Now, input.CompanyName, input.Comment,input.Department,input.DepreciableCost);

            IList<Detail> @details = new List<Detail>();
            IList<Stock> @stocks = new List<Stock>();

            foreach (var item in input.DetailsRequest)
            {
                var @entityDetail = Detail.Create(null, @entityOutRequest.Id, null, item.AssetId, item.NameAsset, item.StockAsset, Double.Parse(item.Price),
                    input.CreatorGuidId.Value, _dateTime.Now, input.CompanyName);
                @details.Add(@entityDetail);
            }

            foreach (var item in input.DetailsRequest)
            {
                //var assets = _assetRepository
                var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, input.CellarId, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = input.CellarId;

               /* @stockUpdate.UltOutRequest = item.OutRequestId;
                @stockUpdate.DateUltOutRequest = DateTime.Now;*/

                @stockUpdate.AddToBlockedQty(item.StockAsset, Double.Parse(item.Price));

                @stocks.Add(@stockUpdate);

            }

            if (HasFile(input.Image1))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image1.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../OutRequestImages/";
                input.ImagePath1 = folderPathRelative + fileName;
                input.Image1.SaveAs(mappedPath);

                @entityOutRequest.SetImage1(input.ImagePath1);
            }

            if (HasFile(input.Image2))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image2.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../InRequestImages/";
                input.ImagePath2 = folderPathRelative + fileName;
                input.Image2.SaveAs(mappedPath);

                @entityOutRequest.SetImage2(input.ImagePath2);
            }
            if (HasFile(input.Image3))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image3.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../InRequestImages/";
                input.ImagePath3 = folderPathRelative + fileName;
                input.Image3.SaveAs(mappedPath);

                @entityOutRequest.SetImage3(input.ImagePath3);
            }
            if (HasFile(input.Image4))
            {
                string folderPath = "~/OutRequestImages/";
                string fileName = input.Image4.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../InRequestImages/";
                input.ImagePath4 = folderPathRelative + fileName;
                input.Image4.SaveAs(mappedPath);

                @entityOutRequest.SetImage4(input.ImagePath4);
            }
            _outRequestManager.Create(@entityOutRequest, @stocks, @details);
        }

        private static bool HasFile(HttpPostedFile file)
        {
            return file != null && file.ContentLength > 0;
        }

        private IList<Movement> Movements(IList<Detail> details, IList<StockMap> stocksGeneral, Guid cellarId, Guid userId, Guid outRequestId, Guid? projectId, TypeMovement type, string companyName)
        {
            //Movimiento
            IList<Movement> movements = new List<Movement>();

            int lastmov = _outRequestManager.GetNextMovementNumber(companyName);
            double montoTotalInventario = stocksGeneral.Sum(a => a.GetAmountInStock());
            double montoTotalBodega = stocksGeneral.Where(a => a.CellarId == cellarId).Sum(a => a.GetAmountInStock());
            foreach (var item in details)
            {
                var stockListCellar = _outRequestManager.GetStocksList(companyName, cellarId, item.AssetId);
                var articuloExistenciaInventarioArticulo = stockListCellar.Where(a => a.AssetId == item.AssetId);
                var articuloExistenciaBodega = articuloExistenciaInventarioArticulo.FirstOrDefault(a => a.CellarId == cellarId);

                double cntAnteriorExistenciaBodegaMigrado = articuloExistenciaBodega.GetStockItemsQty();
                double cntAntExisteciasInvMigrado = articuloExistenciaInventarioArticulo.Sum(a => a.GetStockItemsQty());

                // Montos antes del movimiento en todo el inventario
                double montoAnteriorExistenciaBodega = articuloExistenciaBodega.GetAmountInStock();
                double montoAntExistenciasInv = articuloExistenciaInventarioArticulo.Sum(a => a.GetAmountInStock());

                // Monto en todo el inventario y bodega
                double previousGeneralInvAmount = montoTotalInventario;
                double previousGeneralStockAmount = montoTotalBodega;

                if (type == TypeMovement.Output)
                {
                    var @entity = Movement.Create(lastmov, item.StockAsset, item.Price, _dateTime.Now,
                      StatusMovement.AplicadoInventario, type, MovementCategory.OutRequest, cntAnteriorExistenciaBodegaMigrado, montoAnteriorExistenciaBodega, cntAntExisteciasInvMigrado, montoAntExistenciasInv,
                      previousGeneralInvAmount, previousGeneralStockAmount, cellarId, item.AssetId, userId, null, outRequestId, null, projectId, userId, _dateTime.Now, companyName);
                    movements.Add(@entity);
                    montoTotalInventario -= @entity.GetAmountMovement(); //Actualiza monto inventario
                    montoTotalBodega -= @entity.GetAmountMovement(); //Actualiza monto bodega    
                }
                else if (type == TypeMovement.Input)
                {
                    var @entity = Movement.Create(lastmov, item.AssetReturnQty.Value, item.Price, _dateTime.Now,
                   StatusMovement.AplicadoInventario, type, MovementCategory.OutRequest, cntAnteriorExistenciaBodegaMigrado, montoAnteriorExistenciaBodega, cntAntExisteciasInvMigrado, montoAntExistenciasInv,
                   previousGeneralInvAmount, previousGeneralStockAmount, cellarId, item.AssetId, userId, null, outRequestId, null, projectId, userId, _dateTime.Now, companyName);
                    movements.Add(@entity);
                    montoTotalInventario += @entity.GetAmountMovement(); //Actualiza monto inventario
                    montoTotalBodega += @entity.GetAmountMovement(); //Actualiza monto bodega    
                }
                lastmov++;
            }
            return movements;
        }

        public void Delete(Guid id, Guid userId, string company)
        {
            var @entity = _outRequestRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.IsDeleted = true;
            @entity.DeleterUserId = userId;
            @entity.DeletionTime = _dateTime.Now;

            //var stocksList = _outRequestManager.GetStocksList(company);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId);
            IList<Detail> detailRemove = new List<Detail>();
            IList<Stock> stockRemove = new List<Stock>();
            var @entityDetail = _outRequestManager.GetEditDetails(id).ToList();
            foreach (var item in @entityDetail)
            {
                item.DeleterUserId = userId;
                item.IsDeleted = true;
                item.DeletionTime = _dateTime.Now;
                detailRemove.Add(item);
                var stockListCellar = _outRequestManager.GetStocksList(company, entity.CellarId.Value, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @stockUpdate.IsDeleted = true;
                @stockUpdate.DeletionTime = _dateTime.Now;
                @stockUpdate.DeleterUserId = userId;
                @stockUpdate.RemoveFromBlockedStock(item.StockAsset, item.Price);
                stockRemove.Add(@stockUpdate);

            }
            _outRequestManager.Delete(@entity, stockRemove, detailRemove);
        }
        public IList<User> GetAllWareHouseUsers()
        {
            throw new NotImplementedException();
        }

        public IPagedList<Stock> GetAllStocks(string query, Guid? cellarId, Guid? assetId, TypeOutRequest? typeOutRequest, int defaultPageSize, int? page, string company)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var stocks = _outRequestManager.SearchAsset(query, cellarId, assetId, typeOutRequest.Value, company);
            int totalCount = stocks.Count();
            return stocks.Skip(currentPageIndex * defaultPageSize).Take(defaultPageSize).MapTo<List<Stock>>().ToPagedList(currentPageIndex, defaultPageSize, totalCount);
        }

        public int GetNextRequestNumber(string company)
        {
            return _outRequestManager.GetNextRequestNumber(company);
        }

        private void SendOutRequestMailNotification(string urlActionRequest, OutRequest entity, string company)
        {
            IList<User> listUsers = _outRequestManager.GetUsersList(company);
            var requestUser = listUsers.FirstOrDefault(a => a.Id == @entity.CreatorUserId);
            var wareHouseManagertUser = listUsers.FirstOrDefault(a => a.Id == @entity.Cellar.WareHouseManagerId);
            var approvalUser = listUsers.FirstOrDefault(a => a.Id == @entity.ApprovalUserId);
            var coordinatorUsers = _outRequestManager.GetUsers(e => e.IsDeleted == false && e.Rol == UserRoles.Coordinador);
            var contratorUser = listUsers.FirstOrDefault(a => a.Id == @entity.ContractorId);
            string approvalUserName = String.Empty, approvalUserEmail = String.Empty;
            string wareHouseManagerName = String.Empty, wareHouseManagerEmail = String.Empty;
            string requestUserName = String.Empty, requestUserEmail = String.Empty;
            string contractorName = String.Empty, contractorEmail = String.Empty;

            if (requestUser != null)
            {
                requestUserName = requestUser.CompleteName;
                requestUserEmail = requestUser.Email;
            }
            if (wareHouseManagertUser != null)
            {
                wareHouseManagerName = wareHouseManagertUser.CompleteName;
                wareHouseManagerEmail = wareHouseManagertUser.Email;
            }
            if (approvalUser != null)
            {
                approvalUserName = approvalUser.CompleteName;
                approvalUserEmail = approvalUser.Email;
            }

            if (contratorUser != null)
            {
                contractorName = contratorUser.CompleteName;
                contractorEmail = contratorUser.Email;
            }
            if (requestUserEmail != null)
            {
                _mailSenderService.OutRequestAprovalEmail(@entity.RequestNumber.ToString(), @entity.Cellar.Name, coordinatorUsers,
                    wareHouseManagerName, wareHouseManagerEmail, requestUserName,
                    approvalUserName, requestUserEmail, approvalUserEmail, contractorEmail,
                    urlActionRequest, @entity.Status, @entity.TypeOutRequest, true);
            }


        }

        public void WaitApprovalStatus(Guid outRequestId, Guid userId, string urlActionRequest, string company)
        {
            var @entity = _outRequestRepository.GetAll().Where(a => a.IsDeleted == false && a.Id == outRequestId).Include(a => a.ApprovalUser).Include(a => a.WareHouseMan).Include(a => a.Contractor).FirstOrDefault();

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.Status = OutRequestStatus.WaitApproval;
            @entity.LastModifierUserId = userId;
            @entity.LastModificationTime = _dateTime.Now;
            _outRequestRepository.Update(@entity);

            SendOutRequestMailNotification(urlActionRequest, entity, company);
        }

        public void ApprovedStatus(Guid outRequestId, Guid userId, string urlActionRequest, string company)
        {
            var @entity = _outRequestRepository.Get(outRequestId);
            var @entityDetail = _outRequestManager.GetEditDetails(outRequestId).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.Status = OutRequestStatus.Approved;
            @entity.ApprovalUserId = userId;
            @entity.AprovedDate = _dateTime.Now;

            foreach (var item in @entityDetail)
            {
                item.Status = Status.Approved;
            }

            _outRequestManager.UpdateDetails(@entityDetail);
            _outRequestRepository.Update(@entity);

            SendOutRequestMailNotification(urlActionRequest, entity, company);
        }

        public void ProcessedInWareHouseStatus(Guid outRequestId, Guid userId, string urlActionRequest, string companyName, IList<StockMap> stockList)
        {
            var @entity = _outRequestRepository.Get(outRequestId);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.Status = OutRequestStatus.ProcessedInWareHouse;
            @entity.WareHouseManId = userId;
            @entity.DeliverDate = _dateTime.Now;

            var @entityDetails = _outRequestManager.GetEditDetails(outRequestId).ToList();

            //var stocksList = _outRequestManager.GetStocksList(companyName);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId).ToList();

            IList<Movement> @movements = new List<Movement>();
            IList<Stock> @removeBlockedStocks = new List<Stock>();
            IList<Stock> @updateStocks = new List<Stock>();


            foreach (var item in @entityDetails)
            {
                var stockListCellar = _outRequestManager.GetStocksList(companyName, entity.CellarId.Value, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @stockUpdate.RemoveFromBlockedStock(item.StockAsset, item.Price);
                @removeBlockedStocks.Add(@stockUpdate);
            }

            @movements = Movements(@entityDetails, stockList, @entity.CellarId.Value, userId, @entity.Id, @entity.ProjectId, TypeMovement.Output, companyName);

            foreach (var item in @entityDetails)
            {
                var stockListCellar = _outRequestManager.GetStocksList(companyName, entity.CellarId.Value, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @stockUpdate.RemoveFromStock(item.StockAsset, item.Price);
                @updateStocks.Add(@stockUpdate);
            }

            _outRequestManager.ChangeStatus(@entity, @updateStocks, @removeBlockedStocks, @entityDetails, @movements);

            SendOutRequestMailNotification(urlActionRequest, entity, companyName);
        }

        public void ProcessedBackOrderStatus(Guid outRequestId, Guid userId, string urlActionRequest, string companyName, IList<StockMap> stockList)
        {
            var @entity = _outRequestRepository.Get(outRequestId);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.Status = OutRequestStatus.Backorder;
            @entity.WareHouseManId = userId;
            @entity.DeliverDate = _dateTime.Now;

            var @entityDetails = _outRequestManager.GetEditDetails(outRequestId).ToList();

            //var stocksList = _outRequestManager.GetStocksList(companyName);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId).ToList();

            IList<Movement> @movements = new List<Movement>();
            IList<Stock> @removeBlockedStocks = new List<Stock>();
            IList<Stock> @updateStocks = new List<Stock>();


            foreach (var item in @entityDetails)
            {
                var stockListCellar = _outRequestManager.GetStocksList(companyName, entity.CellarId.Value, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @stockUpdate.RemoveFromBlockedStock(item.StockAsset, item.Price);
                @removeBlockedStocks.Add(@stockUpdate);
            }

            @movements = Movements(@entityDetails, stockList, @entity.CellarId.Value, userId, @entity.Id, @entity.ProjectId, TypeMovement.Output, companyName);

            foreach (var item in @entityDetails)
            {
                var stockListCellar = _outRequestManager.GetStocksList(companyName, entity.CellarId.Value, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @stockUpdate.RemoveFromStock(item.StockAsset, item.Price);
                @updateStocks.Add(@stockUpdate);
            }

            _outRequestManager.ChangeStatus(@entity, @updateStocks, @removeBlockedStocks, @entityDetails, @movements);

            SendOutRequestMailNotification(urlActionRequest, entity, companyName);
        }

        public void DisprovedStatus(Guid outRequestId, Guid userId, string urlActionRequest, string company)
        {
            var @entity = _outRequestRepository.Get(outRequestId);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.Status = OutRequestStatus.Disproved;
            @entity.LastModifierUserId = userId;
            @entity.LastModificationTime = _dateTime.Now;

            var @entityDetails = _outRequestManager.GetEditDetails(outRequestId).ToList();

            //var stocksList = _outRequestManager.GetStocksList(company);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId).ToList();

            IList<Stock> @removeBlockedStocks = new List<Stock>();

            foreach (var item in @entityDetails)
            {
                var stockListCellar = _outRequestManager.GetStocksList(company, @entity.CellarId.Value, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @stockUpdate.RemoveFromBlockedStock(item.StockAsset, item.Price);
                @removeBlockedStocks.Add(@stockUpdate);
            }

            _outRequestManager.ChangeStatus(@entity, new List<Stock>(), @removeBlockedStocks, @entityDetails, new List<Movement>());

            SendOutRequestMailNotification(urlActionRequest, entity, company);
        }

        public IList<Contractor> GetAllContractor(string company)
        {
            return _outRequestManager.GetContractorList(company);
        }

        public Asset GetAsset(Guid id)
        {
            var @asset = _outRequestManager.GetAsset(id);
            return @asset;
        }

        public Asset GetAssetBarCode(string code, string companyName)
        {
            var @asset = _outRequestManager.GetAssetBarcode(code, companyName);
            return @asset;
        }
        public IList<Detail> ActualizaImpresion(Guid outRequestId)
        {
            var outRequest = _outRequestRepository.Get(outRequestId);
            var detalles = _outRequestManager.GetEditDetails(outRequestId).ToList();
            if (detalles == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            foreach (var item in detalles)
            {
                if (item.Status == Status.Delivered)
                {
                    item.Impress = Details.Impress.Impreso;
                }
                
            }
            _outRequestManager.UpdateDetails(detalles);
            return detalles;

        }

        public void ClosePartialRequest(Guid outRequestId, Guid assetId, double qty, Guid userId, string companyName, IList<StockMap> stockList)
        {
            var @entity = _outRequestRepository.Get(outRequestId);
            var @entityDetail = _outRequestManager.GetEditDetails(outRequestId).ToList();

            //var stocksList = _outRequestManager.GetStocksList(companyName);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId).ToList();

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }

            IList<Stock> @updatestocks = new List<Stock>();
            IList<Movement> @movements = new List<Movement>();
            IList<Detail> @updateDetail = new List<Detail>();

            //@entity.Status = OutRequestStatus.confirmado;
            double price = 0;
            double leftOver = 0;
            double stockUptate = 0;
            //bool closedRequest = false;
            if (@entityDetail.Exists(a => a.AssetId == assetId))
            {
                var @updateEntity = @entityDetail.Where(a => a.AssetId == assetId).FirstOrDefault();

                ////Con solo uno falso no cierra la solicitud
                //if (leftOver == 0)
                //    closedRequest = true;
                //else
                //    closedRequest = false;
                price = @updateEntity.Price;
                if (@updateEntity.AssetReturnPartialQty != null)
                {
                    leftOver = @updateEntity.StockAsset - @updateEntity.AssetReturnPartialQty.Value - qty;
                    stockUptate = @updateEntity.AssetReturnPartialQty.Value + qty;
                }
                else
                {
                    stockUptate = qty;
                    leftOver = @updateEntity.StockAsset - qty;
                }
                @updateEntity.AssetReturnQty = leftOver;
                @updateEntity.AssetReturnPartialQty = stockUptate;
                @updateEntity.LastModifierUserId = userId;
                @updateEntity.LastModificationTime = _dateTime.Now;
                @updateDetail.Add(@updateEntity);
            }
            //----------------------------------------------------------
            //Movimiento

            var lastmov = _outRequestManager.GetNextMovementNumber(companyName);
            //var stockListCellar = _outRequestManager.GetStocksList(input.CompanyName, input.CellarId, item.AssetId);
            double montoTotalInventario = stockList.Sum(a => a.GetAmountInStock());
            double montoTotalBodega = stockList.Where(a => a.CellarId == @entity.CellarId).Sum(a => a.GetAmountInStock());

            var stockListCellar = _outRequestManager.GetStocksList(companyName, @entity.CellarId.Value, assetId);
            var articuloExistenciaInventarioArticulo = stockListCellar.Where(a => a.AssetId == assetId);
            var articuloExistenciaBodega = articuloExistenciaInventarioArticulo.FirstOrDefault(a => a.CellarId == @entity.CellarId);

            double cntAnteriorExistenciaBodegaMigrado = articuloExistenciaBodega.GetStockItemsQty();
            double cntAntExisteciasInvMigrado = articuloExistenciaInventarioArticulo.Sum(a => a.GetStockItemsQty());

            // Montos antes del movimiento en todo el inventario
            double montoAnteriorExistenciaBodega = articuloExistenciaBodega.GetAmountInStock();
            double montoAntExistenciasInv = articuloExistenciaInventarioArticulo.Sum(a => a.GetAmountInStock());

            // Monto en todo el inventario y bodega
            double previousGeneralInvAmount = montoTotalInventario;
            double previousGeneralStockAmount = montoTotalBodega;


            var @entityMovement = Movement.Create(lastmov, qty, price, _dateTime.Now,
           StatusMovement.AplicadoInventario, TypeMovement.Input, MovementCategory.OutRequest, cntAnteriorExistenciaBodegaMigrado, montoAnteriorExistenciaBodega, cntAntExisteciasInvMigrado, montoAntExistenciasInv,
           previousGeneralInvAmount, previousGeneralStockAmount, @entity.CellarId.Value, assetId, userId, null, outRequestId, null, @entity.ProjectId, userId, _dateTime.Now, companyName);
            movements.Add(@entityMovement);

            //-----------------------------------------------------------------
            if (@entityDetail.Exists(a => a.AssetId == assetId))
            {
                var @updateEntity = @entityDetail.Where(a => a.AssetId == assetId).FirstOrDefault();

                var @stockUpdate = stockListCellar.Where(a => a.AssetId == assetId).FirstOrDefault();
                if (@stockUpdate != null)
                {
                    @stockUpdate.AssetId = assetId;
                    @stockUpdate.CellarId = @entity.CellarId.Value;
                    @stockUpdate.AddToStock(qty, @updateEntity.Price);
                    @updatestocks.Add(@stockUpdate);
                }
            }
            //    _outRequestManager.CloseRequest(@entity, @updatestocks, @movements, @updateDetail);
        }
    }
}
