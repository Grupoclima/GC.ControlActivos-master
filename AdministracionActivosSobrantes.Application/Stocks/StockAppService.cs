using System;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Stocks.Dto;
using System.Collections.Generic;

namespace AdministracionActivosSobrantes.Stocks
{
    class StockAppService : ApplicationService, IStockAppService
    {
        private readonly IRepository<Stock, Guid> _stockRepository;
        private readonly IStockManager _stockManager;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public StockAppService(IRepository<Stock, Guid> stockRepository, IStockManager stockManager, IDateTime dateTime)
        {
            _stockRepository = stockRepository;
            _stockManager = stockManager;
            _dateTime = dateTime;
        }

        /*public IPagedList<StockDto> SearchStock(SearchStocksInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value - 1 : 0;

            if (searchInput.Query == null)
                searchInput.Query = "";

            var @entities = _StockRepository.GetAll().Where(c => c.IsDeleted != null && c.IsDeleted.Value == false && (c.Name.Contains(searchInput.Query)
                                                                || c.Address.Contains(searchInput.Query))
                                                           ).OrderByDescending(p => p.Name);

            return @entities.MapTo<List<StockDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }*/


       /* public GetStockOutput Get(Guid id)
        {
            var @entity = _StockRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            return Mapper.Map<GetStockOutput>(@entity);
        }*/

        /*public  UpdateStockInput GetEdit(Guid id)
        {
            var @entity = _StockRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            return Mapper.Map<UpdateStockInput>(@entity);
        }*/

/*        public DetailStockOutput GetDetail(Guid id)
        {
            var @entity = _StockRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            var dto = Mapper.Map<DetailStockOutput>(@entity);
            dto.CreatorUser = _StockManager.GetUser(dto.CreatorUserId);
            dto.LastModifierUser = _StockManager.GetUser(dto.LastModifierUserId);
            return dto;
        }*/


        /*public void Update(UpdateStockInput input)
        {
            var @entity = _StockRepository.Get(input.Id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
            }
            if (_StockManager.StockExist(input.Name,input.Id))
            {
                throw new UserFriendlyException("Existe una Ubicación con el mismo Nombre.");
            }

            @entity.Name = input.Name;
            @entity.Address = input.Address;
            @entity.Phone = input.Phone;
            @entity.WareHouseManagerId = input.WareHouseManagerId;
            @entity.Latitude = input.Latitude;
            @entity.Longitude= input.Longitude;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.LastModifierUserId;

            _StockRepository.Update(@entity);
        }*/

        public void Create(CreateStockInput input)
        {
            if (input.Price < 1)
            {
                throw new UserFriendlyException("El monto no puede ser 0.");
            }

            var @entity = Stock.Create(input.CellarId, input.AssetId, input.Qty, input.Price,input.CreatorUserId, _dateTime.Now, input.CompanyName);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo crear la existencia.");
            }
            _stockRepository.Insert(@entity);
        }

        public IList<Stock> GetStockInformation(Guid id,string company)
        {
            IList<Stocks.Stock> stockList = new List<Stocks.Stock>();
            stockList = _stockManager.SearchStockInfo(id,company);
         if (stockList == null)
         {
             throw new UserFriendlyException("No se pudo encontrar el Activo, fue borrado o no existe.");
         }
         return stockList;
    }
        
        //public void Delete(Guid id, Guid userId)
        //{
        //    var @entity = _StockRepository.Get(id);
        //    if (@entity == null)
        //    {
        //        throw new UserFriendlyException("No se pudo encontrar la Ubicación, fue borrada o no existe.");
        //    }
        //    @entity.IsDeleted = true;
        //    @entity.DeleterUserId = userId;
        //    @entity.DeletionTime = _dateTime.Now;
        //    _StockRepository.Update(@entity);
        //}
    }
}
