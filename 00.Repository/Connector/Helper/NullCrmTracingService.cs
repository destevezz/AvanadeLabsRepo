// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullCrmTracingService.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class NullCrmTracingService
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Connector.Helper
{
    using System.Diagnostics;
    using Microsoft.Xrm.Sdk;

    public class NullCrmTracingService : ITracingService
    {
        public void Trace(string format, params object[] args)
        {
            Debug.WriteLine(format);
        }
    }
}
