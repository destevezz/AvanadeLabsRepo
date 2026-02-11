// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardAttributesE.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class intelliSense of standard attributes 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Library.Auxiliary.Constants.Entities.Custom
{
    public partial class StandardAttributesE
    {
        #region Standars

        public const string ImportSequenceNumber = "importsequencenumber";
        public const string Owner = "ownerid";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";

        public const string CreatedBy = "createdby";
        public const string CreatedOn = "createdon";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string ModifiedByDelegate = "modifiedonbehalfby";

        public const string RecordCreatedOn = "overriddencreatedon";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string OwningTeam = "owningteam";
        public const string OwningUser = "owninguser";
        public const int LanguageCodeEsp = 3082;
        public const int LanguageCodeCat = 1027;
        public const int LanguageCodeEng = 1033;

        #endregion
    }
    public partial class StandardAttributesE
    {
        #region enumValues

        public enum StatusReasonEnum
        {
            Active = 1,
            Inactive = 2
        }

        public enum StatusEnum
        {
            Active = 0,
            Inactive = 1
        }

        #endregion enumValues
    }
}
