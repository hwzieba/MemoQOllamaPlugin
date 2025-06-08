using MemoQ.Addins.Common.DataStructures;
using MemoQ.MTInterfaces;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MemoQOllamaPlugin
{
    public class OllamaSession : ISession
    {
        private readonly string _apiEndpoint;
        private readonly string _model;
        private static readonly HttpClient client = new HttpClient();

        public OllamaSession(string apiEndpoint, string model)
        {
            _apiEndpoint = apiEndpoint;
            _model = model;
        }

        public TranslationResult TranslateCorrectSegment(Segment segm, Segment tmSource, Segment tmTarget)
        {
            var plainText = segm.PlainText;

            var translationPrompt = $"\nTranslate the following text from English to Polish.\nRules:\n1. Return ONLY the translation\n2. No additional comments\n3. No explanations\n4. No notes\n5. No prefix/suffix\n\nText to translate:\n{plainText}\n";

            var requestPayload = new
            {
                model = _model,
                prompt = translationPrompt,
                stream = false,
                options = new { temperature = 0.3 } //kreatywność
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestPayload), Encoding.UTF8, "application/json");
            var response = client.PostAsync($"{_apiEndpoint}/api/generate", content).Result;

            var responseJson = response.Content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject<JObject>(responseJson);
            string translatedText = obj["response"].ToString();

            var builder = new SegmentBuilder();
            builder.AppendString(translatedText);

            return new TranslationResult
            {
                Translation = builder.ToSegment(),
                Confidence = 0.9,
                Info = "Ollama",
                Exception = null
            };
        }

        public TranslationResult[] TranslateCorrectSegment(Segment[] segs, Segment[] tmSources, Segment[] tmTargets)
        {
            var results = new TranslationResult[segs.Length];
            for (int i = 0; i < segs.Length; i++)
            {
                results[i] = TranslateCorrectSegment(segs[i], tmSources?[i], tmTargets?[i]);
            }
            return results;
        }

        public void Dispose() { }
    }
}