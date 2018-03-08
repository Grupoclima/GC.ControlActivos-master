using System;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Projects
{
    public class ProjectManager : DomainService, IProjectManager
    {
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IDateTime _dateTime;

        public ProjectManager(IRepository<Project, Guid> projectRepository, IRepository<User, Guid> userRepository, IDateTime dateTime)
        {

            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _dateTime = dateTime;
        }

        public bool ProjectExist(string name, Guid id, string company)
        {
            var @entity = _projectRepository.FirstOrDefault(e => e.Name.Equals(name) && e.CompanyName.Equals(company) && e.Id != id && e.IsDeleted.Value ==false);
            return @entity != null;
        }

        public void Create(Project entity)
        {
            _projectRepository.Insert(entity);
        }

        public User GetUser(Guid? id)
        {
            if (id == null)
                return null;

            Guid idTemp = id.Value;
            var @entity = _userRepository.Get(idTemp);
            return @entity;
        }
    }
}
