using Microsoft.Extensions.Configuration;

namespace LeonardoStore.SharedContext.MessageBus
{
    public static class ConfigurationExtension
    {
        public static string GetMessageQueueConnection(this IConfiguration configuration, string name)
        {
            return configuration?.GetSection("MessageQueueConnection")?[name];
        }
    }
}