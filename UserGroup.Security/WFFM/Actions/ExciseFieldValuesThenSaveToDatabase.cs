using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Form.Core.Client.Data.Submit;
using Sitecore.Form.Submit;

using UserGroup.Security.Utilities.Excisors;
using UserGroup.Security.Utilities.Excisors.Base;

namespace UserGroup.Security.WFFM.Actions
{
    public class ExciseFieldValuesThenSaveToDatabase : SaveToDatabase
    {
        private static IEnumerable<string> _FieldsToExcise;
        private static IEnumerable<string> FieldsToExcise
        {
            get
            {
                if (_FieldsToExcise == null)
                {
                    _FieldsToExcise = Factory.GetStringSet("FieldsToBlankOut/FieldName");
                }

                return _FieldsToExcise;
            }
        }
        
        public override void Execute(ID formId, AdaptedResultList fields, object[] data)
        {
            base.Execute(formId, ExciseFields(fields), data);
        }

        private AdaptedResultList ExciseFields(AdaptedResultList fields)
        {
            IWFFMFieldValuesExcisor excisor = WFFMFieldValuesExcisor.CreateNewWFFMFieldValuesExcisor(FieldsToExcise);
            return excisor.Excise(fields);
        }
    }
}