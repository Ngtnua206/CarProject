using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CarProject.Services;

public class SepaySettings
{
    public string ApiKey { get; set; } = "";
    public string BankAccount { get; set; } = "";
    public string BankName { get; set; } = "";
    public string BankNumber { get; set; } = "";
    public string WebhookSecret { get; set; } = "";
    public string WebhookBaseUrl { get; set; } = "";
}

public class SepayWebhookPayload
{
    public long id { get; set; }
    public string gateway { get; set; } = "";
    public string transaction_date { get; set; } = "";
    public string account_number { get; set; } = "";
    public decimal amount { get; set; }
    public string content { get; set; } = "";
    public string status { get; set; } = "";
    public string reference_code { get; set; } = "";
}

public class SepayWebhookResponse
{
    public bool success { get; set; }
    public string message { get; set; } = "";
    public SepayWebhookData data { get; set; } = new();
}

public class SepayWebhookData
{
    public long id { get; set; }
    public string transaction_date { get; set; } = "";
    public decimal amount { get; set; }
    public string content { get; set; } = "";
    public string reference_code { get; set; } = "";
}

public interface ISepayService
{
    Task<string> GenerateTransactionCodeAsync(int maDonCoc);
    Task<SepayQrResult?> GenerateQrAsync(decimal amount, string orderCode, string description);
    bool VerifyWebhook(string requestBody, string signatureHeader);
    string ExtractTransactionCode(string content);
}

public class SepayQrResult
{
    public string QrImageUrl { get; set; } = "";
    public string QrDataUrl { get; set; } = "";
}

public class SepayService : ISepayService
{
    private readonly SepaySettings _settings;
    private readonly HttpClient _http;
    private readonly ILogger<SepayService> _logger;

    public SepayService(Microsoft.Extensions.Options.IOptions<SepaySettings> options, HttpClient http, ILogger<SepayService> logger)
    {
        _settings = options.Value;
        _http = http;
        _logger = logger;
    }

    public async Task<string> GenerateTransactionCodeAsync(int maDonCoc)
    {
        var code = $"MGC{maDonCoc:D6}{DateTime.Now:yyMMddHHmm}";
        return await Task.FromResult(code);
    }

    public async Task<SepayQrResult?> GenerateQrAsync(decimal amount, string orderCode, string description)
    {
        try
        {
            var body = new
            {
                amount = (long)amount,
                order_code = orderCode,
                content = description
            };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://my.sepay.vn/api/v1/merchant/create");
            request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
            request.Content = content;

            var response = await _http.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Sepay QR generation failed: {Status} {Body}", response.StatusCode, responseJson);
                return null;
            }

            var result = JsonSerializer.Deserialize<JsonElement>(responseJson);
            return new SepayQrResult
            {
                QrImageUrl = result.TryGetProperty("qr_image_url", out var img) ? img.GetString() ?? "" : "",
                QrDataUrl = result.TryGetProperty("qr_data_url", out var data) ? data.GetString() ?? "" : ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sepay QR generation error");
            return null;
        }
    }

    public bool VerifyWebhook(string requestBody, string signatureHeader)
    {
        try
        {
            if (string.IsNullOrEmpty(_settings.WebhookSecret))
                return true;

            var keyBytes = Encoding.UTF8.GetBytes(_settings.WebhookSecret);
            var bodyBytes = Encoding.UTF8.GetBytes(requestBody);
            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(bodyBytes);
            var computed = Convert.ToHexString(hash).ToLower();

            return computed == signatureHeader?.ToLower();
        }
        catch
        {
            return false;
        }
    }

    public string ExtractTransactionCode(string content)
    {
        if (string.IsNullOrEmpty(content)) return "";

        var match = System.Text.RegularExpressions.Regex.Match(content, @"MGC(\d{6})");
        if (match.Success)
            return match.Value;

        match = System.Text.RegularExpressions.Regex.Match(content, @"MGC\d{6}\d{12}");
        if (match.Success)
            return match.Value;

        return "";
    }
}
