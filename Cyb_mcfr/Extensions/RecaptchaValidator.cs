using Newtonsoft.Json;

namespace Cyb_mcfr.Extensions
{
    public class RecaptchaValidator
    {
        private readonly string _recaptchaSecretKey;

        public RecaptchaValidator(string recaptchaSecretKey)
        {
            _recaptchaSecretKey = recaptchaSecretKey;
        }

        public async Task<RecaptchaResult> ValidateAsync(string response)
        {
            // Use HttpClient to send a request to the reCAPTCHA verification endpoint
            // Parse the response and return the result

            // Example implementation using HttpClient:
            using (var httpClient = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
            {
                {"secret", _recaptchaSecretKey},
                {"response", response}
            };

                var responseMessage = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(parameters));
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<RecaptchaResult>(responseBody);

                return result;
            }
        }
    }

    public class RecaptchaResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

}
