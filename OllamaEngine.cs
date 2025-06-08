using MemoQ.MTInterfaces;
using System;
using System.Drawing;

namespace MemoQOllamaPlugin
{
    public class OllamaEngine : EngineBase
    {
        private readonly string _apiEndpoint;
        private readonly string _model;

        public OllamaEngine(string apiEndpoint, string model)
        {
            _apiEndpoint = apiEndpoint;
            _model = model;
        }

        public override Image SmallIcon => null;
        public override bool SupportsFuzzyCorrection => false;

        public override void SetProperty(string name, string value) { }

        public override ISession CreateLookupSession()
        {
            return new OllamaSession(_apiEndpoint, _model);
        }

        public override ISessionForStoringTranslations CreateStoreTranslationSession()
        {
            return null;
        }

        public override void Dispose() { }
    }
}