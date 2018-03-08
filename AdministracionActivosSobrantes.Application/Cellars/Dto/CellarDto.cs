using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Cellars.Dto
{
    [AutoMapFrom(typeof(Cellar))]
    public class CellarDto : EntityDto<Guid>, IDtoViewBaseFields
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public bool Active { get; set; }

        public string CostCenter { get; set; }

        public Guid WareHouseManagerId { get; set; }

        public virtual string Latitude { get; set; }

        public virtual string Longitude { get; set; }


        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
