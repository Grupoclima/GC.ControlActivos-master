using System;
using Abp.Domain.Repositories;
using Abp.Domain.Services;

namespace AdministracionActivosSobrantes.CustomFields
{
    public class CustomFieldsManager : DomainService, ICustomFieldManager
    {
        private readonly IRepository<CustomField, Guid> _customFieldRepository;

        public CustomFieldsManager(IRepository<CustomField, Guid> customFieldRepository)
        {
            _customFieldRepository = customFieldRepository;
        }
    }
}
