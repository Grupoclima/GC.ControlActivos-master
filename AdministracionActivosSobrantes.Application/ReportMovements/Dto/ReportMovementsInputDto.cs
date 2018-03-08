using System;
using System.Collections.Generic;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Movements;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.Users;

namespace AdministracionActivosSobrantes.ReportMovements.Dto
{
    public class ReportMovementsInputDto : IDtoViewBaseFields
    {
        public Guid? CellarId { get; set; }

        public Guid? AssetId { get; set; }

        public Guid? UserId { get; set; }

        public Guid? ProjectId { get; set; }

        public string ProjectName { get; set; }

        //public Guid? InRequestId { get; set; }

        //public Guid? OutRequestId { get; set; }// ver si se la juegan con el query

        //public Guid? AdjustmentId { get; set; }

        public IEnumerable<Project> Projects { get; set; }

        public IEnumerable<Cellar> Cellars { get; set; }

        public IEnumerable<User> Users { get; set; }

        public IEnumerable<Movement> Movements { get; set; }

        public DateTime? BeginDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string CurrentUserName { get; set; }

        public DateTime CurrentDateTime { get; set; }
        
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string CompanyName { get; set; }
    }
}
