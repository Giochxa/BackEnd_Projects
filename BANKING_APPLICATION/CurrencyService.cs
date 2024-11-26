using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BANKING_APPLICATION
{
    // Class responsible for currency-related functionality
    public class CurrencyService
    {
        private const string Url = "https://nbg.gov.ge/gw/api/ct/monetarypolicy/currencies/ka/json";

        public async Task<List<CurrencyData>> FetchCurrencyDataAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(Url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CurrencyData>>(jsonResponse);
                }
                else
                {
                    Console.WriteLine("Error fetching data: " + response.StatusCode);
                    return null;
                }
            }
        }

        public double? GetExchangeRateByCode(List<CurrencyData> currencyData, string code)
        {
            var currency = currencyData[0].Currencies
                .FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

            return currency?.Rate;
        }
    }

    public class CurrencyData
    {
        public string Date { get; set; }
        public List<Currency> Currencies { get; set; }
    }

    public class Currency
    {
        public string Code { get; set; }
        public double Rate { get; set; }
    }
    
}
