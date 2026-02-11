// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataFactory.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.Interfaces
{
    using Microsoft.Xrm.Sdk;
    using System;
    public interface IDataFactory
    {
        Guid Create(Entity _ent);
        EntityCollection Create(EntityCollection entityList);
        bool Update(Entity _ent);
        bool Delete(Entity _ent);
        OrganizationResponse Execute(OrganizationRequest request);
        EntityCollection RetrieveMultiple(ICustomQuery query);
        Entity Retrieve(ICustomQuery query);
    }
}