using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.InRequest.Dto;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.PriceChanges;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;
using AdministracionActivosSobrantes.Users;
using AutoMapper;
using GCMvcMailer;
using MvcPaging;

namespace AdministracionActivosSobrantes.InRequest
{
    class InRequestAppService : ApplicationService, IInRequestAppService
    {
        private readonly IRepository<InRequest, Guid> _inRequestRepository;
        private readonly IInRequestManager _inRequestManager;
        private readonly IDateTime _dateTime;
        private readonly IMailNotificationService _mailSender;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public InRequestAppService(IRepository<InRequest, Guid> inRequestRepository, IInRequestManager inRequestManager, IDateTime dateTime, IMailNotificationService mailSender)
        {
            _inRequestRepository = inRequestRepository;
            _inRequestManager = inRequestManager;
            _dateTime = dateTime;
            _mailSender = mailSender;
        }

        public IPagedList<InRequestDto> SearchInRequest(SearchInRequestInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;
            var @entities = _inRequestManager.SearchRequests(searchInput.Query, searchInput.CellarId, searchInput.UserId.Value, searchInput.CompanyName).ToList();
            return @entities.MapTo<List<InRequestDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public IPagedList<Asset> SearchAssets(string query, int? page, TypeInRequest type,string company)
        {
            int defaultPageSize = 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var @entities = _inRequestManager.SearchAsset(query, type, company);
            int totalCount = @entities.Count();
            return @entities.OrderByDescending(p => p.Code).Skip(currentPageIndex * defaultPageSize).Take(defaultPageSize).MapTo<List<Asset>>().ToPagedList(currentPageIndex, defaultPageSize,totalCount);
        }

        public int TotalCount(string query, int? page, TypeInRequest type, string company)
        {
            var @entities = _inRequestManager.SearchAsset(query, type, company);
            return @entities.Count();
        }

        public IList<Stock> GetAllStocks(string company)
        {
            var @entities = _inRequestManager.GetStocksList(company);
            return @entities;
        }

        public DetailInRequestOutput Get(Guid id)
        {
            var @entity = _inRequestRepository.Get(id);
            @entity.Details = _inRequestManager.GetEditDetails(id).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            return Mapper.Map<DetailInRequestOutput>(@entity);
        }

        public CreateInRequestInput GetEdit(Guid id)
        {
            var @entity = _inRequestManager.GetEdit(id).FirstOrDefault();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }
            return Mapper.Map<CreateInRequestInput>(@entity);
        }

        public DetailInRequestOutput GetDetail(Guid id)
        {
            var @entity = _inRequestRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            return Mapper.Map<DetailInRequestOutput>(@entity);
        }

        public IList<DetailAssetInRequestDto> GetEditDetails(Guid inRequestId, Guid cellarId)
        {
            IList<DetailAssetInRequestDto> detail = new List<DetailAssetInRequestDto>();
            var @entity = _inRequestManager.GetEditDetails(inRequestId).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            int index = 0;
            foreach (var item in @entity)
            {
                DetailAssetInRequestDto detailItem = new DetailAssetInRequestDto();
                detailItem.Asset = item.Asset;
                detailItem.NameAsset = item.NameAsset;
                detailItem.InRequestId = item.InRequestId;
                detailItem.StockAsset = item.StockAsset;
                detailItem.AssetId = item.AssetId;
                detailItem.AssetCode = item.Asset.Code;
                detailItem.Price = item.Price.ToString();
                detailItem.Index = index;
                detailItem.Update = 1;
                detailItem.Saved = 1;
                detailItem.Delete = 0;
                detailItem.ErrorCode = 0;
                detailItem.ErrorDescription = "";
                detail.Add(detailItem);
                index++;
            }
            return detail;
        }

