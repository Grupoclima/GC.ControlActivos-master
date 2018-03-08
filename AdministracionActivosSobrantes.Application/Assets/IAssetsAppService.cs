using System;
using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AdministracionActivosSobrantes.Assets.Dto;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.CustomFields.Dto;
using MvcPaging;

namespace AdministracionActivosSobrantes.Assets
{
    public interface IAssetsAppService : IApplicationService
    {

        IPagedList<AssetDto> SearchAssets(SearchAssetsInput searchInput);
        void Update(UpdateAssetInput input);
        void Create(CreateAssetInput input);
        void Delete(Guid id, Guid userId);
        GetAssetOutput Get(Guid input);
        DetailAssetOutput GetDetail(Guid id, string company);
        UpdateAssetInput GetEdit(Guid id, string company);
        IList<CustomFieldDto> GetEditCustomFields(Guid assetId, string company);
        IList<DetailAssetToolKitsDto> GetEditToolKits(Guid assetId, string company);
        IList<Category> GetAllCategories(string company);
        IList<Cellar> GetAllCellars(string company);
        IPagedList<Asset> SearchAssetToolKits(string query, int? page, string company);
        void UpdateInOutRequest(Guid id, string request, string type);
    }
}
