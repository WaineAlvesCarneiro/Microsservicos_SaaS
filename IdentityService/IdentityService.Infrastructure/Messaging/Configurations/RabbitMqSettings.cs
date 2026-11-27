using System.ComponentModel.DataAnnotations;

namespace IdentityService.Infrastructure.Messaging.Configurations;

public class RabbitMqSettings
{
    [Required]
    public string Host { get; init; } = string.Empty;

    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

    [Required]
    public string QueueName { get; init; } = "fila_emails";

    [Required]
    public string DeadLetterExchange { get; init; } = "dlx_exchange";

    [Required]
    public string DeadLetterQueue { get; init; } = "fila_emails_erro";

    [Range(1, 65535)]
    public int Port { get; init; } = 5672;
}
