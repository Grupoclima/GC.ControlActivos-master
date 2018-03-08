using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Projects.Dto
{
    [AutoMapFrom(typeof(Project))]
    public class UpdateProjectInput : EntityDto<Guid>, IDtoViewBaseFields
    {
        [Display(Name = "Nombre: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Project.MaxNameLength, ErrorMessage = "* El Nombre no puede ser mayor a 256 caracteres.")]
        public string Name { get; set; }

        [Display(Name = "Descripción: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Project.MaxDescriptionLength, ErrorMessage = "* La Descripción no puede ser mayor a 1024 caracteres.")]
        public string Description { get; set; }

        [Display(Name = "Código: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Project.MaxDescriptionLength, ErrorMessage = "* El Código no puede ser mayor a 256 caracteres.")]
        public string Code { get; set; }

        [Display(Name = "Centro de Costo: ")]
        public string CostCenter { get; set; }

        [Display(Name = "Estado del Proyecto: ")]
        public EstadoProyecto EstadoProyecto { get; set; }

        [Display(Name = "Fecha Inicial: ")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Fecha Final: ")]
        public DateTime? FinalDate { get; set; }

        public Guid? LastModifierUserId { get; set; }
        public User LastModifierUser { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
