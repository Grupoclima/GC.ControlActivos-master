using System;
using System.ComponentModel.DataAnnotations;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Web.Models.Assets
{
    public class AssetViewModel:BaseViewModel
    {
        public Guid AssetId { get; set; }

        [Display(Name = "Código: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Asset.MaxCodeLength, ErrorMessage = "* El Código no puede ser mayor a 256 caracteres.")]
        public string Code { get; set; }

        [Display(Name = "Nombre: ")]
        [Required(ErrorMessage = "* Requerido.")]
        [StringLength(Asset.MaxNameLength, ErrorMessage = "* El Nombre no puede ser mayor a 256 caracteres.")]
        public string Name { get; set; }

        [Display(Name = "Código de Barras: ")]
        [StringLength(Asset.MaxBarCodeLength, ErrorMessage = "* El Código de Barras no puede ser mayor a 256 caracteres.")]
        public string Barcode { get; set; }

        [Display(Name = "Descripción: ")]
        [StringLength(Asset.MaxDescriptionLength, ErrorMessage = "* La Descripción no puede ser mayor a 1024 caracteres.")]
        public string Description { get; set; }

        [Display(Name = "Modelo: ")]
        public DateTime? Model { get; set; }

        [Display(Name = "Fecha de Compra: ")]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Fecha de Ingreso: ")]
        public DateTime AdmissionDate { get; private set; }

        [Display(Name = "Depreciación Mensual: ")]
        [Required(ErrorMessage = "* Requerido.")]
        public double MonthDepreciation { get; private set; }

        [Display(Name = "Precio: ")]
        [Required(ErrorMessage = "* Requerido.")]
        public double Price { get; set; }

        [Display(Name = "Tipo: ")]
        [Required(ErrorMessage = "* Requerido.")]
        public AssetType AssetType { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        [Required(ErrorMessage = "* Requerido.")]
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public bool IsEdit { get; set; }

        public AssetViewModel()
        {
            Name = String.Empty;
        }
    }
}