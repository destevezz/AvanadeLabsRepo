// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMain.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Interfaces
{
    using System.Collections.Generic;

    public interface IMain
    {
        IList<string> Attributes { get; set; }
        string[,] Conditions { get; set; }
    }
}