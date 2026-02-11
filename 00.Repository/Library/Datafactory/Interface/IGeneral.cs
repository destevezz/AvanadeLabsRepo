// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGeneral.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Interfaces
{
    using Microsoft.Xrm.Sdk;
    using System.Text;

    public interface IGeneral
    {
        string LogicalNameE { get; set; }
        string OrderDirection { get; set; }
        string OrderField { get; set; }
        EntityCollection ResultadoEC { get; set; }
        int Top { get; set; }
        bool UseDistinct { get; set; }
        StringBuilder Xml { get; set; }
    }
}