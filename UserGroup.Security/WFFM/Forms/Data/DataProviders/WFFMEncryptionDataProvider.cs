using System;
using System.Collections.Generic;

using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Forms.Data;
using Sitecore.Forms.Data.DataProviders;
using Sitecore.Reflection;

using UserGroup.Security.Security.Encryption;
using UserGroup.Security.Security.Encryption.DTO;
using UserGroup.Security.WFFM.Forms.Data.DataProviders.DTO;

namespace UserGroup.Security.WFFM.Forms.Data.DataProviders
{
    public class WFFMEncryptionDataProvider : WFMDataProviderBase
    {
        private WFMDataProviderBase InnerProvider { get; set; }
        private IEncryptor Encryptor { get; set; }

        public WFFMEncryptionDataProvider(string innerProvider)
            : this(CreateInnerProvider(innerProvider), CreateDefaultEncryptor())
        {
        }

        public WFFMEncryptionDataProvider(string connectionString, string innerProvider)
            : this(CreateInnerProvider(innerProvider, connectionString), CreateDefaultEncryptor())
        {
        }

        public WFFMEncryptionDataProvider(WFMDataProviderBase innerProvider)
            : this(innerProvider, CreateDefaultEncryptor())
        {
        }

        public WFFMEncryptionDataProvider(WFMDataProviderBase innerProvider, IEncryptor encryptor)
        {
            SetInnerProvider(innerProvider);
            SetEncryptor(encryptor);
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

        private static IEncryptor CreateDefaultEncryptor()
        {
            return DataNullTerminatorEncryptor.CreateNewDataNullTerminatorEncryptor(GetEncryptorSettings());
        }

        private static DataNullTerminatorEncryptorSettings GetEncryptorSettings()
        {
            return new DataNullTerminatorEncryptorSettings
            {
                EncryptionDataNullTerminator = Settings.GetSetting("WFFM.Encryption.DataNullTerminator"),
                InnerEncryptor = RC2Encryptor.CreateNewRC2Encryptor(Settings.GetSetting("WFFM.Encryption.Key"))
            };
        }

        private void SetEncryptor(IEncryptor encryptor)
        {
            Assert.ArgumentNotNull(encryptor, "encryptor");
            Encryptor = encryptor;
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
            IEnumerable<IForm> forms = InnerProvider.GetForms(queryParams, out total);
            DecryptForms(forms);
            return forms;
        }

        public override IEnumerable<IForm> GetFormsByIds(IEnumerable<Guid> ids)
        {
            IEnumerable<IForm> forms = InnerProvider.GetFormsByIds(ids);
            DecryptForms(forms);
            return forms;
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
            EncryptForm(form);
            InnerProvider.InsertForm(form);
        }

        public override void ResetPool(Guid fieldId)
        {
            InnerProvider.ResetPool(fieldId);
        }

        public override IForm SelectSingleForm(Guid fieldId, string likeValue)
        {
            IForm form = InnerProvider.SelectSingleForm(fieldId, likeValue);
            DecryptForm(form);
            return form;
        }

        public override bool UpdateForm(IForm form)
        {
            EncryptForm(form);
            return InnerProvider.UpdateForm(form);
        }

        private void EncryptForms(IEnumerable<IForm> forms)
        {
            Assert.ArgumentNotNull(forms, "forms");
            foreach (IForm form in forms)
            {
                EncryptForm(form);
            }
        }

        private void EncryptForm(IForm form)
        {
            Assert.ArgumentNotNull(form, "form");
            Assert.ArgumentNotNull(form.Field, "form.Field");
            form.Field = EncryptFields(form.Field);
        }

        private IEnumerable<IField> EncryptFields(IEnumerable<IField> fields)
        {
            Assert.ArgumentNotNull(fields, "fields");
            IList<IField> encryptedFields = new List<IField>();
            foreach (IField field in fields)
            {
                encryptedFields.Add(EncryptField(field));
            }

            return encryptedFields;
        }

        private IField EncryptField(IField field)
        {
            Assert.ArgumentNotNull(field, "field");

            if(string.IsNullOrEmpty(field.Data))
            {
                return CreateNewWFFMField(field, Encrypt(field.FieldName), Encrypt(field.Value), string.Empty);
            }

            return CreateNewWFFMField(field, Encrypt(field.FieldName), Encrypt(field.Value), Encrypt(field.Data));
        }

        private void DecryptForms(IEnumerable<IForm> forms)
        {
            Assert.ArgumentNotNull(forms, "forms");
            foreach (IForm form in forms)
            {
                DecryptForm(form);
            }
        }

        private void DecryptForm(IForm form)
        {
            Assert.ArgumentNotNull(form, "form");
            Assert.ArgumentNotNull(form.Field, "form.Field");
            form.Field = DecryptFields(form.Field);
        }

        private IEnumerable<IField> DecryptFields(IEnumerable<IField> fields)
        {
            Assert.ArgumentNotNull(fields, "fields");
            IList<IField> decryptedFields = new List<IField>();
            foreach (IField field in fields)
            {
                decryptedFields.Add(DecryptField(field));
            }

            return decryptedFields;
        }

        private IField DecryptField(IField field)
        {
            Assert.ArgumentNotNull(field, "field");
            return CreateNewWFFMField(field, Decrypt(field.FieldName), Decrypt(field.Value), Decrypt(field.Data));
        }

        private string Encrypt(string input)
        {
            return Encryptor.Encrypt(input);
        }

        private string Decrypt(string input)
        {
            return Encryptor.Decrypt(input);
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
