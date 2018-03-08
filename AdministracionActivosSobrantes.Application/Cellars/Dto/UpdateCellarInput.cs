using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Cellars.Dto
{
    [AutoMapFrom(typeof(Cellar))]
    public class UpdateCellarInput: EntityDto<Guid>, IDtoViewBaseFields
    {
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        [Display(Name = "Dirección: ")]
        public string Address { get; set; }

        [Display(Name = "Teléfono: ")]
        public string Phone { get; set; }

        [Display(Name = "Centro de Costo: ")]
        public string CostCenter { get; set; }

        [Display(Name = "Encargado de Ubicación: ")]
        public Guid WareHouseManagerId { get; set; }
        public User WareHouseManager { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        [Display(Name = "Encargado de Ubicación: ")]
        public IList<User> WareHouseUsers { get; set; }

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
