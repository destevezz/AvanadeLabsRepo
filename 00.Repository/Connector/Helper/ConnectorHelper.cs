// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectorHelper.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class Connector Helper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Connector.Helper
{
    using System;
    using System.Configuration;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Tooling.Connector;

    public class ConnectorHelper : IDisposable
    {
        private IOrganizationService _service;

        public void Dispose(IOrganizationService service)
        {
            _service = service;
        }
        public void Dispose()
        {
            if (_service != null)
            {
                ((OrganizationServiceProxy)_service).Dispose();
            }
        }

        /// <summary>
        /// Connect CRM online
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="urlCRMService"></param>
        /// <returns></returns>
        private IOrganizationService GetService(string userName, string password, string urlCRMService)
        {
            string conectionString = $"AuthType=OAuth;Username={userName};Password={password};Url={urlCRMService};AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;LoginPrompt=Auto";

            CrmServiceClient crmSvc = new CrmServiceClient(conectionString);
            if (crmSvc.IsReady)
            {
                IOrganizationService _orgService = (IOrganizationService)crmSvc.OrganizationWebProxyClient != null ? (IOrganizationService)crmSvc.OrganizationWebProxyClient : (IOrganizationService)crmSvc.OrganizationServiceProxy;
                return _orgService;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Recupera la cadena de conexión de la configuración del servicio de Azure para conectarse a DataVerse
        /// </summary>
        /// <returns></returns>
        public IOrganizationService GetServiceForAzureService()
        {
            ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;
            string user = settings["User"].ConnectionString;
            string password = settings["Password"].ConnectionString;
            string url = settings["Url"].ConnectionString;

            return GetService(user, password, url);
        }

        public IOrganizationService GetServiceRolDesarrollo()
        {
            string user = Properties.Settings.Default.UserDevTest;
            string password = Properties.Settings.Default.UserDevPassword;
            string url = Properties.Settings.Default.URLDev;

            return GetService(user, password, url);
        }

        public IOrganizationService GetServiceRolPre()
        {

            string user = Properties.Settings.Default.UserPreTest;
            string password = Properties.Settings.Default.UserPrePassword;
            string url = Properties.Settings.Default.URLPre;

            return GetService(user, password, url);
        }

        public IOrganizationService GetServiceRolUAT()
        {

            string user = Properties.Settings.Default.UserUatTest;
            string password = Properties.Settings.Default.UserUatPassword;
            string url = Properties.Settings.Default.URLUat;

            return GetService(user, password, url);
        }

        public IOrganizationService GetServiceRolPro()
        {
            string user = Properties.Settings.Default.UserProTest;
            string password = Properties.Settings.Default.UserProPassword;
            string url = Properties.Settings.Default.URLPro;

            return GetService(user, password, url);
        }
    }
}
