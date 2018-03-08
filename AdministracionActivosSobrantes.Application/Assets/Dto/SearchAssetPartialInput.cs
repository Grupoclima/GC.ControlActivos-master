using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcPaging;

namespace AdministracionActivosSobrantes.Assets.Dto
{
    public class SearchAssetPartialInput
    {
        public Guid? UserId { get; set; }

        public const int DefaultPageSize = 20;

        public int? Page { get; set; }

        public int TotalItem { get; set; }

        public int MaxResultCount { get; set; }

        public string TypeInRequestValue { get; set; }

        public IPagedList<Asset> Entities { get; set; }

        public SearchAssetPartialInput()
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
