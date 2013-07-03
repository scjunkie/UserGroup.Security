using System;
using System.Collections.Generic;
using System.Linq;

using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Forms.Data;
using Sitecore.Forms.Data.DataProviders;
using Sitecore.Reflection;

using UserGroup.Security.WFFM.Forms.Data.DataProviders.DTO;

namespace UserGroup.Security.WFFM.Forms.Data.DataProviders
{
    public class BlankOutFieldsDataProvider : WFMDataProviderBase
    {
        private static IEnumerable<string> _FieldsToBlankOut;
        private static IEnumerable<string> FieldsToBlankOut
        {
            get
            {
                if (_FieldsToBlankOut == null)
                {
                    _FieldsToBlankOut = Factory.GetStringSet("FieldsToBlankOut/FieldName");
                }

                return _FieldsToBlankOut;
            }
        }

        private WFMDataProviderBase InnerProvider { get; set; }

        public BlankOutFieldsDataProvider(string connectionString, string innerProvider)
            : this(CreateInnerProvider(innerProvider, connectionString))
        {
        }

        public BlankOutFieldsDataProvider(WFMDataProviderBase innerProvider)
        {
            SetInnerProvider(innerProvider);
        }

        private static WFMDataProviderBase CreateInnerProvider(string innerProvider, string connectionString = null)
        {
            Assert.ArgumentNotNullOrEmpty(innerProvider, "innerProvider");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return ReflectionUtil.CreateObject(innerProvider, new[] { connectionString }) as WFMDataProviderBase;
            }

            return ReflectionUtil.CreateObject(innerProvider, new object[0]) as WFMDataProviderBase;
        }
        
        private void SetInnerProvider(WFMDataProviderBase innerProvider)
        {
            Assert.ArgumentNotNull(innerProvider, "innerProvider");
            InnerProvider = innerProvider;
        }
        
        public override void ChangeStorage(Guid formItemId, string newStorage)
        {
            InnerProvider.ChangeStorage(formItemId, newStorage);
        }

        public override void ChangeStorageForForms(IEnumerable<Guid> ids, string storageName)
        {
            InnerProvider.ChangeStorageForForms(ids, storageName);
        }

        public override void DeleteForms(IEnumerable<Guid> formSubmitIds)
        {
            InnerProvider.DeleteForms(formSubmitIds);
        }

        public override void DeleteForms(Guid formItemId, string storageName)
        {
            InnerProvider.DeleteForms(formItemId, storageName);
        }

        public override IEnumerable<IPool> GetAbundantPools(Guid fieldId, int top, out int total)
        {
            return InnerProvider.GetAbundantPools(fieldId, top, out total);
        }

        public override IEnumerable<IForm> GetForms(QueryParams queryParams, out int total)
        {
            return InnerProvider.GetForms(queryParams, out total);
        }

        public override IEnumerable<IForm> GetFormsByIds(IEnumerable<Guid> ids)
        {
            return InnerProvider.GetFormsByIds(ids);
        }

        public override int GetFormsCount(Guid formItemId, string storageName, string filter)
        {
            return InnerProvider.GetFormsCount(formItemId, storageName, filter);
        }

        public override IEnumerable<IPool> GetPools(Guid fieldId)
        {
            return InnerProvider.GetPools(fieldId);
        }

        public override void InsertForm(IForm form)
        {
            RipOutFieldsIfApplicable(form);
            InnerProvider.InsertForm(form);
        }

        public override void ResetPool(Guid fieldId)
        {
            InnerProvider.ResetPool(fieldId);
        }

        public override IForm SelectSingleForm(Guid fieldId, string likeValue)
        {
            return InnerProvider.SelectSingleForm(fieldId, likeValue);
        }

        public override bool UpdateForm(IForm form)
        {
            RipOutFieldsIfApplicable(form);
            return InnerProvider.UpdateForm(form);
        }

        private void RipOutFieldsIfApplicable(IEnumerable<IForm> forms)
        {
            Assert.ArgumentNotNull(forms, "forms");
            foreach (IForm form in forms)
            {
                RipOutFieldsIfApplicable(form);
            }
        }

        private void RipOutFieldsIfApplicable(IForm form)
        {
            Assert.ArgumentNotNull(form, "form");
            Assert.ArgumentNotNull(form.Field, "form.Field");
            form.Field = RipOutFieldsIfApplicable(form.Field);
        }

        private IEnumerable<IField> RipOutFieldsIfApplicable(IEnumerable<IField> fields)
        {
            Assert.ArgumentNotNull(fields, "fields");
            IList<IField> manipulatedFields = new List<IField>();
            foreach (IField field in fields)
            {
                manipulatedFields.Add(RipOutFieldValueIfApplicable(field));
            }

            return manipulatedFields;
        }

        private IField RipOutFieldValueIfApplicable(IField field)
        {
            Assert.ArgumentNotNull(field, "field");
            if (!IsFieldToRipOut(field))
            {
                return field;
            }

            return CreateNewWFFMField(field, field.FieldName, string.Empty, string.Empty);
        }

        private static bool IsFieldToRipOut(IField field)
        {
            Assert.ArgumentNotNull(field, "field");
            return FieldsToBlankOut.Contains(field.FieldName);
        }

        private static IField CreateNewWFFMField(IField field, string fieldName, string value, string data)
        {
            if (field != null)
            {
                return new WFFMField
                {
                    Data = data,
                    FieldId = field.FieldId,
                    FieldName = fieldName,
                    Form = field.Form,
                    Id = field.Id,
                    Value = value
                };
            }

            return null;
        }
    }
}
