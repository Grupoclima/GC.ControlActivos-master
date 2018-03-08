namespace AdministracionActivosSobrantes.Common
{
    public interface IDtoViewBaseFields
    {
        int? ErrorCode { get; set; }

        string ErrorDescription { get; set; }

        string Action { get; set; }

        string Control { get; set; }

        string Query { get; set; }

        string CompanyName { get; set; }
    }
}
