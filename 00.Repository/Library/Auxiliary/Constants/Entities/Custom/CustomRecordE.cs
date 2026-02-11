// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomRecordE.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class intelliSense of record
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Library.Auxiliary.Constants.Entities.Custom
{
    public partial class CustomRecordE : StandardAttributesE
    {
        #region Custom Attributes
        public const string Title = "publisher_fieldname";
        public const string Description = "publisher_fieldname";
        public const string UserEmail = "publisher_fieldname";
        public const string Longitude = "publisher_fieldname";
        public const string Latitude = "publisher_fieldname";
        public const string ExampleOptionSet = "publisher_fieldname";
        #endregion Custom Attributes
    }
    public partial class CustomRecordE
    {
        #region Local enumValues

        public enum CustomEnum
        {
            CustomValue1 = 0,
            CustomValue2 = 1
        }

        #endregion Local enumValues
    }
}
