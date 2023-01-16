using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCDWeb.Areas.RCD.Uility
{
    public class MapObj
    {
        public T ObjectToStatic<T>(object expando, T entity)
        {
            if (entity==null) entity = Activator.CreateInstance<T>();

            var properties = expando.GetType().GetProperties();
            if (properties == null)
                return entity;

            foreach (var entry in properties)
            {
                var propertyExpando = expando.GetType().GetProperty(entry.Name);
                var propertyInfo = entity.GetType().GetProperty(entry.Name);
                if ((propertyExpando != null) && (propertyInfo != null))
                {
                    var pippo = propertyExpando.GetValue(expando);
                    System.Diagnostics.Debug.WriteLine(entry.Name);
                    // System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(pippo));
                    propertyInfo.SetValue(entity, pippo, null);
                }

            }
            return entity;
        }
    }
}