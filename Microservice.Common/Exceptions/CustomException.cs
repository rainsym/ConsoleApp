using Newtonsoft.Json;
using System;

namespace Microservice.Common.Exceptions
{
    public class CustomException: Exception
    {
        public string Code { get; set; }

        public CustomException(string code, string message = null) : base(message)
        {
            Code = code;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new
            {
                Code,
                Message
            });
        }
    }
}
