using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Adjustments.Dto;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Details;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.PriceChanges;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;
using AdministracionActivosSobrantes.Users;
using AutoMapper;
using MvcPaging;

namespace AdministracionActivosSobrantes.Adjustments
{
    class AdjustmentAppService : ApplicationService, IAdjustmentAppService
    {
        private readonly IRepository<Adjustment, Guid> _adjustmentRepository;
        private readonly IAdjustmentManager _adjustmentManager;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public AdjustmentAppService(IRepository<Adjustment, Guid> adjustmentRepository, IAdjustmentManager adjustmentManager, IDateTime dateTime)
        {
            _adjustmentRepository = adjustmentRepository;
            _adjustmentManager = adjustmentManager;
            _dateTime = dateTime;
        }

        public IPagedList<AdjustmentDto> SearchAdjustment(SearchAdjustmentInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;
            var @entities = _adjustmentManager.SearchRequests(searchInput.Query, searchInput.CellarId, searchInput.ProjectId, searchInput.UserId.Value,searchInput.CompanyName).ToList();
            return @entities.MapTo<List<AdjustmentDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public DetailAdjustmentOutput Get(Guid id)
        {
            var @entity = _adjustmentRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Bodega, fue borrada o no existe.");
            }
            return Mapper.Map<DetailAdjustmentOutput>(@entity);
        }

        public CreateAdjustmentInput GetEdit(Guid id)
        {
            var @entity = _adjustmentManager.GetEdit(id).FirstOrDefault();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }
            return Mapper.Map<CreateAdjustmentInput>(@entity);
        }

        public User GetUser(Guid userId)
        {
            var @entity = _adjustmentManager.GetUser(userId);
            return @entity;
        }

