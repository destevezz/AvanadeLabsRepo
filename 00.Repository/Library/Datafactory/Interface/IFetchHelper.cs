// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFetchHelper.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Interfaces
{
    public interface IFetchHelper
    {
        IFetch PRFetch { get; }
        IGeneral General { get; }

        /// <summary>
        /// return StrinBuilder with the query for FetchXML
        /// </summary>
        /// <returns>StringBuilder</returns>
        void CreateXmlActives();

        void CreateXML();
    }

}
