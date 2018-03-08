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
using AdministracionActivosSobrantes.Assets.Dto;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.CustomFields;
using AdministracionActivosSobrantes.CustomFields.Dto;
using AdministracionActivosSobrantes.ToolAssets;
using AutoMapper;
using MvcPaging;

namespace AdministracionActivosSobrantes.Assets
{
    class AssetsAppService : ApplicationService, IAssetsAppService
    {
        private readonly IRepository<Asset, Guid> _assetRepository;
        private readonly IAssetManager _assetManager;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public AssetsAppService(IRepository<Asset, Guid> assetRepository, IAssetManager assetManager, IDateTime dateTime)
        {
            _assetRepository = assetRepository;
            _assetManager = assetManager;
            _dateTime = dateTime;
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

            @entities = @entities.Where(c => c.IsDeleted != null && c.IsDeleted.Value == false && c.CompanyName.Equals(searchInput.CompanyName)&&
            ((c.Name.ToLower().Contains(searchInput.Query) || c.Code.ToLower().Contains(searchInput.Query)) ||
                c.Brand.ToLower().Contains(searchInput.Query) || c.Brand.ToLower().Equals(searchInput.Query) ||
                c.Category.Name.ToLower().Contains(searchInput.Query) || c.Category.Name.ToLower().Equals(searchInput.Query) ||
                c.ModelStr.ToLower().Contains(searchInput.Query) || c.ModelStr.ToLower().Equals(searchInput.Query) ||
                c.Plate.ToLower().Contains(searchInput.Query) || c.Plate.ToLower().Equals(searchInput.Query) ||
                c.Series.ToLower().Contains(searchInput.Query) || c.Series.ToLower().Equals(searchInput.Query)));

            return @entities.OrderByDescending(p => p.Code).Skip(currentPageIndex * searchInput.MaxResultCount).Take(searchInput.MaxResultCount).MapTo<List<AssetDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount,totalCount);
        }

        public IList<Category> GetAllCategories(string company)
        {
            return _assetManager.GetCategoriesList(company);
        }

        public IList<Cellar> GetAllCellars(string company)
        {
            return _assetManager.GetCellarsList(company);
        }

