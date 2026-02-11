// --------------------------------------------------------------------------------------------------------------------
// <copyright file="P001EntityNameHandler.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Plugin for EntityName entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugins.AppName.Handler
{
    using FactoryDataLayer.Interfaces;
    using Microsoft.Xrm.Sdk;

    public class P001EntityNameHandler
    {
        #region Private Attributes

        private ITracingService _tracingService;
        private IDataFactory _df;

        #endregion Private Attributes

        public P001EntityNameHandler(IDataFactory df, ITracingService tracingService)
        {
            _df = df;
            _tracingService = tracingService;
        }

        public void PostOperationCreate(Entity preImageRecordE, Entity recordE, Entity postImageRecordE)
        {
            // Salta el trigger, tengo el contexto y he validado. Se que estoy en fase(postOperation)-mensaje(create) de la tabla "X" -> Aplico lógica de negocio para mi proceso

        }

    }
}
