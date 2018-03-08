using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Contractors.Dto
{
    [AutoMapFrom(typeof(Contractor))]
    public class CreateContractorInput : EntityDto<Guid>, IDtoViewBaseFields
    {
        [StringLength(Contractor.MaxCodeLength)]
        [Required]
        [Display(Name = "Código: ")]
        public string ContractorCode { get; set; }

        [Required]
        [StringLength(Contractor.MaxCompleteNameLength)]
        [Display(Name = "Nombre: ")]
        public string CompleteName { get; set; }

        [StringLength(Contractor.MaxEmailLength)]
        [Display(Name = "Correo: ")]
        public string Email { get; set; }

        [StringLength(Contractor.MaxPhoneLength)]
        [Display(Name = "Teléfono: ")]
        public string Phone { get; set; }

        public Guid CreatorGuidId { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
