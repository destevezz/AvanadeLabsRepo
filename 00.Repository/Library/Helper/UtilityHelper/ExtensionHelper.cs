// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionHelper.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class Extension Helper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Library
{
    using Microsoft.Xrm.Sdk;

    public static class ExtensionHelper
    {
        public static bool ExistField(this Entity e, string f)
        {
            return e!= null && e.Contains(f) && e[f] != null;
        }
    }
}
