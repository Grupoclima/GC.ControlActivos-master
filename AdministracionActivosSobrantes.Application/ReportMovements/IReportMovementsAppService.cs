using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.ReportMovements.Dto;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.ReportMovements
{
    public interface IReportMovementsAppService : IApplicationService
    {
        IEnumerable<Movement> SearchReportMovements(ReportMovementsInputDto searchInput);
        IList<Cellar> GetAllCellars(string company);
        IList<Project> GetAllProjects(string company);
        IList<User> GetAllUsers(string company);
    }
}
