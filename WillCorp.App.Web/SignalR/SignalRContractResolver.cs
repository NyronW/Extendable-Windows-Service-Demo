using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace WillCorp.App.Web.SignalR
{
    public class SignalRContractResolver : IContractResolver
    {

        private readonly Assembly assembly;
        private readonly IContractResolver camelCaseContractResolver;
        private readonly IContractResolver defaultContractSerializer;

        public SignalRContractResolver()
        {
            defaultContractSerializer = new DefaultContractResolver();
            camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public JsonContract ResolveContract(Type type)
        {
            return camelCaseContractResolver.ResolveContract(type);
        }

    }
}
