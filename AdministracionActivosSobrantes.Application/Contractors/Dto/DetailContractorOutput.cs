using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Contractors.Dto
{
    [AutoMapFrom(typeof(Contractor))]
    public class DetailContractorOutput : EntityDto<Guid>, IDtoViewBaseFields
    {
        public string ContractorCode { get; set; }

        public string CompleteName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

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
