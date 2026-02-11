// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataVerseHelper.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class with auxiliary methos for use in Dynamics CRM back end.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Library.Helper.UtilityHelper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using Library.Auxiliary.Constants.Entities.Custom;

    internal class DataVerseHelper
    {
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracingService;

        public DataVerseHelper(IOrganizationService service)
        {
            _service = service;
        }

        public DataVerseHelper(IOrganizationService service, ITracingService tracingService)
        {
            _service = service;
            _tracingService = tracingService;
        }

        #region Entity relations

        public void AssociatedRelatiosNtoN(EntityReference relationstoCreateER, EntityReference relationRegisterER, StringBuilder relationshipName)
        {
            if (relationstoCreateER == null) return;

            List<EntityReference> relationstoCreate = new List<EntityReference> { relationstoCreateER };

            AssociatedRelatiosNtoN(relationstoCreate, relationRegisterER, relationshipName);

        }

        private void AssociatedRelatiosNtoN(List<EntityReference> relationstoCreate, EntityReference relationRegisterER, StringBuilder relationshipName)
        {
            if (!relationstoCreate.Any()) return;

            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();

            foreach (EntityReference relationtoCreateER in relationstoCreate)
            {
                relatedEntities.Add(relationtoCreateER);
            }

            Relationship relationship = new Relationship(relationshipName.ToString());

            _service.Associate(relationRegisterER.LogicalName, relationRegisterER.Id, relationship, relatedEntities);
        }

        public void DisAssociatedRelatiosNtoN(EntityReference relationstoCreateER, EntityReference relationRegisterER, StringBuilder relationshipName)
        {
            if (relationstoCreateER == null) return;

            List<EntityReference> relationstoCreate = new List<EntityReference> { relationstoCreateER };

            DisAssociatedRelatiosNtoN(relationstoCreate, relationRegisterER, relationshipName);
        }

        private void DisAssociatedRelatiosNtoN(List<EntityReference> relationstoCreate, EntityReference relationRegisterER, StringBuilder relationshipName)
        {
            if (!relationstoCreate.Any()) return;

            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();

            foreach (EntityReference relationtoCreateER in relationstoCreate)
            {
                relatedEntities.Add(relationtoCreateER);
            }

            Relationship relationship = new Relationship(relationshipName.ToString());

            _service.Disassociate(relationRegisterER.LogicalName, relationRegisterER.Id, relationship, relatedEntities);
        }

        #endregion Entity relations

        #region Share and assign records

        /// <summary>
        /// Gives read access to the user or team given as destUserOrTeam for the record given in record
        /// </summary>
        /// <param name="userOrTeam"></param>
        /// <param name="record"></param>
        public void ShareRecord(EntityReference userOrTeam, EntityReference record)
        {
            GrantAccessRequest grantAccessRequest = new GrantAccessRequest()
            {
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = AccessRights.ReadAccess,
                    Principal = userOrTeam
                },
                Target = record
            };

            _service.Execute(grantAccessRequest);
        }

        public void DoNotShareRecord(EntityReference userOrTeam, EntityReference record)
        {
            RevokeAccessRequest revokeAccessRequest = new RevokeAccessRequest()
            {
                Target = record,
                Revokee = userOrTeam
            };
            _service.Execute(revokeAccessRequest);
        }

        /// <summary>
        /// Gives read access to the user or team in given as destUserOrTeam for the records given in records
        /// </summary>
        /// <param name="userOrTeam"></param>
        /// <param name="records"></param>
        public void ShareRecord(EntityReference userOrTeam, IEnumerable<Entity> records)
        {
            foreach (Entity recordE in records)
            {
                ShareRecord(userOrTeam, recordE.ToEntityReference());
            }
        }

        /// <summary>
        /// Gives read, write, append to and append access to the user or team given as destUserOrTeam for the record given in record
        /// </summary>
        /// <param name="userOrTeam"></param>
        /// <param name="record"></param>
        public void ShareRecordWithMorePrivileges(EntityReference userOrTeam, IEnumerable<Entity> records)
        {
            foreach (Entity recordE in records)
            {
                ShareRecordWithMorePrivileges(userOrTeam, recordE.ToEntityReference());
            }
        }
      
        private void ShareRecordWithMorePrivileges(EntityReference userOrTeam, EntityReference record)
        {
            GrantAccessRequest grantAccessRequest = new GrantAccessRequest()
            {
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess,
                    Principal = userOrTeam
                },
                Target = record
            };

            _service.Execute(grantAccessRequest);
        }

        public void GivenRecordAndTeamThenShareWithReadAndWritePrivileges(EntityReference userOrTeam, EntityReference record)
        {
            GrantAccessRequest grantAccessRequest = new GrantAccessRequest()
            {
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess,
                    Principal = userOrTeam
                },
                Target = record
            };

            _service.Execute(grantAccessRequest);
        }

        /// <summary>
        /// Gives read, write, append to and append access to the user or team given as destUserOrTeam for the record given in record
        /// </summary>
        /// <param name="userOrTeam"></param>
        /// <param name="record"></param>
        public void ShareRecordWithMorePrivilegesAndDelete(EntityReference userOrTeam, EntityReference record)
        {
            GrantAccessRequest grantAccessRequest = new GrantAccessRequest()
            {
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess | AccessRights.DeleteAccess,
                    Principal = userOrTeam
                },
                Target = record
            };

            _service.Execute(grantAccessRequest);
        }

        /// <summary>
        /// Gives read, write, append to and append access to the user or team given as destUserOrTeam for the record given in record
        /// </summary>
        /// <param name="userOrTeam"></param>
        /// <param name="record"></param>
        public void ShareRecordWithMorePrivilegesNoWrite(EntityReference userOrTeam, EntityReference record)
        {
            GrantAccessRequest grantAccessRequest = new GrantAccessRequest()
            {
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = AccessRights.ReadAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess,
                    Principal = userOrTeam
                },
                Target = record
            };

            _service.Execute(grantAccessRequest);
        }

        public void AssingOwner(EntityReference recordER, EntityReference ownerTeam)
        {
            AssignRequest assignWorkorderOwner = new AssignRequest
            {
                Assignee = ownerTeam,
                Target = recordER
            };
            _service.Execute(assignWorkorderOwner);
        }

        #endregion Share and assign records

        #region OptionSets

        public OptionSetMetadata RetrieveGlobalOptionSetMetadata(string globalOptionSetName)
        {
            RetrieveOptionSetRequest retrieveOptionSetRequest =
                new RetrieveOptionSetRequest
                {
                    Name = globalOptionSetName
                };
            // Execute the request.
            RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)_service.Execute(retrieveOptionSetRequest);

            OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

            return retrievedOptionSetMetadata;
        }

        /// <summary>
        /// Retrieve all metadata values in array
        /// </summary>
        /// <param name="entityE"></param>
        /// <param name="optionSetFieldLogicalName"></param>
        /// <returns>OptionSet metadata in array</returns>
        public OptionMetadata[] RetrieveOptionSetValueMetadataCollection(Entity entityE, string optionSetFieldLogicalName)
        {
            RetrieveAttributeRequest request = new RetrieveAttributeRequest();
            request.EntityLogicalName = entityE.LogicalName;
            request.LogicalName = optionSetFieldLogicalName;
            request.RetrieveAsIfPublished = true;
            RetrieveAttributeResponse response = (RetrieveAttributeResponse)_service.Execute(request);
            EnumAttributeMetadata metadata = (EnumAttributeMetadata)response.AttributeMetadata;

            OptionMetadata[] optionSetMetadata = metadata.OptionSet.Options.Where(x => x.Value != null).ToArray();

            return optionSetMetadata;
        }

        /// <summary>
        /// Create new Option Set value 
        /// </summary>
        /// <param name="optionSetSchemaName"> Local or global Option Set schema name </param>
        /// <param name="newOptionSetLabel"> New label to insert in the Option Set </param>
        /// <returns>New OptionSet value (metadata value)</returns>
        public int CreateNewOptionSetValue(string optionSetSchemaName, string newOptionSetLabel)
        {
            int _languageCode = 3082;

            InsertOptionValueRequest insertOptionValueRequest =
            new InsertOptionValueRequest
            {
                OptionSetName = optionSetSchemaName,
                Label = new Label(newOptionSetLabel, _languageCode)
            };

            return ((InsertOptionValueResponse)_service.Execute(insertOptionValueRequest)).NewOptionValue;
        }

        /// <summary>
        /// Update Option Set value (new label)
        /// </summary>
        /// <param name="optionSetSchemaName"> Local or global Option Set schema name </param>
        /// <param name="optionSetValue"> Option Set value to change </param>
        /// <param name="newLabel"> New label to insert in the Option Set </param>
        public void UpdateOptionSetValueLabel(string optionSetSchemaName, int optionSetValue, string newLabel)
        {
            int _languageCode = 3082;

            UpdateOptionValueRequest updateOptionValueRequest =
            new UpdateOptionValueRequest
            {
                OptionSetName = optionSetSchemaName,
                // Update the second option value.
                Value = optionSetValue,
                Label = new Label(newLabel, _languageCode)
            };
            _service.Execute(updateOptionValueRequest);
        }

        /// <summary>
        /// Delete single Option Set value
        /// </summary>
        /// <param name="optionSetSchemaName"> Local or global Option Set schema name </param>
        /// <param name="optionSetValue"> Value to delete </param>
        public void DeleteOptionSetValue(string optionSetSchemaName, int optionSetValue)
        {
            DeleteOptionValueRequest deleteOptionValueRequest =
            new DeleteOptionValueRequest
            {
                OptionSetName = optionSetSchemaName,
                Value = optionSetValue
            };

            _service.Execute(deleteOptionValueRequest);
        }


        #endregion OptionSets

        #region Activate/Deactivate record

        /// <summary>
        /// The activate record.
        /// </summary>
        /// <param name="logicalName">The entity name.</param>
        /// <param name="recordId">The record id.</param>
        /// <param name="organizationService">The organization service.</param>
        public void ActivateRecord(EntityReference recordToActivateER)
        {
            ColumnSet cols = new ColumnSet(new[] { "statecode", "statuscode" });

            Entity entity = _service.Retrieve(recordToActivateER.LogicalName, recordToActivateER.Id, cols);

            if (entity != null && entity.GetAttributeValue<OptionSetValue>("statecode").Value == (int)StandardAttributesE.StatusEnum.Inactive)
            {
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = recordToActivateER.Id,
                        LogicalName = recordToActivateER.LogicalName,
                    },
                    State = new OptionSetValue((int)StandardAttributesE.StatusEnum.Active),
                    Status = new OptionSetValue((int)StandardAttributesE.StatusReasonEnum.Active)
                };

                _service.Execute(setStateRequest);
            }
        }

        /// <summary>
        /// Deactivates the record.
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <param name="organizationService">The organization service.</param>
        public void DeactivateRecord(EntityReference recordToDeactivateER)
        {
            ColumnSet cols = new ColumnSet(new[] { "statecode", "statuscode" });

            Entity entity = _service.Retrieve(recordToDeactivateER.LogicalName, recordToDeactivateER.Id, cols);

            if (entity != null && entity.GetAttributeValue<OptionSetValue>("statecode").Value == (int)StandardAttributesE.StatusEnum.Active)
            {
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = recordToDeactivateER.Id,
                        LogicalName = recordToDeactivateER.LogicalName,
                    },
                    State = new OptionSetValue((int)StandardAttributesE.StatusEnum.Inactive),
                    Status = new OptionSetValue((int)StandardAttributesE.StatusReasonEnum.Inactive)
                };

                _service.Execute(setStateRequest);
            }
        }


        #endregion Activate/Deactivate record
    }
}
