using System.Text.Json;
using System.Text;


public class NovinoPay
{
    private readonly HttpClient _httpClient;
    private const string MerchantId = "3ED781FE-1C4C-4253-9C9E-87A66A847824"; // کد درگاه پرداخت نوینو
    private const string PaymentRequestUrl = "https://api.novinopay.com/payment/ipg/v2/request";
    private const string CallbackUrl = "https://YourDomain.com/callback"; // آدرس بازگشتی

    public NovinoPay(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CreateTransaction(decimal amount, string invoiceId, string description, string email, string mobile, string name)
    {
        var requestData = new
        {
            merchant_id = MerchantId,
            amount = (int)(amount * 10), // تبدیل به ریال
            callback_url = CallbackUrl,
            callback_method = "GET",
            invoice_id = invoiceId,
            description = description,
            email = email,
            mobile = mobile,
            name = name
        };

        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(PaymentRequestUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentResponse>(responseData);

            if (result != null && result.status == "100")
            {
                return result.data.payment_url; // لینک پرداخت
            }
            else
            {
                throw new Exception($"Error: {result?.message ?? "Unknown error"}");
            }
        }
        else
        {
            throw new Exception("Failed to create transaction");
        }
    }

    public async Task<bool> VerifyTransaction(string authority, decimal amount)
    {
        var verificationUrl = "https://api.novinopay.com/payment/ipg/v2/verification";
        var verifyData = new
        {
            merchant_id = MerchantId,
            amount = (int)(amount * 10), // تبدیل به ریال
            authority = authority
        };

        var content = new StringContent(JsonSerializer.Serialize(verifyData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(verificationUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentResponse>(responseData);

            return result != null && result.status == "100"; // تأیید موفقیت‌آمیز
        }
        return false;
    }
}

public class PaymentResponse
{
    public string status { get; set; }
    public string message { get; set; }
    public PaymentData data { get; set; }
}

public class PaymentData
{
    public string authority { get; set; }
    public int trans_id { get; set; }
    public string payment_url { get; set; }
}


