using Barkod2021API_INTelligence.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Barkod2021API_INTelligence.Logic
{
    public class CurrencyService
    {
        public bool CheckTimeStamp(long currentUnix,long unixFromDict)
        {
            long currentUnixSeconds = currentUnix / 1000;
            long unixFromDictSeconds = unixFromDict / 1000;
            if ((currentUnixSeconds - unixFromDictSeconds) > 30) return false;
            return true;
        }

        public async Task<ConversionResponseModel> GetCurrency(CurrencyModel currencyModel)
        {
            HttpClient httpClient = new HttpClient();
            ConversionResponseModel responseModel = new ConversionResponseModel();
            string url = "https://api.currencylayer.com/convert?from=" + currencyModel.fromCurr + "&to=" + currencyModel.toCurr + "&amount=" + currencyModel.amount + "&access_key=96c7a03cce11e464756d645302fbb324";
            var response = await httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseString);
            var result = jObject.SelectToken("result");
            var quote = jObject.SelectToken("info.quote");
            DateTime date = DateTime.Now;
            string dateToChange = date.ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(dateToChange);
            long unixTimeStamp = dateTimeOffset.ToUnixTimeMilliseconds();
            responseModel.result = decimal.Parse(result.ToString());
            responseModel.result = Math.Round(responseModel.result, 2);
            responseModel.timeStamp = unixTimeStamp;
            return responseModel;
        }
    }
}
