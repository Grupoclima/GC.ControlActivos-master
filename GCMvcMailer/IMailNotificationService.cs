using System.Collections.Generic;
using Abp.Application.Services;
using AdministracionActivosSobrantes.OutRequest;
using AdministracionActivosSobrantes.Users;

namespace GCMvcMailer
{
    public interface IMailNotificationService : IApplicationService
    {
        void OutRequestAprovalEmail(string requestNo, string cellarName, IList<User> coordinatorList, string warehouseMan, string warehouseManEmail,
            string requestUserName, string approvalUserName, string requestUserEmail, string approvalUserEmail,
            string contractorEmail, string requestUrl, OutRequestStatus status, TypeOutRequest requestType, bool showUrl);
    }
}
