namespace AdministracionActivosSobrantes.Web.Models
{
    public abstract class BaseViewModel
    {
        public int ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string Action { get; set; }

        public string Control { get; set; }

        public string Query { get; set; }
    }
}