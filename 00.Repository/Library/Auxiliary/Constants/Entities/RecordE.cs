// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordE.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class intelliSense of entity record 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Library.Auxiliary.Constants.Entities
{
    using Library.Auxiliary.Constants.Entities.Custom;

    public partial class RecordE : CustomRecordE
    {
        #region Standard Attributes
        public const string Id = "publisher_fieldname";
        public const string CF = "publisher_tablename";

        public const string FieldName = "systemfieldname";
        #endregion Standard Attributes

        #region BusinessValues
        public const string BvMyRecordId = "ed3552e0-16e3-ec11-bb3c-000d3ade6919";
        #endregion BusinessValues
    }

    public partial class RecordE
    {
        #region Local system enumvalues
        public enum EnumExample
        {
            Example1 = 1,
            Example2 = 2,
            Example3 = 3

        }
        #endregion Local system enumvalues
    }
}
