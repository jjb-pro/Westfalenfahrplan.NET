using System.Text;

namespace Westfalenfahrplan.NET.Helper
{
    public class UrlBuilder
    {
        private readonly StringBuilder _baseUrl;
        private bool _hasQueryParameters;

        public UrlBuilder(string baseUrl, bool hasQueryParameters = false)
        {
            _baseUrl = new StringBuilder(baseUrl);
            _hasQueryParameters = hasQueryParameters;
        }

        public UrlBuilder AddPathSegment(string segment)
        {
            if (_baseUrl[_baseUrl.Length - 1] != '/')
                _baseUrl.Append('/');

            _baseUrl.Append(segment);
            return this;
        }

        public UrlBuilder AddQueryParameter(string key, string value)
        {
            if (!_hasQueryParameters)
            {
                _baseUrl.Append('?');
                _hasQueryParameters = true;
            }
            else
            {
                _baseUrl.Append('&');
            }

            _baseUrl.Append(key).Append('=').Append(value);
            return this;
        }

        public string Build() => _baseUrl.ToString();
    }
}