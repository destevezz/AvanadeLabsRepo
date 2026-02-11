// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryById.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Interfaces
{
    public interface ICustomQuery
    {
        IFetchHelper Fetch { get; set; }

        IQueryById QueryById { get; set; }

        string StringFetch { get; set; }
    }
}
