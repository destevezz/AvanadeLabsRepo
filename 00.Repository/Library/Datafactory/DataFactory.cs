// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataFactory.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer
{
    using FactoryDataLayer.BaseClasses;
    using FactoryDataLayer.Interfaces;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public enum BulkOperations
    {
        Create = 0
    }
    public class DataFactory : IDataFactory
    {
        #region variables
        private IOrganizationService service;
        private ITracingService tracer;
        #endregion

        #region propiedades
        private IOrganizationService Service { get => (service == null) ? throw new DataFactoryException("Object null reference in Service", tracer) : service; set => service = value; }
        #endregion

        #region constructor
        public DataFactory(IOrganizationService _service, ITracingService tracer)
        {
            this.Service = _service;
            this.tracer = tracer;
        }
        #endregion

        #region métodos
        public Guid Create(Entity _ent)
        {
            return Service.Create(_ent);
        }
        public EntityCollection Create(EntityCollection entityList)
        {
            EntityCollection createdIds = new EntityCollection();

            foreach(Entity ent in entityList.Entities)
            {
                ent.Id = Create(ent);
                createdIds.Entities.Add(ent);
            }

            return createdIds;
        }
        public bool Update(Entity _ent)
        {
            Service.Update(_ent);

            return true;
        }
        public bool Delete(Entity _ent)
        {
            Service.Delete(_ent.LogicalName, _ent.Id);

            return true;
        }
        public OrganizationResponse Execute(OrganizationRequest request)
        {
          return Service.Execute(request);
        }
        public EntityCollection RetrieveMultiple(ICustomQuery query)
        {
            string xml = string.Empty;

            if (query.StringFetch.Equals(string.Empty))
            {
                xml = query.Fetch.PRFetch.General.Xml.ToString();
            } else
            {
                xml = query.StringFetch;
            }

            RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(xml)
            };
            return ((RetrieveMultipleResponse)Service.Execute(fetchRequest)).EntityCollection;
        }
        public Entity Retrieve(ICustomQuery query)
        {
            return Service.Retrieve(query.QueryById.EntityName, query.QueryById.Id, query.QueryById.AttrList);
        }
        #endregion
    }
}
