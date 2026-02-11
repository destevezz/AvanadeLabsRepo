// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFetch.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Interfaces
{
    using System.Collections.Generic;

    public interface IFetch
    {
        IGeneral General { get; }
        IList<ILinkEntity> LinkEntity { get; set; }
        IMain Main { get; set; }
    }
}