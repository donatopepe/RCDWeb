using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCDWeb.Areas.RCD.Uility
{
    public class ValidationError
    {

        public string PropertyName = "";
        public string[] ErrorList = null;
    }
    public class ModelStateErrors
    {
        public IEnumerable<ValidationError> GetModelStateErrors(ModelStateDictionary modelState)
        {
            var errors = (from m in modelState
                          where m.Value.Errors.Count() > 0
                          select
                             new ValidationError
                             {
                                 PropertyName = m.Key,
                                 ErrorList = (from msg in m.Value.Errors
                                              select msg.ErrorMessage).ToArray()
                             })
                                .AsEnumerable();
            return errors;
        }
    }

}