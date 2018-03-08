using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.InRequest.Dto
{
   [AutoMapFrom(typeof(InRequest))]
    public class GetInRequestOutput: EntityDto<Guid>, IDtoViewBaseFields
    {

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
       public string CompanyName { get; set; }
    }
}
