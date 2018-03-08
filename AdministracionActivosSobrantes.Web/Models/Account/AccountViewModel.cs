using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdministracionActivosSobrantes.Web.Models.Account
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "* Requerido.")]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "* Requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "* Requerido.")]
        [Display(Name = "Compañía")]
        public string CompanyName { get; set; }

        [Display(Name = "Compañías: ")]
        public IList<string> Companies { get; set; }

        [Display(Name = "Recuerdame ?")]
        public bool RememberMe { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

       public string Department { get; set; }
    }
}