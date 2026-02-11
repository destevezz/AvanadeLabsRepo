"use strict";

var MainEntityName = { __namespace: true };

MainEntityName.LogicalName = "publisher_entityname";

MainEntityName.FormEvents = {

    OnLoad: function (ec) {

        let fc = ec.getFormContext(); // Usar siempre let en lugar de var

        MainEntityName.Functions.BusinessLogic.DoSomething(fc);
        MainEntityName.Functions.BusinessLogic.DoSomethingIfIsNotCreationForm(fc);
    },

    OnChange: {

        publisher_fieldname1: function (ec) {

            let fc = ec.getFormContext();

            MainEntityName.Functions.BusinessLogic.DoSomething(fc);
            MainEntityName.Functions.BusinessLogic.DoSomethingBasedOnValue(fc);
        },

        publisher_fieldname2: function (ec) {

            let fc = ec.getFormContext();

            MainEntityName.Functions.BusinessLogic.DoSomethingIfIsNotCreationForm(fc);
        },
    },

    OnSave: function (ec) {

        let fc = ec.getFormContext();

        MainEntityName.Functions.BusinessLogic.DoSomething(fc);
        MainEntityName.Functions.BusinessLogic.DoSomethingBasedOnValue(fc);
    },

    __namespace: true
}

MainEntityName.Functions = {

    BusinessLogic: {

        DoSomething: function (fc) {

            if (fc.getAttribute(MainEntityName.Constants.Fields.FieldName1).getValue()) {

                fc.getAttribute(MainEntityName.Constants.Fields.FieldName1).setValue(false);
            }

        },

        DoSomethingIfIsNotCreationForm: function (fc) {

            if (fc.ui.getFormType() !== MainEntityName.Constants.General.FormType.Create) {

                fc.getAttribute(MainEntityName.Constants.Fields.FieldName1).setValue(false);
            }

        },

        DoSomethingBasedOnValue: function (fc) {

            let myFieldValue = fc.getAttribute(MainEntityName.Constants.Fields.FieldName1).getValue();

            if (myFieldValue !== null) {

                switch (myFieldValue) {
                    case MainEntityName.Constants.OptionSets.StateCodeEnum.Custom1:
                    case MainEntityName.Constants.OptionSets.StateCodeEnum.Custom2:
                    case MainEntityName.Constants.OptionSets.StateCodeEnum.Custom3:
                        MainEntityName.Functions.BusinessLogic.DoSomething(fc);
                        break;
                    case MainEntityName.Constants.OptionSets.StateCodeEnum.Active:
                        fc.getControl(MainEntityName.Constants.Fields.FieldName1).setDisabled(false);
                        break;
                    default:
                        fc.getControl(MainEntityName.Constants.Fields.FieldName1).setDisabled(true);
                        break;
                }
            }
        },

        RetrieveSomethingAndDoSomething: function (fc) {

            let myValue = "exampleValue";

            Xrm.WebApi.online.retrieveMultipleRecords(MainEntityName.Constants.Entities.RelatedEntity1.LogicalName, MainEntityName.Queries.QueryToRetrieveSomething(fc, myValue)).then(
                function success(results) {
                    // Do something with retrieved records.
                    for (let i = 0; i < result.entities.length; i++) {
                        // Do something
                    }
                },
                function (error) {
                    // Handle error in this errorCallback function
                    Xrm.Navigation.openAlertDialog(error.message);
                }
            );
        },
    },

    __namespace: true
}

MainEntityName.Queries = {

    QueryToRetrieveSomething: function (fc, fieldValue) {
        let fetchXml =
            "<fetch no-lock='true'> \
                    <entity name='publisher_entityname'> \
                        <attribute name='publisher_fieldname' /> \
                        <filter type='and'> \
                            <condition attribute='publisher_fieldname1' operator='null' /> \
                            <condition attribute='publisher_entitynameid' operator='eq' value='" + fc.data.entity.getId() + "' /> \
                            <condition attribute='publisher_fieldname2' operator='eq' value='" + fieldValue + "' /> \
                            <condition attribute='publisher_fieldname3' operator='eq' value='" + MainEntityName.Constants.OptionSets.StateCodeEnum.Custom1 + "' /> \
                        </filter> \
                    </entity> \
                </fetch >";

        fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);

        return fetchXml;
    },

    __namespace: true
}

MainEntityName.Constants =
{
    General:
    {
        FormType: {
            Undefined: 0,
            Create: 1,
            Update: 2
        },

        UI: {
            WarningMessage1:
            {
                Message: "Your custom message goes here.",
                Level: "WARNING",
                UniqueId: "001",
            },
            ErrorMessage1:
            {
                Message: "Your custom message goes here.",
                Level: "ERROR",
                UniqueId: "002",
            },
        },
    },

    Fields:
    {
        FieldName1: "publisher_fieldname1",
        FieldName2: "publisher_fieldname2",
        FieldName3: "publisher_fieldname3",
    },

    OptionSets: {
        StateCodeEnum: {
            Active: 0,
            Inactive: 1,
            Custom1: 123567,
            Custom2: 1235678,
            Custom3: 12356789,
        },
    },

    Entities: {

        RelatedEntity1: {

            LogicalName: "publisher_relatedentityname",

            Fields: {
                FieldName1: "publisher_fieldname1",
                FieldName2: "publisher_fieldname2",
                FieldName3: "publisher_fieldname3"
            }
        }
    },

    __namespace: true
}

