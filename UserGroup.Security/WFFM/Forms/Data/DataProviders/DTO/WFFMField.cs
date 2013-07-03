using System;

using Sitecore.Forms.Data;

namespace UserGroup.Security.WFFM.Forms.Data.DataProviders.DTO
{
    public class WFFMField : IField
    {
        public string Data { get; set; }

        public Guid FieldId { get; set; }

        public string FieldName { get; set; }

        public IForm Form { get; set; }

        public Guid Id { get; internal set; }

        public string Value { get; set; }
    }
}