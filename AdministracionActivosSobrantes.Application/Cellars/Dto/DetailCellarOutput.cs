using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Cellars.Dto
{
    [AutoMapFrom(typeof(Cellar))]
    public class DetailCellarOutput : EntityDto<Guid>, IDtoViewBaseFields
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string CostCenter { get; set; }

        public string Phone { get; set; }

        public Guid WareHouseManagerId { get; set; }

        public User WareHouseManagerd { get; set; }

        public virtual string Latitude { get; set; }

        public virtual string Longitude { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }
        public User LastModifierUser { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }
        public User CreatorUser { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
