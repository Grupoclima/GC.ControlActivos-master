using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.ReportMovements.Dto;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.ReportMovements
{
    class ReportMovementsAppService : ApplicationService, IReportMovementsAppService
    {
        private readonly IRepository<Movement, Guid> _movementsRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<Cellar, Guid> _cellarRepository;
        private readonly IDateTime _dateTime;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public ReportMovementsAppService(IRepository<Movement, Guid> movementsRepository, IRepository<User, Guid> userRepository, IRepository<Cellar, Guid> cellarRepository, IRepository<Project, Guid> projectRepository, IDateTime dateTime)
        {
            _projectRepository = projectRepository;
            _movementsRepository = movementsRepository;
            _userRepository = userRepository;
            _cellarRepository = cellarRepository;
            _dateTime = dateTime;
        }

        public IList<Cellar> GetAllCellars(string company)
        {
            var @entities = _cellarRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public IList<Project> GetAllProjects(string company)
        {
            var @entities = _projectRepository.GetAllList(e => e.IsDeleted.Value == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public IList<User> GetAllUsers(string company)
        {
            var @entities = _userRepository.GetAllList(e => e.IsDeleted == false && e.CompanyName.Equals(company));
            return @entities;
        }

        public IEnumerable<Movement> SearchReportMovements(ReportMovementsInputDto searchInput)
        {
            var query = "";
            if (searchInput.Query != null)
                query = searchInput.Query.ToLower();

            var movementsList = _movementsRepository.GetAll();
            movementsList = movementsList.Where(a => a.IsDeleted == false && 
            a.Asset.Name.ToLower().Contains(query) || a.Asset.Code.ToLower().Equals(query) || a.Asset.Code.ToLower().Contains(query) || a.Asset.Code.ToLower().Equals(query));

            if (searchInput.CellarId != null)
                movementsList = movementsList.Where(a => a.CellarId == searchInput.CellarId.Value);

            if (searchInput.ProjectId != null)
                movementsList = movementsList.Where(a => a.ProjectId == searchInput.ProjectId.Value);

            if (searchInput.UserId != null)
                movementsList = movementsList.Where(a => a.UserId == searchInput.UserId.Value);

            if (searchInput.BeginDateTime != null)
                movementsList = movementsList.Where(a => a.ApplicationDateTime >= searchInput.BeginDateTime.Value);

            if (searchInput.EndDateTime != null)
                movementsList = movementsList.Where(a => a.ApplicationDateTime <= searchInput.EndDateTime.Value);

            return movementsList.Include(a => a.Asset).Include(a => a.Cellar).Include(a => a.User).Include(a => a.Project).OrderBy(a => a.ApplicationDateTime).ToList();
        }
    }
}
