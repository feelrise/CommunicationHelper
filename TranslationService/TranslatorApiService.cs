using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TranslationService
{
    public class TranslatorApiService : ITranslatorApiService
    {
        private const String SubscriptionKey = "0cfee141326a464d83fe75d4eb4c4540";
        private const String Endpoint = "https://api.cognitive.microsofttranslator.com";
            
        public async Task<TranslateResult> Translate(String original, String initialLanguage, String targetLanguage)
        {
            return await GetTranslationAsync(original, $"/translate?api-version=3.0&from={initialLanguage}&to={targetLanguage}");
        }

        public async Task<TranslateResult> Translate(String original, String targetLanguage)
        {
            return await GetTranslationAsync(original, $"/translate?api-version=3.0&to={targetLanguage}");
        }

        private static async Task<TranslateResult> GetTranslationAsync(String original, String route)
        {
            Object[] body = {new {Text = original}};
            var requestBody = JsonConvert.SerializeObject(body);
            TranslationResult[] deserializedOutput;

            using (var client = new HttpClient())
            {
                using var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri =
                        new Uri(Endpoint + route),
                    Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);

                var response = await client.SendAsync(request).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync();

                deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
            }

            return new TranslateResult {Translated = deserializedOutput[0].Translations[0].Text};
        }

        public async Task<IEnumerable<LanguageInfo>> GetAvailableLanguages()
        {
            var json = await GetCulturesJson();
            var cultures = json.SelectToken("translation");

            var language = new List<LanguageInfo>();
            
            foreach (var culture in cultures.Children<JProperty>())
            {
                var info = culture.Value.ToObject<LanguageInfo>();
                info.Key = culture.Name;
                language.Add(info);
            }

            return language;
        }

        private static async Task<JObject> GetCulturesJson()
        {
            const String route = "/languages?api-version=3.0";
            using var client = new HttpClient();
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get, RequestUri = new Uri(Endpoint + route)
            };

            var response = await client.SendAsync(request);
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
    }
}