        public void Update(CreateInRequestInput input)
        {
            var @entity = _inRequestRepository.Get(input.Id);
            var @entityDetail = _inRequestManager.GetEditDetails(input.Id).ToList();

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            //if (_inRequestManager.InRequestExist(@entity.RequestDocumentNumber, input.Id,input.CompanyName))
            //{
            //    throw new UserFriendlyException("Existe una Ubicación con el mismo Nombre.");
            //}
            IList<Detail> newDetails = new List<Detail>();
            IList<Detail> updateDetails = new List<Detail>();

            //@entity.RequestDocumentNumber = input.RequestDocumentNumber;
            //@entity.AssetsReturnDate = input.AssetsReturnDate;
            @entity.TypeInRequest = (TypeInRequest)input.TypeInRequestValue;
            @entity.Status = (InRequestStatus)input.StateRequest;
            @entity.ProjectId = input.ProjectId;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.CreatorUserId;
            @entity.Notes = input.Notes;
            @entity.PersonInCharge = input.PersonInCharge;

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
                            updateDetails.Add(@updateEntity);
                        }
                    }
                    else if (item.Update == 1)
                    {
                        @updateEntity.Price = Double.Parse(item.Price);
                        @updateEntity.NameAsset = item.NameAsset;
                        @updateEntity.StockAsset = item.StockAsset;
                        @updateEntity.LastModificationTime = _dateTime.Now;
                        @updateEntity.LastModifierUserId = input.CreatorUserId;
                        updateDetails.Add(@updateEntity);
                    }
                }
                else
                {
                    var @createDetail = Detail.Create(input.Id, null, null, item.AssetId, item.NameAsset, item.StockAsset, Double.Parse(item.Price),
                    input.CreatorUserId, _dateTime.Now,input.CompanyName);
                    newDetails.Add(@createDetail);
                }
            }

            if (HasFile(input.Image1))
            {
                string folderPath = "~/InRequestImages/";
                string fileName = input.Image1.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../InRequestImages/";
                input.ImagePath1 = folderPathRelative + fileName;
                input.Image1.SaveAs(mappedPath);

                @entity.SetImage1(input.ImagePath1);
            }

            if (HasFile(input.Image2))
            {
                string folderPath = "~/InRequestImages/";
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

                @entity.SetImage2(input.ImagePath2);
            }
            if (!string.IsNullOrEmpty(input.SignatureData))
            {
                @entity.SetSignatureData(input.SignatureData);
            }

            _inRequestManager.Update(@entity, newDetails, updateDetails);
        }

        private static bool HasFile(HttpPostedFile file)
        {
            return file != null && file.ContentLength > 0;
        }

        public void Create(CreateInRequestInput input)
        {
            //if (_inRequestManager.InRequestExist(input.RequestDocumentNumber, input.Id, input.CompanyName))
            //{
            //    throw new UserFriendlyException("Existe una solicitud con el mismo numero de Solicitud Fisica.");
            //}
            TypeInRequest temp = (TypeInRequest)input.TypeInRequestValue;
            InRequestStatus temp1;

            temp1 = InRequestStatus.Active;

            var requestNumber = GetNextRequestNumber(input.CompanyName);

            var @entityInRequest = InRequest.Create(requestNumber, requestNumber.ToString(), input.PurchaseOrderNumber, input.Notes, null,
                input.CellarId, temp1, temp, input.CreatorGuidId.Value, _dateTime.Now, input.PersonInCharge,input.CompanyName, input.Comment);

            IList<Detail> @details = new List<Detail>();
            foreach (var item in input.DetailsRequest)
            {
                var @entityDetail = Detail.Create(@entityInRequest.Id, null, null, item.AssetId, item.NameAsset, item.StockAsset, Double.Parse(item.Price),
                    input.CreatorGuidId.Value, _dateTime.Now,input.CompanyName);
                @details.Add(@entityDetail);
            }

            if (HasFile(input.Image1))
            {
                string folderPath = "~/InRequestImages/";
                string fileName = input.Image1.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../InRequestImages/";
                input.ImagePath1 = folderPathRelative + fileName;
                input.Image1.SaveAs(mappedPath);

                @entityInRequest.SetImage1(input.ImagePath1);
            }

            if (HasFile(input.Image2))
            {
                string folderPath = "~/InRequestImages/";
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

                @entityInRequest.SetImage2(input.ImagePath2);
            }
            if (!string.IsNullOrEmpty(input.SignatureData))
            {
                @entityInRequest.SetSignatureData(input.SignatureData);
            }

            _inRequestManager.Create(@entityInRequest, @details);
        }



        private IList<Movement> Movements(IList<Detail> details, IList<StockMap> stocksGeneral, Guid cellarId, Guid userId, Guid inRequestId,string companyName)
        {
            //Movimiento
            IList<Movement> movements = new List<Movement>();
            var lastmov = _inRequestManager.GetNextMovementNumber(companyName);
            double montoTotalInventario = stocksGeneral.Sum(a => a.GetAmountInStock());
            double montoTotalBodega = stocksGeneral.Where(a => a.CellarId == cellarId).Sum(a => a.GetAmountInStock());
            foreach (var item in details)
            {
                var stockListCellar = _inRequestManager.GetStocksList(companyName, cellarId, item.AssetId);
                var articuloExistenciaInventarioArticulo = stockListCellar.Where(a => a.AssetId == item.AssetId);
                var articuloExistenciaBodega = articuloExistenciaInventarioArticulo.FirstOrDefault(a => a.CellarId == cellarId);
                if (articuloExistenciaBodega != null)
                {
                    double cntAnteriorExistenciaBodegaMigrado = articuloExistenciaBodega.GetStockItemsQty();
                    double cntAntExisteciasInvMigrado = articuloExistenciaInventarioArticulo.Sum(a => a.GetStockItemsQty());
                    // Montos antes del movimiento en todo el inventario
                    double montoAnteriorExistenciaBodega = articuloExistenciaBodega.GetAmountInStock();
                    double montoAntExistenciasInv = articuloExistenciaInventarioArticulo.Sum(a => a.GetAmountInStock());
                    // Monto en todo el inventario y bodega
                    double previousGeneralInvAmount = montoTotalInventario;
                    double previousGeneralStockAmount = montoTotalBodega;

                    var @entity = Movement.Create(lastmov, item.StockAsset, item.Price, _dateTime.Now,
                        StatusMovement.AplicadoInventario, TypeMovement.Input, MovementCategory.InRequest, cntAnteriorExistenciaBodegaMigrado,
                        montoAnteriorExistenciaBodega, cntAntExisteciasInvMigrado, montoAntExistenciasInv,
                        previousGeneralInvAmount, previousGeneralStockAmount, cellarId, item.AssetId, userId, inRequestId,
                        null, null, null, userId, _dateTime.Now, companyName);

                    montoTotalInventario += @entity.GetAmountMovement(); //Actualiza monto inventario
                    montoTotalBodega += @entity.GetAmountMovement(); //Actualiza monto bodega
                    movements.Add(@entity);
                }
                else
                {
                    double cntAnteriorExistenciaBodegaMigrado = 0;
                    double cntAntExisteciasInvMigrado = 0;
                    // Montos antes del movimiento en todo el inventario
                    double montoAnteriorExistenciaBodega = 0;
                    double montoAntExistenciasInv = 0;
                    // Monto en todo el inventario y bodega
                    double previousGeneralInvAmount = montoTotalInventario;
                    double previousGeneralStockAmount = montoTotalBodega;
                    var @entity = Movement.Create(lastmov, item.StockAsset, item.Price, _dateTime.Now,
                        StatusMovement.AplicadoInventario, TypeMovement.Input, MovementCategory.InRequest, cntAnteriorExistenciaBodegaMigrado,
                        montoAnteriorExistenciaBodega, cntAntExisteciasInvMigrado, montoAntExistenciasInv,
                        previousGeneralInvAmount, previousGeneralStockAmount, cellarId, item.AssetId, userId, inRequestId,
                        null, null, null, userId, _dateTime.Now, companyName);

                    montoTotalInventario += @entity.GetAmountMovement(); //Actualiza monto inventario
                    montoTotalBodega += @entity.GetAmountMovement(); //Actualiza monto bodega
                    movements.Add(@entity);
                }
            }
            return movements;
        }

        public void Delete(Guid id, Guid userId)
        {
            var @entity = _inRequestRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.IsDeleted = true;
            @entity.DeleterUserId = userId;
            @entity.DeletionTime = _dateTime.Now;

            IList<Detail> detailRemove = new List<Detail>();
            IList<Stock> stockRemove = new List<Stock>();
            var @entityDetail = _inRequestManager.GetEditDetails(id).ToList();
            foreach (var item in @entityDetail)
            {
                item.DeleterUserId = userId;
                item.IsDeleted = true;
                item.DeletionTime = _dateTime.Now;
                detailRemove.Add(item);
            }
            _inRequestManager.Delete(@entity, detailRemove);
        }
       /* public Stock GetStockAsset(Guid id, string companyNamed)
        {
            var stock = _inRequestManager.GetStockAsset(id, companyNamed);
            return stock;
        }*/
        public void ChangeProcessedStatus(Guid inRequestId, Guid userId, string company, IList<StockMap> stockList)
        {
            var @entity = _inRequestRepository.Get(inRequestId);
            var @entityDetails = _inRequestManager.GetEditDetails(inRequestId).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.Status = InRequestStatus.Processed;
            @entity.LastModifierUserId = userId;
            @entity.LastModificationTime = _dateTime.Now;

            @entity.ProcessedDate = _dateTime.Now;

            //var stocksList = _inRequestManager.GetStocksList(company);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId).ToList();

            IList<Movement> @movement = new List<Movement>();
            IList<Stock> @updatestocks = new List<Stock>();
            IList<Stock> @newstocks = new List<Stock>();

            IList<PriceChange> @priceChanges = new List<PriceChange>();
            @movement = Movements(@entityDetails, stockList, @entity.CellarId, userId, @entity.Id, company);
            //var listAsset = _inRequestManager.SearchAsset("", @entity.TypeInRequest,company).ToList();
            foreach (var item in @entityDetails)
            {
                var stockListCellar = _inRequestManager.GetStocksList(company, entity.CellarId, item.AssetId);
                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                double priceList = _inRequestManager.GetAssetId(item.AssetId).Price;
                if (item.Price != priceList)
                {
                    var @changePrice = PriceChange.Create(item.AssetId, priceList, item.Price, userId, _dateTime.Now,company);
                    @priceChanges.Add(@changePrice);
                }

                //var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                if (@stockUpdate != null)
                {
                    @stockUpdate.AssetId = item.AssetId;
                    @stockUpdate.CellarId = @entity.CellarId;
                    @stockUpdate.AddToStock(item.StockAsset, item.Price);
                    @updatestocks.Add(@stockUpdate);
                }
                else
                {
                    var @stock = Stock.Create(@entity.CellarId, item.AssetId, item.StockAsset, item.Price, userId, _dateTime.Now,company);
                    @newstocks.Add(@stock);
                }
            }
            _inRequestManager.ChangeStatus(@entity, @entityDetails, @newstocks, @updatestocks, @movement, @priceChanges);
        }

        public int GetNextRequestNumber(string company)
        {
            return _inRequestManager.GetNextRequestNumber(company);
        }

        public User GetUser(Guid userId)
        {
            var @entity = _inRequestManager.GetUser(userId);
            return @entity;
        }

        public IList<Cellar> GetAllCellars(string company)
        {
            return _inRequestManager.GetCellarsList(company);
        }

        public IList<User> GetAllUsers(string company)
        {
            return _inRequestManager.GetUsersList(company);
        }

        public Asset GetAssetBarCode(string code, string companyName)
        {
            var @asset = _inRequestManager.GetAssetBarcode(code, companyName);
            return @asset;
        }

        public IPagedList<Stock> GetAllStocks(string query, Guid? cellarId, Guid? assetId, int defaultPageSize, int? page)
        {
            throw new NotImplementedException();
        }

        /*  public IPagedList<Stock> GetAllStocks(string query, Guid? cellarId, Guid? assetId, int defaultPageSize, int? page)
          {
              throw new NotImplementedException();
          }*/
    }
}
