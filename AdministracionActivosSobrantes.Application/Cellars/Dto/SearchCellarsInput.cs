﻿using System;
using AdministracionActivosSobrantes.Common;
using MvcPaging;

namespace AdministracionActivosSobrantes.Cellars.Dto
{
    public class SearchCellarsInput: IDtoViewBaseFields
    {
        public Guid? UserId { get; set; }

        public const int DefaultPageSize = 20;

        public int? Page { get; set; }

        public int MaxResultCount { get; set; }

        public IPagedList<CellarDto> Entities { get; set; }

        public SearchCellarsInput()
        {
            MaxResultCount = DefaultPageSize;
        }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
