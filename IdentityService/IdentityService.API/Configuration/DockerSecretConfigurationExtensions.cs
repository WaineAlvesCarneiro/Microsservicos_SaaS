namespace IdentityService.API.Configuration;

public static class DockerSecretConfigurationExtensions
{
    private static readonly string[] SecretKeys =
    [
        "Jwt:Key",

        "SqlServer:UserName",
        "SqlServer:Password",

        "RabbitMq:UserName",
        "RabbitMq:Password",

        "AdminSettings:UserName",
        "AdminSettings:Password"
    ];

    public static IConfigurationManager AddDockerSecretFiles(this IConfigurationManager configuration)
    {
        foreach (var key in SecretKeys)
        {
            var filePath = configuration[$"{key}:File"];
            if (string.IsNullOrWhiteSpace(filePath))
                continue;

            if (!File.Exists(filePath))
                throw new InvalidOperationException($"O arquivo do secret configurado para '{key}' não foi encontrado.");

            var value = File.ReadAllText(filePath).TrimEnd('\r', '\n');
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"O arquivo do secret configurado para '{key}' está vazio.");

            configuration[key] = value;
        }

        return configuration;
    }
}
