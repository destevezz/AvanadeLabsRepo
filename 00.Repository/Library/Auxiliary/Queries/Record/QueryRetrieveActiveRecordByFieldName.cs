// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryRetrieveActiveRecordByFieldName.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class intelliSense of Query Retrieve active records By FieldName
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Library.Auxiliary.Queries.Record
{
    using FactoryDataLayer.BaseClasses;
    using Library.Auxiliary.Constants.Entities;
    using System.Collections.Generic;

    public class QueryRetrieveActiveRecordByFieldName : CustomQuery
    {
        public QueryRetrieveActiveRecordByFieldName(string name)
        {
            Fetch = new CustomFetch();
            Fetch.PRFetch.General.LogicalNameE = RecordE.CF;
            Fetch.PRFetch.Main.Attributes = new List<string>(new[] { RecordE.UserEmail, RecordE.Id });
            Fetch.PRFetch.Main.Conditions = new string[,]
            {
               { RecordE.Status, CustomFetch.Equal, RecordE.StatusEnum.Active.ToString()},
               { RecordE.FieldName, CustomFetch.Equal, name },
            };
            Fetch.CreateXML();
        }
    }
}
