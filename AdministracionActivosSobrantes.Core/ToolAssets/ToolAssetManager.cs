using System;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Assets;

namespace AdministracionActivosSobrantes.ToolAssets
{
    public class ToolAssetManager :DomainService, IToolAssetManager
    {
        public ToolAsset GetAsset(int id)
        {
            throw new NotImplementedException();
        }
    }
}
