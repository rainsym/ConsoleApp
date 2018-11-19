using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Common.CustomAttributes
{
    public class SQLViewName : Attribute
    {
        public string SqlViewName { get; set; }

        public SQLViewName(string sqlViewName)
        {
            if (string.IsNullOrEmpty(sqlViewName))
            {
                throw new ArgumentNullException("sqlViewName");
            }

            SqlViewName = sqlViewName;
        }
    }
}
