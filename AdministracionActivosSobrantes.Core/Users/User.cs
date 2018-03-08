using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using AdministracionActivosSobrantes.Filters;
using AdministracionActivosSobrantes.Roles;

namespace AdministracionActivosSobrantes.Users
{
    public class User : AuditedEntity<Guid>, IFullAudited,ITenantCompanyName
    {
        public const int MaxUserNameLength = 64;
        public const int MaxNameLength = 256;
        public const int MaxPasswordLength = 128;
        public const int MaxCompleteNameLength = 128;
        public const int MaxEmailLength = 256;
        public const int MaxCodeLength = 64;
        public const int MaxPhoneLength = 48;
        public const int MaxCompanyName = 256;

        [Required]
        [StringLength(MaxCodeLength)]
        public string UserCode { get; set; }

        [Required]
        [StringLength(MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(MaxPasswordLength)]
        public string Password { get; set; }

        [Required]
        [StringLength(MaxCompleteNameLength)]
        public string CompleteName { get; set; }

        [StringLength(MaxEmailLength)]
        public string Email { get; set; }

        [StringLength(MaxPhoneLength)]
        public string Phone { get; set; }

        public UserRoles Rol { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        [StringLength(250)]
        public string CompanyName { get; set; }

       [StringLength(250)]
        public string Department { get; set; }
    }
    
    //[Serializable]
    //[Flags]
    public enum UserRoles
    {
        SuperAdministrador = 1,//64
        SupervisorUca = 2,//2
        Coordinador = 3,//4
        AuxiliarUca = 4,//8
        Solicitante = 5, //16
    }
}
