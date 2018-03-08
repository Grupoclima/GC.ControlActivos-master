using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Contractors.Dto
{
    [AutoMapFrom(typeof(Contractor))]
    public class ContractorDto : EntityDto<Guid>, IDtoViewBaseFields
    {
        [StringLength(Contractor.MaxCodeLength)]
        public string ContractorCode { get; set; }

        [Required]
        [StringLength(Contractor.MaxCompleteNameLength)]
        public string CompleteName { get; set; }

        [StringLength(Contractor.MaxEmailLength)]
        public string Email { get; set; }

        [StringLength(Contractor.MaxPhoneLength)]
        public string Phone { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
