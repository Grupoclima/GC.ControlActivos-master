using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.CustomFields;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.ToolAssets;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Assets
{
    public class AssetManager :DomainService, IAssetManager
    {
        public IEventBus EventBus { get; set; }
        private readonly IRepository<Asset, Guid> _assetRepository;
        private readonly IRepository<Category, Guid> _categoryRepository;
        private readonly IRepository<Cellar, Guid> _cellarRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<CustomField, Guid> _customFieldsRepository;
        private readonly IRepository<ToolAsset, Guid> _toolAssetRepository;
        private readonly IDateTime _dateTime;

        public AssetManager(IRepository<Asset, Guid> assetRepository, IRepository<Category, Guid> categoryRepository,
            IRepository<Cellar, Guid> cellarRepository, IRepository<User, Guid> userRepository,IRepository<CustomField, Guid> customFieldsRepository, 
            IDateTime dateTime, IRepository<ToolAsset, Guid> toolAssetRepository)
        {
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _customFieldsRepository = customFieldsRepository;
            _dateTime = dateTime;
            _cellarRepository = cellarRepository;
            _toolAssetRepository = toolAssetRepository;
            EventBus = NullEventBus.Instance;
        }

        public void Create(Asset asset)
        {
            _assetRepository.Insert(asset);
        }

        public void Update(Asset asset, IList<CustomField> newDetails, IList<CustomField> removeDetails, IList<CustomField> updateDetails,
            IList<ToolAsset> newToolKit, IList<ToolAsset> removeToolKit, IList<ToolAsset> updateToolKit)
        {
            _assetRepository.Update(asset);
            foreach (var cf in updateDetails)
            {
                cf.AssetId = asset.Id;
                _customFieldsRepository.Update(cf);
            }
            foreach (var cf in newDetails)
            {
                cf.AssetId = asset.Id;
                _customFieldsRepository.Insert(cf);
            }
            foreach (var cf in removeDetails)
            {
                cf.AssetId = asset.Id;
                _customFieldsRepository.Update(cf);
            }

            foreach (var cf in updateToolKit)
            {
                cf.AssetId = asset.Id;
                _toolAssetRepository.Update(cf);
            }
            foreach (var cf in newToolKit)
            {
                cf.AssetId = asset.Id;
                _toolAssetRepository.Insert(cf);
            }
            foreach (var cf in removeToolKit)
            {
                cf.AssetId = asset.Id;
                _toolAssetRepository.Update(cf);
            }
        }

        public IQueryable<Asset> GetEdit(Guid id, string company)
        {
            var list = _assetRepository.GetAll().Where(a => a.IsDeleted == false && a.CompanyName.Equals(company) && a.Id == id).Include(a => a.CustomFields).OrderBy(a => a.CreationTime);
            return list;
        }

        public bool AssetExist(string code, Guid id, string company)
        {
            var @entity = _assetRepository.FirstOrDefault(e =>e.Code.Equals(code) && e.CompanyName.Equals(company) &&e.Id != id && e.IsDeleted.Value == false);
            return @entity != null;
        }

        public IQueryable<CustomField> GetAssetCustomFields(Guid id, string company)
        {
            var details = _customFieldsRepository.GetAll().Where(a => a.IsDeleted == false && a.CompanyName.Equals(company) && a.AssetId == id).Include(a => a.Asset).OrderBy(a => a.Name);
            return details;
        }

        public IQueryable<ToolAsset> GetAssetToolAsset(Guid id, string company)
        {
            var details = _toolAssetRepository.GetAll().Where(a => a.IsDeleted == false && a.CompanyName.Equals(company)&& a.AssetId == id).Include(a => a.Asset).OrderBy(a => a.Name);
            return details;
        }

        public IList<Category> GetCategoriesList(string company)
        {
            var @entities = _categoryRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public IList<Cellar> GetCellarsList(string company)
        {
            var @entities = _cellarRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
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
        public void updateOutInRequest (Asset asset)
        {
            _assetRepository.Update(asset);

        }
    }
}
