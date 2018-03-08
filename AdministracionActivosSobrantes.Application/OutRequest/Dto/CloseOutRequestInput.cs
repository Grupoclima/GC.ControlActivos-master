using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdministracionActivosSobrantes.Common;

namespace AdministracionActivosSobrantes.OutRequest.Dto
{
    public class CloseOutRequestInput : IDtoViewBaseFields
    {
        public IList<DetailAssetCloseRequest> DetailsRequest { get; set; }

        [Required(ErrorMessage = "* Requerido.")]
        public Guid CellarId { get; set; }

        public Guid Id { get; set; }

        public int RequestNumber { get; set; }

        public string RequestDocumentNumber { get; set; }

        public string CellarName { get; set; }

        public string WarehouseManName { get; set; }

        public string AssetReturnDate { get; set; }

        public Guid CreatorUserId { get; set; }

        public string UrlAction { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
