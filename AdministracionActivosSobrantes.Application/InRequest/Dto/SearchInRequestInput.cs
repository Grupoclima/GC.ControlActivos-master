using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;
using MvcPaging;
using AdministracionActivosSobrantes.Stocks;

namespace AdministracionActivosSobrantes.InRequest.Dto
{
    public class SearchInRequestInput : IDtoViewBaseFields
    {
        public const int DefaultPageSize = 20;

        public int MaxResultCount { get; set; }

        public int? RequestNumber { get; set; }

        public Guid? CellarId { get; set; }

        public Guid? ProjectId { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        public int? Page { get; set; }// Para la Paginación

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }

        public IPagedList<InRequestDto> Entities { get; set; }

        [DisplayName("Ubicación: ")]
        public IList<Cellar> Cellars { get; set; }


        public IList<Stock> Stocks { get; set; }

        [DisplayName("Proyecto: ")]
        public IList<Project> Projects { get; set; }

        [DisplayName("Proyecto: ")]
        public IList<User> Users { get; set; }

        public SearchInRequestInput()
        {
            MaxResultCount = DefaultPageSize;
        }

    }
}