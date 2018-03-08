using System;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;
using System.Collections.Generic;

namespace AdministracionActivosSobrantes.Stocks
{
    public class StockManager : DomainService, IStockManager
    {
        public IEventBus EventBus { get; set; }
        private readonly IRepository<Stock, Guid> _stockRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IDateTime _dateTime;

        public StockManager(IRepository<Stock, Guid> stockRepository, IRepository<User, Guid> userRepository, IDateTime dateTime)
        {
            _stockRepository = stockRepository;
            _userRepository = userRepository;
            _dateTime = dateTime;
            EventBus = NullEventBus.Instance;
        }


        public User GetUser(Guid? id)
        {
            if (id == null)
                return null;

            Guid idTemp = id.Value;
            var @entity = _userRepository.Get(idTemp);
            return @entity;
        }

        public void Create(Stock @entity)
        {
            _stockRepository.Insert(@entity);
        }

        public IList<Stock> SearchStockInfo(Guid assetId,string company)
        {
            var @entities = _stockRepository.GetAllList(a => a.AssetId == assetId && a.CompanyName.Equals(company));
            return @entities;
        }

    }
}
