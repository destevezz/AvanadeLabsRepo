// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataFactoryException.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.BaseClasses
{
    using System;
    using Microsoft.Xrm.Sdk;

    internal class DataFactoryException : Exception
    {
        internal DataFactoryException(string message, ITracingService tracer)
        {
        }
    }
}
