// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomFetch.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class to crate dynamic query from Dynamics Enviroment using FetchXml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryDataLayer.BaseClasses
{
    using FactoryDataLayer.Interfaces;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    internal partial class CustomFetch
    {
        public IFetch PRFetch { get; set; }
        public class General : IGeneral
        {
            public StringBuilder Xml { get; set; }
            public EntityCollection ResultadoEC { get; set; }
            public string LogicalNameE { get; set; }
            public string OrderDirection { get; set; }
            public string OrderField { get; set; }
            public bool UseDistinct { get; set; }
            public int Top { get; set; }

            public General()
            {
                Top = 0;
            }
        }

        public class Main : IMain
        {
            public IList<string> Attributes { get; set; }
            public string[,] Conditions { get; set; }

        }

        public class LinkEntity : ILinkEntity
        {
            public string[] CriteriosRelations { get; set; }
            public IList<string> AttributesRelations { get; set; }
            public string[,] ConditionsRelations { get; set; }
            public string RelationType { get; set; }
            public LinkEntity()
            {
                CriteriosRelations = new string[] { };
                AttributesRelations = new List<string>();
                ConditionsRelations = new string[,] { };
            }
        }

        public class Fetch : IFetch
        {
            private General general = new General();
            private Main main = new Main();

            public IGeneral General { get => general; set => general = (General) value; }

            public IList<ILinkEntity> LinkEntity { get; set; } = new List<ILinkEntity>();
            public IMain Main { get => main; set => main = (Main)value; }

        }
    }

    internal partial class CustomFetch : IFetchHelper
    {
        internal CustomFetch()
        {
            Initialize();
        }
        public void Initialize()
        {
            Fetch fetch = new Fetch()
            {
                General = new General()
                {
                    Xml = new StringBuilder(),
                    ResultadoEC = new EntityCollection(),
                    LogicalNameE = string.Empty,
                    OrderField = string.Empty,
                    OrderDirection = string.Empty,
                    UseDistinct = false
                },
                Main = new Main()
                {
                    Attributes = new List<string>(),
                    Conditions = new string[,] { },

                },
                LinkEntity = new List<ILinkEntity>()
            };

            PRFetch = fetch;
        }

        public List<string> Atributes { get; set; }

        IFetch IFetchHelper.PRFetch => PRFetch;

        IGeneral IFetchHelper.General => this.PRFetch.General;

        /// <summary>
        /// return StrinBuilder with the query for FetchXML
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="atributes"></param>
        /// <returns>StringBuilder</returns>
        public void CreateXmlActives()
        {
            //  CreateFetch(entityName, atributes, new[,] { { "statecode", equal, "0" } });
        }

        public void CreateXML()
        {

            PRFetch.General.Xml = new StringBuilder();
            string strTop = string.Empty;

            if (PRFetch.General.Top > 0)
                strTop = $"top='{PRFetch.General.Top}'";
            
            PRFetch.General.Xml.AppendFormat("<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='true' distinct='{1}' {2}><entity name='{0}' >", PRFetch.General.LogicalNameE, PRFetch.General.UseDistinct, strTop);

            AddAtributes(PRFetch.Main.Attributes);

            if (!string.IsNullOrEmpty(PRFetch.General.OrderField))
            {
                // Ordenacion dentro del fetch existente mediante un Switch
                switch (PRFetch.General.OrderDirection)
                {
                    case Ascending:
                        PRFetch.General.Xml.AppendFormat("<order attribute='{0}' descending='false' />", PRFetch.General.OrderField);
                        break;
                    case Descending:
                        PRFetch.General.Xml.AppendFormat("<order attribute='{0}' descending='true' />", PRFetch.General.OrderField);
                        break;
                    default:
                        PRFetch.General.Xml.AppendFormat("<order attribute='{0}' descending='false' />", PRFetch.General.OrderField);
                        break;
                }
            }

            ConditionsFetch(PRFetch.Main.Conditions);

            AddLinkEntities(PRFetch.LinkEntity);


            PRFetch.General.Xml.AppendFormat("</entity></fetch>");

        }

        /// <summary>
        /// Añade a la consulta los datos relacionados con una entidad relacionada.
        /// </summary>
        /// <param name="criteriosRelations"></param>
        /// <param name="atributesRelations"></param>
        /// <param name="conditionsRelations"></param>
        private void AddLinkEntities(IList<ILinkEntity> lista)
        {
            foreach (ILinkEntity li in lista)
            {
                if (li.CriteriosRelations.Length <= 0) return;
                if (li.RelationType != "" && li.RelationType != null)
                {
                    li.RelationType = $"link-type='{li.RelationType}'";
                }
                PRFetch.General.Xml.AppendFormat("<link-entity name='{0}' from='{1}' to='{2}' {3} alias='{4}' >",
                    li.CriteriosRelations[0], li.CriteriosRelations[1], li.CriteriosRelations[2], li.RelationType, li.CriteriosRelations[3]);

                AddAtributes(li.AttributesRelations);

                ConditionsFetch(li.ConditionsRelations);

                PRFetch.General.Xml.AppendFormat("{0}", "</link-entity>");
            }
        }

        /// <summary>
        /// Añade los campos a obtener en la consulta.
        /// </summary>
        private void AddAtributes(IEnumerable<string> attributes)
        {
            foreach (string atributo in attributes.Where(atributo => atributo != string.Empty))
            {
                PRFetch.General.Xml.AppendFormat("<attribute name='{0}' />", atributo);
            }
        }

        /// <summary>
        /// Ejecuta una Fetch con paginación, obteniendo únicamente la página indicada en el parémtro con el número de registros
        /// del parámetro.
        /// </summary>
        /// <param name="service">Organization Service Proxy de CRM</param>
        /// <param name="pageNumber"></param>
        /// <param name="fetchXml">Fetch a ejecutar</param>
        /// <param name="recordsPerPage">Número de registros por página</param>
        /// <returns></returns>
        public void 
            FetchOnePage(
            IOrganizationService service,
            int pageNumber,
            string fetchXml,
            int recordsPerPage)
        {
            // Specify the current paging cookie. For retrieving the first page, 
            // pagingCookie should be null.
            string pagingCookie = null;

            // Build fetchXml string with the placeholders.
            string xml = CreateXml(fetchXml, pagingCookie, pageNumber, recordsPerPage);

            // Excute the fetch query and get the xml result.
            RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest { Query = new FetchExpression(xml) };

            EntityCollection currentPageCollection =
                ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;
            PRFetch.General.ResultadoEC.Entities.AddRange(currentPageCollection.Entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qe"></param>
        /// <param name="service"></param>
        /// <param name="recordsPerPage"></param>
        /// <returns></returns>
        public void ExecuteQueryExpressionPagging(QueryExpression qe, IOrganizationService service, int recordsPerPage)
        {
            int pageNumber = 1;
            RetrieveMultipleResponse multiResponse = new RetrieveMultipleResponse();

            do
            {
                qe.PageInfo.Count = recordsPerPage;
                qe.PageInfo.PagingCookie = (pageNumber == 1) ? null : multiResponse.EntityCollection.PagingCookie;
                qe.PageInfo.PageNumber = pageNumber++;

                RetrieveMultipleRequest multiRequest = new RetrieveMultipleRequest { Query = qe };
                multiResponse = (RetrieveMultipleResponse)service.Execute(multiRequest);

                PRFetch.General.ResultadoEC.Entities.AddRange(multiResponse.EntityCollection.Entities);
            }
            while (multiResponse.EntityCollection.MoreRecords);

        }


        /// <summary>
        /// Retrieves the multiple all.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="query">The query.</param>
        /// <param name="maxRecords">The maximum records.</param>
        /// <returns></returns>
        public IEnumerable<Entity> RetrieveMultipleAll(string fetchXml, IOrganizationService organizationService, int maxRecords = int.MaxValue)
        {
            int pageNumber = 1;
            string pagingCookie = string.Empty;
            // int pageSize = 5000;
            List<Entity> resultList = new List<Entity>();

            RetrieveMultipleResponse returnCollection = null;
            do
            {
                // Build fetchXml string with the placeholders.
                string xml = CreateXml(fetchXml, pagingCookie, pageNumber, maxRecords);

                // Excute the fetch query and get the xml result.
                RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest
                {
                    Query = new FetchExpression(xml)
                };

                returnCollection = ((RetrieveMultipleResponse)organizationService.Execute(fetchRequest1));

                resultList.AddRange(returnCollection.EntityCollection.Entities);

                if (!returnCollection.EntityCollection.MoreRecords) continue;

                pageNumber++;
                pagingCookie = returnCollection.EntityCollection.PagingCookie;
            }
            while (returnCollection.EntityCollection.MoreRecords && resultList.Count < maxRecords);

            return resultList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="fetchXml"></param>
        /// <param name="recordsPerPage"></param>
        /// <returns></returns>
        public void ExecuteFetchPaging(
            IOrganizationService service,
            string fetchXml,
            int recordsPerPage)
        {
            // Initialize the page number.
            int pageNumber = 1;
            // Specify the current paging cookie. For retrieving the first page, 
            // pagingCookie should be null.
            string pagingCookie = null;

            while (true)
            {
                // Build fetchXml string with the placeholders.
                string xml = CreateXml(fetchXml, pagingCookie, pageNumber, recordsPerPage);

                // Excute the fetch query and get the xml result.
                RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest { Query = new FetchExpression(xml) };

                EntityCollection currentPageCollection =
                    ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;
                PRFetch.General.ResultadoEC.Entities.AddRange(currentPageCollection.Entities);
                // Check for morerecords, if it returns 1.
                if (currentPageCollection.MoreRecords)
                {
                    // Increment the page number to retrieve the next page.
                    pageNumber++;
                    pagingCookie = currentPageCollection.PagingCookie;
                }
                else
                {
                    // If no more records in the result nodes, exit the loop.
                    break;
                }
            }

        }

        private void ConditionsFetch(string[,] conditions)
        {
            bool putendfilter = false;

            if (conditions.Length <= 0) return;

            for (int i = 0; i < conditions.GetLongLength(0); i++)
            {
                if (conditions[i, 0] == string.Empty) continue;

                if (i == 0) { PRFetch.General.Xml.AppendFormat("<filter>"); putendfilter = true; }

                switch (conditions[i, 1])
                {
                    case Null:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='null' />", conditions[i, 0]);
                        break;
                    case NotNull:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='not-null' />", conditions[i, 0]);
                        break;
                    case "":
                        //case string.Empty:
                        // No añadidomos fila.
                        break;
                    case Equal:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='eq' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case NotEqual:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='neq' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case HigherOrEqual:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='ge' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case LessOrEqual:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='le' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case Higher:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='gt' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case Less:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='it' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case LastXDays:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='last-x-days' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case StartWith:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='like' value='{1}%' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case NoStartWith:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='not-like' value='{1}%' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case LastXHours:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='last-x-hours' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case "on":
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='on' value='{1}' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    case Contains:
                        PRFetch.General.Xml.AppendFormat("<condition attribute='{0}' operator='like' value='%{1}%' />", conditions[i, 0], conditions[i, 2]);
                        break;
                    default:
                        throw new InvalidDataException("Consulta con condiciones erroneas");
                }
            }
            if (putendfilter)
            {
                PRFetch.General.Xml.AppendFormat("</filter >");
            }
        }

        /// <summary>
        /// The create xml.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        /// <summary>
        /// The create xml.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

        #region Constants Own FetchUtility
        public const string LastXDays = "lxd";
        public const string Higher = ">";
        public const string HigherOrEqual = ">=";
        public const string Less = "<";
        public const string LessOrEqual = "<=";
        public const string Equal = "=";
        public const string NotEqual = "!=";
        public const string StartWith = "like";
        public const string NoStartWith = "not-like";
        public const string LastXHours = "lxh";
        public const string Contains = "contains";
        public const string Null = "null";
        public const string NotNull = "notnull";

        public const string Ascending = "false";
        public const string Descending = "true";

        public const string Active = "0";
        public const string Inactive = "1";

        #endregion Constants Own FetchUtility

    }
}
