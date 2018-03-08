using System;

namespace AdministracionActivosSobrantes.Common
{
    public interface IFullAuditedCustom
    {
        Guid? DeleterUserId { get; set; }

        bool? IsDeleted { get; set; }

        DateTime? DeletionTime { get; set; }

        DateTime? LastModificationTime { get; set; }

        Guid? LastModifierUserId { get; set; }

        DateTime CreationTime { get; set; }

        Guid CreatorUserId { get; set; }
    }
}
