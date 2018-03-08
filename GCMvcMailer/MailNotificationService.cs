using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Abp.Application.Services;
using AdministracionActivosSobrantes.OutRequest;
using AdministracionActivosSobrantes.Users;

namespace GCMvcMailer
{
    public class MailNotificationService : ApplicationService, IMailNotificationService
    {
        public void OutRequestAprovalEmail(string requestNo,string cellarName,IList<User> coordinatorList, string warehouseMan, string warehouseManEmail, 
            string requestUserName, string approvalUserName, string requestUserEmail, string approvalUserEmail, string contractorEmail, string requestUrl, OutRequestStatus status, TypeOutRequest requestType, bool showUrl)
        {
            string emailBody = string.Empty;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates\NotificationMailTemplate.html");

            string your_id = ConfigurationManager.AppSettings["EmailId"];
            string your_password = ConfigurationManager.AppSettings["EmailPassword"];
            string port = ConfigurationManager.AppSettings["EmailPort"];
            string host = ConfigurationManager.AppSettings["EmailHost"];

            using (var sr = new StreamReader(path)) { emailBody = sr.ReadToEnd(); }

            string subjectMessage = string.Empty;
            if (status == OutRequestStatus.Draft)
            {
                subjectMessage = "La Solicitud de Salida fue guardada";
            }
            else if (status == OutRequestStatus.Active)
            {
                subjectMessage = "La Solicitud de Salida está activa";
            }
            else if (status == OutRequestStatus.WaitApproval)
            {
                subjectMessage = "La Solicitud de Salida está pendiente de aprobación";
            }
            else if (status == OutRequestStatus.Approved)
            {
                subjectMessage = "La Solicitud de Salida fue Aprobada";
            }
            else if (status == OutRequestStatus.ProcessedInWareHouse)
            {
                subjectMessage = "La Solicitud de Salida fue Entregada";
            }
            else if (status == OutRequestStatus.WaitAssetsReturn)
            {
                subjectMessage = "La Solicitud está en espera de la devolución de los activos";
            }
            else if (status == OutRequestStatus.PartialAssetsReturn)
            {
                subjectMessage = "La Solicitud está en espera de la devolución de los activos";
            }
            else if (status == OutRequestStatus.Confirmado)
            {
                subjectMessage = "La Solicitud de Salida fue Confirmada";
            }
            else if (status == OutRequestStatus.Disproved)
            {
                subjectMessage = "La Solicitud de Salida Desaprobada";
            }

            string param0 = subjectMessage;
            string param1 = requestUserName;
            string param2 = approvalUserName;
            string param3 = cellarName;
            string param4 = warehouseMan;
            string param5 = requestNo;
            string param6 = String.Empty;
            string param7 = requestNo;

            if (showUrl)
                param6 = requestUrl;

            string messageBody = string.Format(emailBody, param0, param1, param2, param3, param4,param5, param6, param7);

            SmtpClient client = new SmtpClient
            {
                Host = host,
                Port = Convert.ToInt32(port),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(your_id, your_password),
                Timeout = 10000,
            };
            MailMessage mm = new MailMessage(your_id, requestUserEmail, subjectMessage, messageBody);

            if (!string.IsNullOrEmpty(approvalUserEmail))
                mm.To.Add(approvalUserEmail);

            if (!string.IsNullOrEmpty(warehouseManEmail))
                mm.To.Add(warehouseManEmail);

            if (!string.IsNullOrEmpty(contractorEmail))
                mm.To.Add(contractorEmail);
            
            foreach (User user in coordinatorList)
            {
                if (!string.IsNullOrEmpty(user.Email))
                    mm.To.Add(user.Email);
            }

            mm.IsBodyHtml = true;
            ServicePointManager.ServerCertificateValidationCallback = 
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors){ return true; };// dviquez: para eliminar la petición de certificados

            client.Send(mm);
        }
    }
}