        public GetAssetOutput Get(Guid id)
        {
            var @entity = _assetRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }
            return Mapper.Map<GetAssetOutput>(@entity);
        }

        public UpdateAssetInput GetEdit(Guid id, string company)
        {
            var @entity = _assetManager.GetEdit(id,company).FirstOrDefault();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }
            return Mapper.Map<UpdateAssetInput>(@entity);
        }

        public IList<CustomFieldDto> GetEditCustomFields(Guid assetId, string company)
        {
            IList<CustomFieldDto> cfList = new List<CustomFieldDto>();
            var @entity = _assetManager.GetAssetCustomFields(assetId,company).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Artículo, fue borrado o no existe.");
            }
            int index = 0;
            foreach (var item in @entity)
            {
                CustomFieldDto cfItem = new CustomFieldDto();
                cfItem.Id = item.Id;
                cfItem.Name = item.Name;
                cfItem.AssetId = item.AssetId;
                cfItem.SetValue(item.CustomFieldType, item.Value);
                cfItem.AssetId = item.AssetId;
                //cfItem.Asset = item.Asset;
                cfItem.Index = index;
                cfItem.Update = 1;
                cfItem.Saved = 1;
                cfItem.Delete = 0;
                cfItem.ErrorCode = 0;
                cfItem.ErrorDescription = "";
                cfItem.CompanyName = company;
                cfList.Add(cfItem);
                index++;
            }
            return cfList;
        }

        public IList<DetailAssetToolKitsDto> GetEditToolKits(Guid assetId, string company)
        {
            IList<DetailAssetToolKitsDto> taList = new List<DetailAssetToolKitsDto>();
            var @entity = _assetManager.GetAssetToolAsset(assetId, company).ToList();
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el Artículo, fue borrado o no existe.");
            }
            int index = 0;
            foreach (var item in @entity)
            {
                DetailAssetToolKitsDto toolKitItem = new DetailAssetToolKitsDto();
                toolKitItem.Id = item.Id;
                toolKitItem.Name = item.Name;
                toolKitItem.Code = item.Code;
                toolKitItem.Quatity = item.Quatity;
                toolKitItem.AssetId = item.AssetId;
                toolKitItem.Index = index;
                toolKitItem.Update = 1;
                toolKitItem.Saved = 1;
                toolKitItem.Delete = 0;
                toolKitItem.CompanyName = company;
                toolKitItem.ErrorCode = 0;
                toolKitItem.ErrorDescription = "";
                taList.Add(toolKitItem);
                index++;
            }
            return taList;
        }

        public DetailAssetOutput GetDetail(Guid id, string company)
        {
            var @entity = _assetRepository.Get(id);

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }

            var dto = Mapper.Map<DetailAssetOutput>(@entity);
            dto.CreatorUser = _assetManager.GetUser(dto.CreatorUserId);
            dto.LastModifierUser = _assetManager.GetUser(dto.LastModifierUserId);
            dto.NowDate = _dateTime.Now;
            return dto;
        }

        public void Update(UpdateAssetInput input)
        {
            var @entity = _assetRepository.Get(input.Id);
            var listAssetCustomFields = _assetManager.GetAssetCustomFields(input.Id,input.CompanyName).ToList();
            var listAssetToolKits = _assetManager.GetAssetToolAsset(input.Id, input.CompanyName).ToList();

            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }
            if (_assetManager.AssetExist(@entity.Code, input.Id, input.CompanyName))
            {
                throw new UserFriendlyException("Existe un Artículo con el mismo Código.");
            }

            @entity.Name = input.Name;
            @entity.Description = input.Description;
            @entity.Code = input.Code;
            @entity.Brand = input.Brand;
            @entity.Series = input.Series;
            @entity.ModelStr = input.ModelStr;
            @entity.Plate = input.Plate;
            @entity.Barcode = input.Barcode;
            @entity.PurchaseDate = input.PurchaseDate;
            @entity.IsAToolInKit = input.IsAToolInKit;
            @entity.SetDepretiation(input.AdmissionDate, input.DepreciationMonthsQty);
            @entity.AssetType = input.AssetType;
            @entity.CategoryId = input.CategoryId;
            @entity.LastModificationTime = _dateTime.Now;
            @entity.LastModifierUserId = input.LastModifierUserId;
                     

            IList<CustomField> newCustomFields = new List<CustomField>();
            IList<CustomField> updateCustomFields = new List<CustomField>();
            IList<CustomField> deleteCustomFields = new List<CustomField>();

            IList<ToolAsset> newToolKits = new List<ToolAsset>();
            IList<ToolAsset> updateToolKits = new List<ToolAsset>();
            IList<ToolAsset> deleteToolKits = new List<ToolAsset>();

            foreach (var item in input.CustomFieldsDto)
            {
                if (listAssetCustomFields.Exists(a => a.Id == item.Id))
                {
                    CustomField updateCustomField = listAssetCustomFields.FirstOrDefault(a => a.Id == item.Id);
                    if (item.Delete == 1)
                    {
                        if (item.Saved == 1)
                        {
                            updateCustomField.IsDeleted = true;
                            deleteCustomFields.Add(updateCustomField);
                        }
                    }
                    else if (item.Update == 1)
                    {
                        updateCustomField.Name = item.Name;
                        updateCustomField.SetValue(item.CustomFieldType, item.Value);
                        updateCustomField.LastModificationTime = _dateTime.Now;
                        updateCustomField.LastModifierUserId = input.CreatorGuidId;
                        updateCustomField.CompanyName = input.CompanyName;
                        updateCustomFields.Add(updateCustomField);
                    }
                }
                else
                {
                    var @createDetail = CustomField.Create(item.Name, item.CustomFieldType, @entity.Id, item.Value, item.GetStringValue(), item.GetDateValue(), item.GetIntValue(), item.GetDoubleValue(), input.CreatorGuidId, _dateTime.Now, input.CompanyName);
                    newCustomFields.Add(@createDetail);
                }
            }

            foreach (var item in input.DetailAssetToolKitsDto)
            {
                if (listAssetToolKits.Exists(a => a.Code.Equals(item.Code)))
                {
                    ToolAsset updateToolKit = listAssetToolKits.FirstOrDefault(a => a.Id == item.Id);
                    if (item.Delete == 1)
                    {
                        if (item.Saved == 1)
                        {
                            updateToolKit.IsDeleted = true;
                            deleteToolKits.Add(updateToolKit);
                        }
                    }
                    else if (item.Update == 1)
                    {
                        updateToolKit.Name = item.Name;
                        updateToolKit.Quatity = item.Quatity;
                        updateToolKit.LastModificationTime = _dateTime.Now;
                        updateToolKit.LastModifierUserId = input.CreatorGuidId;
                        updateToolKit.CompanyName = input.CompanyName;
                        updateToolKits.Add(updateToolKit);
                    }
                }
                else
                {
                    var entityTool = ToolAsset.Create(item.Name, item.Name, item.Code, @entity.Id, input.CreatorGuidId, _dateTime.Now, item.Quatity, input.CompanyName);
                    newToolKits.Add(entityTool);
                }
            }

            if (HasFile(input.Image))
            {
                string folderPath = "~/AssetsImages/";
                string fileName = input.Image.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../AssetsImages/";
                input.ImagePath = folderPathRelative + fileName;
                input.Image.SaveAs(mappedPath);

                @entity.SetImage(input.ImagePath);
            }
            _assetManager.Update(@entity, newCustomFields, deleteCustomFields, updateCustomFields, newToolKits, deleteToolKits, updateToolKits);
        }

        public void Create(CreateAssetInput input)
        {
            var @entityAsset = Asset.Create(input.Name, input.Description, input.Code, input.Barcode, input.AdmissionDate, _dateTime.Now, input.Price, input.DepreciationMonthsQty, input.AssetType, input.CategoryId, input.CreatorGuidId, _dateTime.Now, input.Brand, input.ModelStr, input.Plate, input.Series, input.IsAToolInKit, input.CompanyName);

            if (@entityAsset == null)
            {
                throw new UserFriendlyException("No se pudo crear el artículo.");
            }

            if (_assetManager.AssetExist(@entityAsset.Code, input.Id, input.CompanyName))
            {
                throw new UserFriendlyException("Existe un Artículo con el mismo Código.");
            }

            if (input.CustomFieldsDto != null && input.CustomFieldsDto.Any())
            {
                foreach (var cf in input.CustomFieldsDto)
                {
                    var entityCf = CustomField.Create(cf.Name, cf.CustomFieldType, @entityAsset.Id, cf.Value, cf.GetStringValue(), cf.GetDateValue(), cf.GetIntValue(), cf.GetDoubleValue(), input.CreatorGuidId, _dateTime.Now, input.CompanyName);
                    @entityAsset.CustomFields.Add(entityCf);
                }
            }

            if (input.DetailAssetToolKitsDto != null && input.DetailAssetToolKitsDto.Any())
            {
                foreach (var tool in input.DetailAssetToolKitsDto)
                {
                    var entityTool = ToolAsset.Create(tool.Name, tool.Name, tool.Code, @entityAsset.Id, input.CreatorGuidId, _dateTime.Now, tool.Quatity, input.CompanyName);
                    @entityAsset.ToolAssets.Add(entityTool);
                }
            }

            if (HasFile(input.Image))
            {
                string folderPath = "~/AssetsImages/";
                string fileName = input.Image.FileName;//input.Id + ".png"

                var mappedPath = HostingEnvironment.MapPath(folderPath) + fileName;
                var directory = new DirectoryInfo(HostingEnvironment.MapPath(folderPath));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                string folderPathRelative = "../../AssetsImages/";
                input.ImagePath = folderPathRelative + fileName;
                input.Image.SaveAs(mappedPath);

                @entityAsset.SetImage(input.ImagePath);
            }
            _assetManager.Create(@entityAsset);
        }

        private static bool HasFile(HttpPostedFile file)
        {
            return file != null && file.ContentLength > 0;
        }

        public void Delete(Guid id, Guid userId)
        {
            var @entity = _assetRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }
            @entity.IsDeleted = true;
            @entity.DeleterUserId = userId;
            @entity.DeletionTime = _dateTime.Now;
            _assetRepository.Update(@entity);
        }


        public IPagedList<Asset> SearchAssetToolKits(string query, int? page, string company)
        {
            string q = "";
            if (query != null)
                q = query.ToLower();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int maxResultCount = 100;
            var @assets = _assetRepository.GetAll().Where(a => a.IsAToolInKit == false && a.CompanyName.Equals(company) && a.Name.ToLower().Contains(q) || a.Name.ToLower().Equals(q) ||
                a.Code.ToLower().Contains(q) || a.Code.ToLower().Equals(q))
                .Include(a => a.Category).OrderByDescending(p => p.Code); ;
            return @assets.ToPagedList(currentPageIndex, maxResultCount);
        }
        public void UpdateInOutRequest(Guid id,string request,string type)
        {

            var @entity = _assetRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el artículo, fue borrado o no existe.");
            }
            if (type == "out")
            {
                @entity.DateUltOutRequest = DateTime.Now;
                @entity.UltOutRequest = request;
            }
            if(type == "in")
            {
                @entity.DateUltInRequest = DateTime.Now;
                @entity.UltInRequest = request;
            }
            _assetRepository.Update(@entity);
        }
    }
}
