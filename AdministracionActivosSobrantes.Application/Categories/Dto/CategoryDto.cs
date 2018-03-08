﻿using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.Categories.Dto
{
    [AutoMapFrom(typeof(Category))]
    public class CategoryDto : EntityDto<Guid>, IDtoViewBaseFields
    {
        public string Name { get; set; }

        public string Description { get; set; }


        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
