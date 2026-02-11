// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicsProcess.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class Dynamics Process
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Library.Auxiliary.Constants.Process
{
    public class DynamicsProcess
    {
        #region Contexto

        public const string LocalSystem = "localContext";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";

        #endregion Contexto

        public class PluginStage
        {
            public const int PreValidation = 10;
            public const int PreOperation = 20;
            public const int MainOperation = 30;
            public const int PostOperation = 40;
            public const int PostOperationCrm4 = 50;
        }

        public class PluginMessage
        {
            public const string Assign = "Assign";
            public const string Associate = "Associate";
            public const string Create = "Create";
            public const string Delete = "Delete";
            public const string Disassociate = "Disassociate";
            public const string Execute = "Execute";
            public const string GrantAccess = "GrantAccess";
            public const string ModifyAccess = "ModifyAccess";
            public const string Retrieve = "Retrieve";
            public const string RetrieveMultiple = "RetrieveMultiple";
            public const string RetrievePrincipalAccess = "RetrievePrincipalAccess";
            public const string RetrieveSharedPrincipalsAndAccess = "RetrieveSharedPrincipalsAndAccess";
            public const string RevokeAccess = "RevokeAccess";
            public const string SetState = "SetState";
            public const string SetStateDynamicEntity = "SetStateDynamicEntity";
            public const string Update = "Update";
            public const string Send = "Send";
        }

    }
}
