// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILinkEntity.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Interfaces
{
    using System.Collections.Generic;

    public interface ILinkEntity
    {
        IList<string> AttributesRelations { get; set; }
        string[,] ConditionsRelations { get; set; }
        string[] CriteriosRelations { get; set; }
        string RelationType { get; set; }
    }
}