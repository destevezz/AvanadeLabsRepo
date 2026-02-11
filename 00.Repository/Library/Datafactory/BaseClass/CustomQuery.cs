// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomQuery.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.BaseClasses
{
    using FactoryDataLayer.Interfaces;

    public abstract class CustomQuery : ICustomQuery
    {
        private IFetchHelper fetch;
        internal IQueryById queryById;
        private string strFetch= string.Empty;

        public IFetchHelper Fetch { get => fetch; set => fetch = value; }
        public IQueryById QueryById { get => queryById; set => queryById = value; }

        public string StringFetch { get => strFetch; set => strFetch = value; }

        public IQueryById GetQueryById()
        {
            return queryById;
        }
    }
}
