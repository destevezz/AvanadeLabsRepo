// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Plugin class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugins.AppName
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;

    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Base class for all Plugins.
    /// </summary>    
    public class Plugin : IPlugin
    {
        public Guid ServiceEndpointId { get; set; }

        public class LocalPluginContext
        {
            internal IServiceProvider ServiceProvider { get; private set; }

            internal IOrganizationService OrganizationService { get; private set; }

            internal IPluginExecutionContext PluginExecutionContext { get; private set; }

            internal ITracingService TracingService { get; private set; }

            public Guid ServiceEndpointId { get; set; }

            public LocalPluginContext(string config)
            {
                Guid guid = Guid.Empty;
                if (String.IsNullOrEmpty(config) || !Guid.TryParse(config, out guid))
                {
                    throw new InvalidPluginExecutionException("Service endpoint ID should be passed as config.");
                }

                this.ServiceEndpointId = guid;
            }

            public LocalPluginContext(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider");
                }

                // Obtain the execution context service from the service provider.
                this.PluginExecutionContext =
                    (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the tracing service from the service provider.
                this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the Organization Service factory service from the service provider
                IOrganizationServiceFactory factory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                this.ServiceProvider = serviceProvider;

                // Use the factory to generate the Organization Service.
                this.OrganizationService = factory.CreateOrganizationService(this.PluginExecutionContext.UserId);
            }

            internal void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || this.TracingService == null)
                {
                    return;
                }

                if (this.PluginExecutionContext == null)
                {
                    this.TracingService.Trace(message);
                }
                else
                {
                    this.TracingService.Trace(
                        "{0}, Correlation Id: {1}, Initiating User: {2}",
                        message,
                        this.PluginExecutionContext.CorrelationId,
                        this.PluginExecutionContext.InitiatingUserId);
                }
            }
        }

        private Collection<Tuple<int, string, string, Action<LocalPluginContext>>> registeredEvents;

        /// <summary>
        /// Gets the List of events that the plug-in should fire for. Each List
        /// Item is a <see cref="System.Tuple"/> containing the Pipeline Stage, Message and (optionally) the Primary Entity. 
        /// In addition, the fourth parameter provide the delegate to invoke on a matching registration.
        /// </summary>
        protected Collection<Tuple<int, string, string, Action<LocalPluginContext>>> RegisteredEvents
        {
            get
            {
                if (this.registeredEvents == null)
                {
                    this.registeredEvents = new Collection<Tuple<int, string, string, Action<LocalPluginContext>>>();
                }

                return this.registeredEvents;
            }
        }

        /// <summary>
        /// Gets or sets the name of the child class.
        /// </summary>
        /// <value>The name of the child class.</value>
        protected string ChildClassName { get; private set; }

        public Plugin()
        {

        }

        public Plugin(string unsecure)
        {
            Guid guid = Guid.Empty;
            if (string.IsNullOrEmpty(unsecure) || !Guid.TryParse(unsecure, out guid))
            {
                throw new InvalidPluginExecutionException("Service endpoint ID should be passed as config.");
            }

            this.ServiceEndpointId = guid;
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="Plugin"/> class.
        ///// </summary>
        ///// <param name="childClassName">The <see cref=" cred="Type"/> of the derived class.</param>
        //internal Plugin(Type childClassName)

        //{
        //    this.ChildClassName = childClassName.ToString();
        //}
        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // Construct the Local plug-in context.
            LocalPluginContext localcontext = new LocalPluginContext(serviceProvider);
            //localcontext.ServiceEndpointId = this.ServiceEndpointId;

            localcontext.Trace(
                string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.ChildClassName));

            try
            {
                // Iterate over all of the expected registered events to ensure that the plugin
                // has been invoked by an expected event
                // For any given plug-in event at an instance in time, we would expect at most 1 result to match.
                Action<LocalPluginContext> entityAction = (from a in this.RegisteredEvents
                                                           where
                                                               (a.Item1 == localcontext.PluginExecutionContext.Stage
                                                                && a.Item2
                                                                == localcontext.PluginExecutionContext.MessageName
                                                                && (string.IsNullOrWhiteSpace(a.Item3)
                                                                    || a.Item3
                                                                    == localcontext.PluginExecutionContext
                                                                           .PrimaryEntityName))
                                                           select a.Item4).FirstOrDefault();

                if (entityAction != null)
                {
                    localcontext.Trace(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0} is firing for Entity: {1}, Message: {2}, Stage {3}",
                            this.ChildClassName,
                            localcontext.PluginExecutionContext.PrimaryEntityName,
                            localcontext.PluginExecutionContext.MessageName,
                            localcontext.PluginExecutionContext.Stage));

                    entityAction.Invoke(localcontext);

                    // now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                    // guard against multiple executions.
                    return;
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()));

                // Handle the exception.
                throw;
            }
            finally
            {
                localcontext.Trace(
                    string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.ChildClassName));
            }
        }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="columnSet">The column set.</param>
        /// <returns></returns>
        protected IEnumerable<Entity> GetEntities(
            IOrganizationService organizationService,
            string logicalName,
            ColumnSet columnSet)
        {
            if (string.IsNullOrWhiteSpace(logicalName))
            {
                return new Entity[0];
            }

            QueryExpression query = new QueryExpression(logicalName);
            query.NoLock = true;
            query.ColumnSet = columnSet;

            EntityCollection retrieve = organizationService.RetrieveMultiple(query);

            if (retrieve.Entities.Count > 0)
            {
                return retrieve.Entities;
            }

            return new Entity[0];
        }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="fieldname">The fieldname.</param>
        /// <param name="value">The value.</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="columnSet">The column set.</param>
        /// <returns></returns>
        protected IEnumerable<Entity> GetEntities(
            IOrganizationService organizationService,
            string logicalName,
            string fieldname,
            string value,
            ConditionOperator conditionOperator,
            ColumnSet columnSet)
        {
            if (string.IsNullOrWhiteSpace(logicalName) || string.IsNullOrWhiteSpace(fieldname)
                || string.IsNullOrWhiteSpace(value))
            {
                return new Entity[0];
            }

            QueryExpression query = new QueryExpression(logicalName);
            query.NoLock = true;
            query.ColumnSet = columnSet;

            ConditionExpression condition = new ConditionExpression();
            condition.AttributeName = fieldname;
            condition.Operator = conditionOperator;
            condition.Values.Add(value);

            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(condition);
            filter.FilterOperator = LogicalOperator.And;
            query.Criteria.AddCondition(condition);

            //try
            //{
            EntityCollection retrieve = organizationService.RetrieveMultiple(query);

            if (retrieve.Entities.Count > 0)
            {
                return retrieve.Entities;
            }
            //}
            //catch
            //{

            //}

            return new Entity[0];
        }

        protected IEnumerable<Entity> GetEntities(
            IOrganizationService organizationService,
            string logicalName,
            string fieldname,
            string value,
            ConditionOperator conditionOperator)
        {
            return this.GetEntities(organizationService, logicalName, fieldname, value, conditionOperator, new ColumnSet(true));
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="fieldname">The fieldname.</param>
        /// <param name="value">The value.</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="columnSet">The column set.</param>
        /// <returns></returns>
        protected Entity GetEntity(
            IOrganizationService organizationService,
            string logicalName,
            string fieldname,
            string value,
            ConditionOperator conditionOperator,
            ColumnSet columnSet)
        {
            IEnumerable<Entity> entities = this.GetEntities(organizationService, logicalName, fieldname, value, conditionOperator, columnSet);
            return entities.FirstOrDefault();
        }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="conditions">The conditions.</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <returns></returns>
        protected IEnumerable<Entity> GetEntities(
            IOrganizationService organizationService,
            string logicalName,
            IEnumerable<KeyValuePair<string, string>> conditions,
            ConditionOperator conditionOperator)
        {
            return this.GetEntities(organizationService, logicalName, conditions, conditionOperator, new ColumnSet(true));
        }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="conditions">The conditions.</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="columnSet">The column set.</param>
        /// <returns></returns>
        protected IEnumerable<Entity> GetEntities(IOrganizationService organizationService, string logicalName, IEnumerable<KeyValuePair<string, string>> conditions, ConditionOperator conditionOperator, ColumnSet columnSet)
        {
            if (string.IsNullOrWhiteSpace(logicalName) || conditions == null || !conditions.Any())
            {
                return new Entity[0];
            }

            QueryExpression query = new QueryExpression(logicalName);
            query.NoLock = true;
            query.ColumnSet = columnSet;

            foreach (KeyValuePair<string, string> field in conditions)
            {
                ConditionExpression condition = new ConditionExpression();
                condition.AttributeName = field.Key;
                condition.Operator = conditionOperator;
                condition.Values.Add(field.Value);

                FilterExpression filter = new FilterExpression();
                filter.Conditions.Add(condition);
                filter.FilterOperator = LogicalOperator.And;
                query.Criteria.AddCondition(condition);
            }

            EntityCollection retrieve = organizationService.RetrieveMultiple(query);

            if (retrieve.Entities.Count > 0)
            {
                return retrieve.Entities;
            }

            return new Entity[0];
        }

        /// <summary>
        /// Assigns the user to record.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="userId">The user identifier.</param>
        protected void AssignUserToRecord(
            IOrganizationService organizationService,
            string logicalName,
            Guid entityId,
            Guid userId)
        {
            // Create the Request Object and Set the Request Object's Properties
            AssignRequest assign = new AssignRequest
            {
                Assignee = new EntityReference("systemuser", userId),
                Target = new EntityReference(logicalName, entityId)
            };

            organizationService.Execute(assign);
        }

        /// <summary>
        /// Determines whether the specified local context has target.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected bool HasTarget(LocalPluginContext localContext)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context =
                (IPluginExecutionContext)localContext.ServiceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.
            return context.InputParameters.Contains("Target");
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected Entity GetEntity(LocalPluginContext localContext)
        {
            if (!this.HasTarget(localContext))
            {
                return null;
            }

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context =
                (IPluginExecutionContext)localContext.ServiceProvider.GetService(typeof(IPluginExecutionContext));
            return context.InputParameters["Target"] as Entity;
        }

        /// <summary>
        /// Gets the entity reference.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected EntityReference GetEntityReference(LocalPluginContext localContext)
        {
            if (!this.HasTarget(localContext))
            {
                return null;
            }

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context =
                (IPluginExecutionContext)localContext.ServiceProvider.GetService(typeof(IPluginExecutionContext));
            return context.InputParameters["Target"] as EntityReference;
        }

        /// <summary>
        /// Gets the organization service.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected IOrganizationService GetOrganizationService(LocalPluginContext localContext)
        {
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)
                localContext.ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
            return serviceFactory.CreateOrganizationService(localContext.PluginExecutionContext.UserId);
        }

        /// <summary>
        /// Gets the organization service as admin.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected IOrganizationService GetOrganizationServiceAsAdmin(LocalPluginContext localContext)
        {
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)
                localContext.ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
            return serviceFactory.CreateOrganizationService(null);
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected Entity GetPreImage(LocalPluginContext localContext)
        {
            return localContext.PluginExecutionContext.PreEntityImages["preImage"];
        }


        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected Entity GetPostImage(LocalPluginContext localContext)
        {
            return localContext.PluginExecutionContext.PostEntityImages["postImage"];
        }

        /// <summary>
        /// Deactivates the record.
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <param name="organizationService">The organization service.</param>
        public void DeactivateRecord(string logicalName, Guid recordId, IOrganizationService organizationService)
        {
            ColumnSet cols = new ColumnSet(new[] { "statecode", "statuscode" });

            // Check if it is Active or not
            Entity entity = organizationService.Retrieve(logicalName, recordId, cols);

            if (entity != null && entity.GetAttributeValue<OptionSetValue>("statecode").Value == 0)
            {
                // StateCode = 1 and StatusCode = 2 for deactivating Account or Contact
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = recordId,
                        LogicalName = logicalName,
                    },
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(2)
                };

                organizationService.Execute(setStateRequest);
            }
        }

        /// <summary>
        /// The activate record.
        /// </summary>
        /// <param name="logicalName">The entity name.</param>
        /// <param name="recordId">The record id.</param>
        /// <param name="organizationService">The organization service.</param>
        public void ActivateRecord(string logicalName, Guid recordId, IOrganizationService organizationService)
        {
            ColumnSet cols = new ColumnSet(new[] { "statecode", "statuscode" });

            // Check if it is Inactive or not
            Entity entity = organizationService.Retrieve(logicalName, recordId, cols);

            if (entity != null && entity.GetAttributeValue<OptionSetValue>("statecode").Value == 1)
            {
                // StateCode = 0 and StatusCode = 1 for activating Account or Contact
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = recordId,
                        LogicalName = logicalName,
                    },
                    State = new OptionSetValue(0),
                    Status = new OptionSetValue(1)
                };

                organizationService.Execute(setStateRequest);
            }
        }

        /// <summary>
        /// Determines whether [is assign message] [the specified local context].
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected bool IsAssignMessage(LocalPluginContext localContext)
        {
            // Avoid to trigger on Assign message
            IPluginExecutionContext parentContext = localContext.PluginExecutionContext.ParentContext.ParentContext;
            return (parentContext != null && string.Equals(parentContext.MessageName, "Assign", StringComparison.OrdinalIgnoreCase)) ||
                string.Equals(localContext.PluginExecutionContext.MessageName, "Assign", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether [is delete message] [the specified local context].
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <returns></returns>
        protected bool IsDeleteMessage(LocalPluginContext localContext)
        {
            // Avoid to trigger on Assign message
            IPluginExecutionContext parentContext = localContext.PluginExecutionContext.ParentContext.ParentContext;
            return parentContext != null && string.Equals(parentContext.MessageName, "Delete", StringComparison.OrdinalIgnoreCase);
        }
    }
}
