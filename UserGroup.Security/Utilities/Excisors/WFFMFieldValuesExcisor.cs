using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Diagnostics;
using Sitecore.Form.Core.Client.Data.Submit;
using Sitecore.Form.Core.Controls.Data;

using UserGroup.Security.Utilities.Excisors.Base;

namespace UserGroup.Security.Utilities.Excisors
{
    public class WFFMFieldValuesExcisor : IWFFMFieldValuesExcisor
    {
        private IEnumerable<string> FieldNamesForExtraction { get; set; }

        private WFFMFieldValuesExcisor(IEnumerable<string> fieldNamesForExtraction)
        {
            SetFieldNamesForExtraction(fieldNamesForExtraction);
        }

        private void SetFieldNamesForExtraction(IEnumerable<string> fieldNamesForExtraction)
        {
            Assert.ArgumentNotNull(fieldNamesForExtraction, "fieldNamesForExtraction");
            FieldNamesForExtraction = fieldNamesForExtraction;
        }

        public AdaptedResultList Excise(AdaptedResultList fields)
        {
            if(fields == null || fields.Count() < 1)
            {
                return fields;
            }

            List<AdaptedControlResult> adaptedControlResults = new List<AdaptedControlResult>();

            foreach(AdaptedControlResult field in fields)
            {
                adaptedControlResults.Add(GetExtractValueFieldIfApplicable(field));
            }

            return adaptedControlResults;
        }

        private AdaptedControlResult GetExtractValueFieldIfApplicable(AdaptedControlResult field)
        {
            if(ShouldExtractFieldValue(field))
            {
                return GetExtractedValueField(field);
            }

            return field;
        }

        private bool ShouldExtractFieldValue(ControlResult field)
        {
            return FieldNamesForExtraction.Contains(field.FieldName);
        }

        private static AdaptedControlResult GetExtractedValueField(ControlResult field)
        {
            return new AdaptedControlResult(CreateNewControlResultWithEmptyValue(field), true);
        }

        private static ControlResult CreateNewControlResultWithEmptyValue(ControlResult field)
        {
            ControlResult controlResult = new ControlResult(field.FieldName, string.Empty, string.Empty);
            controlResult.FieldID = field.FieldID;
            return controlResult;
        }

        public static IWFFMFieldValuesExcisor CreateNewWFFMFieldValuesExcisor(IEnumerable<string> fieldNamesForExtraction)
        {
            return new WFFMFieldValuesExcisor(fieldNamesForExtraction);
        }
    }
}
