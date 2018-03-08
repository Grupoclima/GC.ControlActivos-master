using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Categories.Dto
{
    [AutoMapFrom(typeof(Category))]
    public class CreateCategoryInput : EntityDto<Guid>, IDtoViewBaseFields
    {
        [Display(Name = "Nombre: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Category.MaxNameLength, ErrorMessage = "* El Nombre no puede ser mayor a 256 caracteres.")]
        public string Name { get; set; }

        [Display(Name = "Descripción: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Category.MaxDescriptionLength, ErrorMessage = "* La Descripción no puede ser mayor a 1024 caracteres.")]
        public string Description { get; set; }

        public Guid CreatorGuidId { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
