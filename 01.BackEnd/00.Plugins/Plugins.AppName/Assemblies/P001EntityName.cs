// --------------------------------------------------------------------------------------------------------------------
// <copyright file="P001EntityName.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Plugin for EntityName entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugins.AppName.Assemblies
{
    using System;
    using C = Library.Auxiliary.Constants;
    using Microsoft.Xrm.Sdk;
    using FactoryDataLayer.Interfaces;
    using Plugins.AppName.Handler;
    using FactoryDataLayer;

    internal class P001EntityName : Plugin
    {
        public P001EntityName()
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(C.Process.DynamicsProcess.PluginStage.PostOperation, C.Process.DynamicsProcess.PluginMessage.Update, C.Entities.RecordE.CF, PostOperationCreate));
        }

        protected void PostOperationCreate(LocalPluginContext localContext)
        {
            localContext.TracingService.Trace("PostOperationCreate of EntityName");

            #region Validations

            if (localContext == null)
            {
                throw new ArgumentNullException(C.Process.DynamicsProcess.LocalSystem);
            }

            if (!this.HasTarget(localContext))
            {
                localContext.TracingService.Trace("Doesn't have target");
                return;
            }

            #endregion Validations

            #region Input parameters

            Entity recordE = (Entity)localContext.PluginExecutionContext.InputParameters["Target"];
            Entity preImageRecordE = (Entity)localContext.PluginExecutionContext.PreEntityImages["preImage"];
            Entity postImageRecordE = (Entity)localContext.PluginExecutionContext.PreEntityImages["postImage"];

            #endregion  Input parameters

            IDataFactory df = new DataFactory(localContext.OrganizationService, localContext.TracingService);

            new P001EntityNameHandler(df, localContext.TracingService).PostOperationCreate(preImageRecordE, recordE, postImageRecordE);
        }

    }
}
