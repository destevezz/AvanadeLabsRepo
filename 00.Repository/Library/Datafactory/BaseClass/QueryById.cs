// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryById.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.BaseClasses
{
    using FactoryDataLayer.Interfaces;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class QueryById : IQueryById
    {
        public Guid id;
        public string entityName = string.Empty;
        public ColumnSet attrList = null;

        public string EntityName { get => entityName; }
        public ColumnSet AttrList { get => attrList; }
        public Guid Id { get => id; }

        public QueryById(string entityName, Guid id, ColumnSet attrList)
        {
            this.entityName = entityName;
            this.id = id;
            this.attrList = attrList;
        }
    }
}