        public DetailAdjustmentOutput GetDetail(Guid id)
        {
            var @entity = _adjustmentRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Bodega, fue borrada o no existe.");
            }
            return Mapper.Map<DetailAdjustmentOutput>(@entity);
        }

        public IList<Cellar> GetAllCellars(string company)
        {
            return _adjustmentManager.GetCellarsList(company);
        }
        public IList<User> GetAllUsers(string company)
        {
            return _adjustmentManager.GetUsersList(company);
        }
        public IList<DetailAssetAdjustmentDto> GetEditDetails(Guid outRequestId, Guid cellarId)
        {
            IList<DetailAssetAdjustmentDto> detail = new List<DetailAssetAdjustmentDto>();
            var @entity = _adjustmentManager.GetEditDetails(outRequestId).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Bodega, fue borrada o no existe.");
            }
            int index = 0;
            foreach (var item in @entity)
            {
                DetailAssetAdjustmentDto detailItem = new DetailAssetAdjustmentDto();
                detailItem.Asset = item.Asset;
                detailItem.NameAsset = item.NameAsset;
                detailItem.AdjustmentId = item.AdjustmentId;
                detailItem.StockAsset = item.StockAsset;
                detailItem.AssetId = item.AssetId;
                detailItem.AssetCode = item.Asset.Code;
                detailItem.Price = item.Price;
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

        public void Update(CreateAdjustmentInput input)
        {
            var @entity = _adjustmentRepository.Get(input.Id);
            var @entityDetail = _adjustmentManager.GetEditDetails(input.Id).ToList();

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Bodega, fue borrada o no existe.");
            }
            if (_adjustmentManager.AdjustmentExist(@entity.RequestDocumentNumber, input.Id, input.CompanyName))
            {
                throw new UserFriendlyException("Existe una Bodega con el mismo Nombre.");
            }
            IList<Detail> newDetails = new List<Detail>();
            IList<Detail> updateDetails = new List<Detail>();

            //@entity.RequestDocumentNumber = ;
            @entity.TypeAdjustment = (TypeAdjustment)input.TypeAdjustmentValue;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.CreatorUserId;
            @entity.PersonInCharge = input.PersonInCharge;

            foreach (var item in input.DetailsAdjustment)
            {
                if (@entityDetail.Exists(a => a.AssetId == item.AssetId))
                {
                    var @updateEntity = @entityDetail.FirstOrDefault(a => a.AssetId == item.AssetId);
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
                        @updateEntity.Price = item.Price;
                        @updateEntity.NameAsset = item.NameAsset;
                        @updateEntity.StockAsset = item.StockAsset;
                        @updateEntity.LastModificationTime = _dateTime.Now;
                        @updateEntity.LastModifierUserId = input.CreatorUserId;
                        updateDetails.Add(@updateEntity);
                    }
                }
                else
                {
                    var @createDetail = Detail.Create(null, null, input.Id, item.AssetId, item.NameAsset, item.StockAsset, item.Price,
                    input.CreatorUserId, _dateTime.Now,input.CompanyName);
                    newDetails.Add(@createDetail);
                }
            }
            _adjustmentManager.Update(@entity, newDetails, updateDetails);
        }

        public void Create(CreateAdjustmentInput input)
        {
            if (_adjustmentManager.AdjustmentExist(input.RequestDocumentNumber, input.Id,input.CompanyName))
            {
                throw new UserFriendlyException("Existe una solicitud con el mismo numero de Solicitud Fisica.");
            }
            TypeAdjustment temp = (TypeAdjustment)input.TypeAdjustmentValue;


            var requestNumber = GetNextRequestNumber(input.CompanyName);

            var @entityAdjustment = Adjustment.Create(requestNumber, requestNumber.ToString(), input.Notes,
                input.CellarId, AdjustmentStatus.Active, temp, input.CreatorGuidId.Value, _dateTime.Now, input.PersonInCharge, input.CompanyName);

            IList<Detail> @details = new List<Detail>();


            foreach (var item in input.DetailsAdjustment)
            {
                var @entityDetail = Detail.Create(null, null, @entityAdjustment.Id, item.AssetId, item.NameAsset, item.StockAsset, item.Price,
                    input.CreatorGuidId.Value, _dateTime.Now,input.CompanyName);
                @details.Add(@entityDetail);
            }

            _adjustmentManager.Create(@entityAdjustment, @details);

        }

        private IList<Movement> Movements(IList<Detail> details, IList<StockMap> stocksGeneral, Guid cellarId, Guid userId, Guid adjustmentId, string company)
        {
            //Movimiento
            IList<Movement> movements = new List<Movement>();
            var lastmov = _adjustmentManager.GetNextMovementNumber(company);
            double montoTotalInventario = stocksGeneral.Sum(a => a.GetAmountInStock());
            double montoTotalBodega = stocksGeneral.Where(a => a.CellarId == cellarId).Sum(a => a.GetAmountInStock());
            double stockMovement = 0;
            TypeMovement temp = TypeMovement.Input;
            foreach (var item in details)
            {
                var stockListCellar = _adjustmentManager.GetStocksList(company, cellarId, item.AssetId);
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

                if (cntAnteriorExistenciaBodegaMigrado > item.StockAsset)
                {
                    stockMovement = cntAntExisteciasInvMigrado - item.StockAsset;
                    temp = TypeMovement.Output;
                }
                else if (cntAnteriorExistenciaBodegaMigrado< item.StockAsset)
                {
                    stockMovement = item.StockAsset - cntAntExisteciasInvMigrado;
                    temp = TypeMovement.Input;
                }


                var @entity = Movement.Create(lastmov, stockMovement, item.Price, _dateTime.Now,
                    StatusMovement.AplicadoInventario, temp, MovementCategory.Adjustment, cntAnteriorExistenciaBodegaMigrado, montoAnteriorExistenciaBodega, cntAntExisteciasInvMigrado, montoAntExistenciasInv,
                    previousGeneralInvAmount, previousGeneralStockAmount, cellarId, item.AssetId, userId, null, null, adjustmentId, null, userId, _dateTime.Now,company);

                if (cntAnteriorExistenciaBodegaMigrado > item.StockAsset)
                {
                    montoTotalInventario -= @entity.GetAmountMovement(); //Actualiza monto inventario
                    montoTotalBodega -= @entity.GetAmountMovement(); //Actualiza monto bodega
                }
                else
                {
                    montoTotalInventario += @entity.GetAmountMovement(); //Actualiza monto inventario
                    montoTotalBodega += @entity.GetAmountMovement(); //Actualiza monto bodega
                }
               

                movements.Add(@entity);
            }

            return movements;

        }

        public void Delete(Guid id, Guid userId, string company)
        {
            var @entity = _adjustmentRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.IsDeleted = true;
            @entity.DeleterUserId = userId;
            @entity.DeletionTime = _dateTime.Now;

            var stocksList = _adjustmentManager.GetStocksList(company);
            var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId);
            IList<Detail> detailRemove = new List<Detail>();
            IList<Stock> stockRemove = new List<Stock>();
            var @entityDetail = _adjustmentManager.GetEditDetails(id).ToList();
            foreach (var item in @entityDetail)
            {
                item.DeleterUserId = userId;
                item.IsDeleted = true;
                item.DeletionTime = _dateTime.Now;
                detailRemove.Add(item);

                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @stockUpdate.IsDeleted = true;
                @stockUpdate.DeletionTime = _dateTime.Now;
                @stockUpdate.DeleterUserId = userId;
                @stockUpdate.RemoveFromBlockedStock(item.StockAsset, item.Price);
                stockRemove.Add(@stockUpdate);

            }
            _adjustmentManager.Delete(@entity, stockRemove, detailRemove);
        }


        public IList<User> GetAllWareHouseUsers()
        {
            throw new NotImplementedException();
        }


        public IPagedList<Stock> GetAllStocks(string query, Guid? cellarId, TypeAdjustment? typeAdjustment, int defaultPageSize, int? page, string company)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            var stocks = _adjustmentManager.SearchAsset(query, cellarId, typeAdjustment, company);
            int totalCount = stocks.Count();
            return stocks.Skip(currentPageIndex * defaultPageSize).Take(defaultPageSize).MapTo<List<Stock>>().ToPagedList(currentPageIndex, defaultPageSize, totalCount);
            
        }

        public int GetTotalItem(string query, Guid? cellarId, TypeAdjustment? typeAdjustment, string company)
        {
            var @entities = _adjustmentManager.SearchAsset(query, cellarId, typeAdjustment, company);
            int totalCount = @entities.Count();
            return totalCount;
        }

        public int GetNextRequestNumber(string company)
        {
            return _adjustmentManager.GetNextRequestNumber(company);
        }


        public void ChangeProcessedStatus(Guid adjustmentId, Guid userId, string company, IList<StockMap> stockList)
        {
            var @entity = _adjustmentRepository.Get(adjustmentId);
            var @entityDetails = _adjustmentManager.GetEditDetails(adjustmentId).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la solicitud, fue borrada o no existe.");
            }
            @entity.Status = AdjustmentStatus.Processed;
            @entity.LastModifierUserId = userId;
            @entity.LastModificationTime = _dateTime.Now;

            //var stocksList = _adjustmentManager.GetStocksList(company);
            //var stockListCellar = stocksList.Where(a => a.CellarId == @entity.CellarId).ToList();

            IList<Movement> @movement = new List<Movement>();
            IList<Stock> @updatestocks = new List<Stock>();

            IList<PriceChange> @priceChanges = new List<PriceChange>();
            //var listAsset = _adjustmentManager.SearchAsset("", company).ToList();
            double stockMovement = 0;
            @movement = Movements(@entityDetails, stockList, @entity.CellarId.Value, userId, adjustmentId, company);
            foreach (var item in @entityDetails)
            {
                var stockListCellar = _adjustmentManager.GetStocksList(company, entity.CellarId.Value, item.AssetId);
                double priceList = _adjustmentManager.GetAssetId(item.AssetId).Price;
                if (item.Price != priceList)
                {
                    var @changePrice = PriceChange.Create(item.AssetId, priceList, item.Price, userId, _dateTime.Now,company);
                    @priceChanges.Add(@changePrice);
                }

                var @stockUpdate = stockListCellar.FirstOrDefault(a => a.AssetId == item.AssetId);
                if (@stockUpdate.GetStockItemsQty() > item.StockAsset)
                {
                    stockMovement = @stockUpdate.GetStockItemsQty() - item.StockAsset;
                    @stockUpdate.RemoveFromStock(stockMovement, item.Price);
                }
                else
                {
                    stockMovement = item.StockAsset - @stockUpdate.GetStockItemsQty();
                    @stockUpdate.AddToStock(stockMovement, item.Price);
                }
                @stockUpdate.AssetId = item.AssetId;
                @stockUpdate.CellarId = @entity.CellarId.Value;
                @updatestocks.Add(@stockUpdate);

            }
            _adjustmentManager.ChangeStatus(@entity, @entityDetails,  @updatestocks, @movement, @priceChanges);
        }
    }
}
