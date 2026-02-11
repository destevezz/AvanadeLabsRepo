// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityById.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class Entity By Id
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Queries
{
    using FactoryDataLayer.BaseClasses;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    internal class EntityById : CustomQuery
    {
        internal EntityById(string entityName, Guid id, ColumnSet attrList)
        {
            this.queryById = new QueryById(entityName, id, attrList);
        }
    }
}