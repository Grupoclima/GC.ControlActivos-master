using System;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.Categories
{
    public class CategoryManager : DomainService, ICategoryManager
    {
        private readonly IRepository<Category, Guid> _categoryRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IDateTime _dateTime;

        public CategoryManager(IRepository<Category, Guid> categoryRepository, IRepository<User, Guid> userRepository, IDateTime dateTime)
        {
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _dateTime = dateTime;
        }

        public bool CategoryExist(string name, Guid id,string company)
        {
            var @entity = _categoryRepository.FirstOrDefault(e => e.Name.Equals(name) && e.CompanyName.Equals(company) && e.Id != id && e.IsDeleted.Value == false);
            return @entity != null;
        }

        public Category GetCategory(string name,string company)
        {
            var @entity = _categoryRepository.FirstOrDefault(e => e.Name.Equals(name) && e.CompanyName.Equals(company) );
            return @entity;
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
