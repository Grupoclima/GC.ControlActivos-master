using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Projects.Dto
{
    [AutoMapFrom(typeof(Project))]
    public class ProjectDto : EntityDto<Guid>, IDtoViewBaseFields
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public string CostCenter { get; set; }

        public EstadoProyecto EstadoProyecto { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinalDate { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
