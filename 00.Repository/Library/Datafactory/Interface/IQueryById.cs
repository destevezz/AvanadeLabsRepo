// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryById.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Interfaces
{
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public interface IQueryById
    {
        string EntityName { get;  }
        ColumnSet AttrList { get;  }
        Guid Id { get;  }
    }
}
