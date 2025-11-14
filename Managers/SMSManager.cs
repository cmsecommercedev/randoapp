using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FMLigApi.Managers
{
    using System.Net.Http.Headers;
    using System.Text;
    using System.Xml;
    using Microsoft.Extensions.Logging;

    public class SmsManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsManager> _logger;
        private readonly HttpClient _httpClient;

        public SmsManager(IConfiguration configuration, ILogger<SmsManager> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<string> SendOtpSmsAsync(string message, string phoneNumber)
        {
            var username = _configuration["Netgsm:Username"];
            var password = _configuration["Netgsm:Password"];
            var apiUrl = _configuration["Netgsm:ApiUrl"];   

            // XML verisini oluştur
            var xmlBuilder = new StringBuilder();
            xmlBuilder.AppendLine("<?xml version='1.0' encoding='UTF-8'?>");
            xmlBuilder.AppendLine("<mainbody>");
            xmlBuilder.AppendLine("<header>");
            xmlBuilder.AppendLine($"<usercode>{username}</usercode>");
            xmlBuilder.AppendLine($"<password>{password}</password>");
            xmlBuilder.AppendLine($"<msgheader>{username}</msgheader>"); 
            xmlBuilder.AppendLine("</header>");
            xmlBuilder.AppendLine("<body>");
            xmlBuilder.AppendLine($"<msg><![CDATA[{message}]]></msg>");
            xmlBuilder.AppendLine($"<no>{phoneNumber}</no>");
            xmlBuilder.AppendLine("</body>");
            xmlBuilder.AppendLine("</mainbody>");

            var content = new StringContent(xmlBuilder.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            try
            {
                var response = await _httpClient.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Netgsm SMS gönderimi başarısız. Status: {Status}, Body: {Body}", response.StatusCode, responseContent);
                    return $"Hata: {response.StatusCode} - {responseContent}";
                }

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Netgsm OTP SMS gönderimi sırasında hata oluştu.");
                return $"Hata: {ex.Message}";
            }
        }
         

    }

}
