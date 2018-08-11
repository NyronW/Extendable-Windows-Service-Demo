using Newtonsoft.Json.Serialization;
using System;

namespace WillCorp.App.Web.SignalR
{
    /// <summary>
    /// Convert property name to camelCase
    /// </summary>
    public class SignalRContractResolver : IContractResolver
    {
        private readonly IContractResolver camelCaseContractResolver;

        public SignalRContractResolver()
        {
            camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public JsonContract ResolveContract(Type type)
        {
            return camelCaseContractResolver.ResolveContract(type);
        }
    }
}
