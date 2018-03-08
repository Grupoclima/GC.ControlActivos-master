using Abp.Domain.Services;

namespace AdministracionActivosSobrantes.ToolAssets
{
    public interface IToolAssetManager:IDomainService
    {
        ToolAsset GetAsset(int id);
    }
}
