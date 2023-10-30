using System.Net.Mime;
using System.Text.Json;
using Microsoft.Net.Http.Headers;

namespace platform6_service
{
    public class PortalPayloadBuilder
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger _logger;
        private readonly string _scriptBundle;

        public PortalPayloadBuilder(IHttpClientFactory? httpClientFactory, ILogger logger, string scriptBundle)
        {
            _logger = logger;
            _scriptBundle = scriptBundle;
            if (null == httpClientFactory)
            {
                throw new IOException("HttpClientFactory is null!");
            }
            _httpClientFactory = httpClientFactory;
        }

        public async Task<String> getPriceAsync(string id)
        {
            DataResponse response = new DataResponse(await refreshDataAsync());
            foreach (AssetResponse asset in response.data)
            {
                if (asset.id.Equals(id))
                {
                    return asset.priceUsd;
                }
            }
            return "";
        }

        public async Task<IResult> buildDataAsync()
        {
            DataResponse response = new DataResponse(await refreshDataAsync());
            return Results.Content(JsonSerializer.Serialize(response), MediaTypeNames.Application.Json);
        }

        public IResult build()
        {
            PortalResponse response = new PortalResponse(_scriptBundle);
            return Results.Content(JsonSerializer.Serialize(response), MediaTypeNames.Application.Json);
        }

        private async Task<AssetResponse[]> refreshDataAsync()
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.coincap.io/v2/assets")
            {
                Headers =
            {
                { HeaderNames.Accept, MediaTypeNames.Application.Json },
                { HeaderNames.UserAgent, Constants.P6UserAgent }
                }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var arrayResponse = await httpResponseMessage.Content.ReadFromJsonAsync<AssetResponses>();
                if (null != arrayResponse)
                {
                    _logger.LogInformation("Crypto Asset Data Retrieved Sucessfully");
                    return arrayResponse.data;
                }

            }
            _logger.LogInformation("Crypto Asset Data Retrieval failed!");
            return new AssetResponses().data;
        }
    }

    // JSON Response
    public class PortalResponse
    {
        public PortalResponse(string script)
        {
            this.script = script;
        }
        public string script { get; set; }
    }

    // JSON Response
    public class DataResponse
    {
        public DataResponse(AssetResponse[] data)
        {
            this.data = data;
        }
        public AssetResponse[] data { get; set; }
    }

    public class AssetResponses
    {
        public AssetResponse[] data { get; set; }
    }

    public class AssetResponse
    {
        public string id { get; set; }	              // unique identifier for asset
        public string rank { get; set; }              // rank is in ascending order - this number is directly associated with the marketcap whereas the highest marketcap receives rank 1
        public string symbol { get; set; }	          // most common symbol used to identify this asset on an exchange
        public string name { get; set; }	          // proper name for asset
        public string supply { get; set; }	          // available supply for trading
        public string maxSupply { get; set; }         // total quantity of asset issued
        public string marketCapUsd { get; set; }	  // supply x price
        public string volumeUsd24Hr { get; set; }     // quantity of trading volume represented in USD over the last 24 hours
        public string priceUsd { get; set; }	      // volume-weighted price based on real-time market data, translated to USD
        public string changePercent24Hr { get; set; } // the direction and value change in the last 24 hours
        public string vwap24Hr { get; set; }	      // Volume Weighted Average Price in the last 24 hours
    }

}