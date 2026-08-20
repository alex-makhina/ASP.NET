using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:8093/hubs/promocode")
    .WithAutomaticReconnect()
    .Build();

connection.On<PromoCodeIssuedMessage>("PromoCodeIssued", msg =>
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Получен промокод: {msg.PromoCode}");
    Console.WriteLine($"  Сервис: {msg.ServiceInfo}");
    Console.WriteLine($"  Партнёр: {msg.PartnerId}");
    Console.WriteLine($"  Предпочтение: {msg.PreferenceId}");
    Console.WriteLine($"  Период: {msg.BeginDate} — {msg.EndDate}");
});

try
{
    await connection.StartAsync();
    Console.WriteLine("Подключено к http://localhost:8093/hubs/promocode");
    Console.WriteLine("Ожидание событий PromoCodeIssued... (Ctrl+C для выхода)");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка подключения: {ex.Message}");
    return;
}

await Task.Delay(Timeout.Infinite);

class PromoCodeIssuedMessage
{
    public Guid PartnerId { get; set; }
    public string PromoCode { get; set; }
    public string ServiceInfo { get; set; }
    public Guid PreferenceId { get; set; }
    public string BeginDate { get; set; }
    public string EndDate { get; set; }
}