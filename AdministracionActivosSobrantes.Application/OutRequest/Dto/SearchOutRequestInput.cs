using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;
using MvcPaging;

namespace AdministracionActivosSobrantes.OutRequest.Dto
{
    public class SearchOutRequestInput : IDtoViewBaseFields
    {
        public const int DefaultPageSize = 20;

        public int MaxResultCount { get; set; }

        public int? RequestNumber { get; set; }

        public Guid? CellarId { get; set; }

        public Guid? ProjectId { get; set; }

        public int? Status { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        public DateTime? DateSearch { get; set; }

        public DateTime? DateSearch2 { get; set; }

        public int? Page { get; set; }// Para la Paginación

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
        public string Department { get; set; }

        public IPagedList<OutRequestDto> Entities { get; set; }

        [DisplayName("Ubicación: ")]
        public IList<Cellar> Cellars { get; set; }
        
        [DisplayName("Proyecto: ")]
        public IList<Project> Projects { get; set; }

        [DisplayName("Proyecto: ")]
        public IList<User> Users { get; set; }

        public SearchOutRequestInput()
        {
            MaxResultCount = DefaultPageSize;
        }

    }
}