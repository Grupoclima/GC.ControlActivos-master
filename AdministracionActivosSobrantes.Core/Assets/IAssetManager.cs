using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.CustomFields;
using AdministracionActivosSobrantes.ToolAssets;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Assets
{
    public interface IAssetManager:IDomainService
    {
        bool AssetExist(string code, Guid id, string company);
        IQueryable<Asset> GetEdit(Guid id, string company);
        IQueryable<CustomField> GetAssetCustomFields(Guid id, string company);
        IQueryable<ToolAsset> GetAssetToolAsset(Guid id, string company);
        User GetUser(Guid? id);
        IList<Category> GetCategoriesList(string company);
        IList<Cellar> GetCellarsList(string company);
        void Create(Asset asset);
        void Update(Asset asset, IList<CustomField> newDetails, IList<CustomField> removeDetails,
            IList<CustomField> updateDetails, IList<ToolAsset> newToolKit, IList<ToolAsset> removeToolKit,
            IList<ToolAsset> updateToolKit);
        void updateOutInRequest(Asset asset);
    }
}
