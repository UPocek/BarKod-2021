using Barkod2021API_INTelligence.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Barkod2021API_INTelligence.Logic;

namespace Barkod2021API_INTelligence.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private const string _ACCESSKEY = "96c7a03cce11e464756d645302fbb324";
        static Dictionary<string, CachingModel> dictCache = new Dictionary<string, CachingModel>()
        {
            { "RSDEUR",null},
            { "RSDUSD",null },
            { "RSDJPY",null },
            { "EURRSD",null},
            { "USDRSD",null},
            { "JPYRSD",null }
        };

        [HttpGet]
        [Route("/Convert")]
        public async Task<ActionResult> ConvertCurrencies([FromQuery] CurrencyModel currencyModel)
        {
            string currencyPair = currencyModel.fromCurr + currencyModel.toCurr;
            CurrencyService service = new CurrencyService();
            DateTimeOffset dateTimeOffset1 = DateTimeOffset.UtcNow;
            long currentTimeStamp = dateTimeOffset1.ToUnixTimeMilliseconds();
            decimal sum = 0;
            if (dictCache[currencyPair] != null)
            {
                if (service.CheckTimeStamp(currentTimeStamp,dictCache[currencyPair].timestamp) == true)
                {
                    ConversionResponseModel conversionResponseModel = new ConversionResponseModel();
                    sum = currencyModel.amount * decimal.Parse(dictCache[currencyPair].quote);
                    conversionResponseModel.result = sum;
                    DateTime date = DateTime.Now;
                    string dateToChange = date.ToString();
                    DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(dateToChange);
                    long unixTimeStamp = dateTimeOffset.ToUnixTimeMilliseconds();
                    conversionResponseModel.timeStamp = unixTimeStamp;
                    return Ok(conversionResponseModel);
                }
                else
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
                    return Ok(responseModel);
                }
            }
            else
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
                dictCache[currencyPair] = new CachingModel { quote = quote.ToString(), timestamp = unixTimeStamp};
                return Ok(responseModel);
            }
            
        }

        [HttpGet]
        [Route("/convertWithHistoricalDate")]
        public async Task<ActionResult<string>> getHistoricalCurrencies([FromQuery] CurrencyModel model)
        {
            HttpClient httpClient = new HttpClient();
            ConversionResponseModel responseModel = new ConversionResponseModel();
            var response = await httpClient.GetAsync("https://api.currencylayer.com/historical?date=" + model.time + "&source=" + model.fromCurr + "&access_key=96c7a03cce11e464756d645302fbb324");
            var responseString = await response.Content.ReadAsStringAsync();
            var key = model.fromCurr + model.toCurr;
            JObject jObject = JObject.Parse(responseString);
            var currencies = jObject.SelectToken("quotes");
            var currencyString = currencies.SelectToken(key).ToString();
            responseModel.result = model.amount * Decimal.Parse(currencyString);
            responseModel.result = Math.Round(responseModel.result, 2);
            responseModel.timeStamp = (long.Parse((jObject.SelectToken("timestamp")).ToString()))*1000;
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(responseModel.timeStamp);
            DateTime date = dateTimeOffset.DateTime;
            return Ok(responseModel);
        }

        [HttpGet]
        [Route("/percent")]
        public async Task<ActionResult<string>> getPercent()
        {
            HttpClient httpClient = new HttpClient();
            String fromCurr = "RSD";
            DateTime date = DateTime.Now;
            DateTime dateBefore = date.AddDays(-1);
            string dateBeforeString = dateBefore.ToString("yyyy-MM-dd");
            string dateString = date.ToString("yyyy-MM-dd");
            var response = await httpClient.GetAsync("https://api.currencylayer.com/timeframe?source=" + fromCurr + "&start_date=" + dateBeforeString + "&end_date=" + dateString + "&access_key=96c7a03cce11e464756d645302fbb324");
            var responseString = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseString);
            var currencies = jObject.SelectToken("quotes");
            //EUR USD JPY
            string rsdeur = (Decimal.Parse(currencies.SelectToken(dateString).SelectToken("RSDEUR").ToString()) * 100 / Decimal.Parse(currencies.SelectToken(dateBeforeString).SelectToken("RSDEUR").ToString())).ToString();
            string rsdusd = (Decimal.Parse(currencies.SelectToken(dateString).SelectToken("RSDUSD").ToString()) * 100 / Decimal.Parse(currencies.SelectToken(dateBeforeString).SelectToken("RSDUSD").ToString())).ToString();
            string rsdjpy = (Decimal.Parse(currencies.SelectToken(dateString).SelectToken("RSDJPY").ToString()) * 100 / Decimal.Parse(currencies.SelectToken(dateBeforeString).SelectToken("RSDJPY").ToString())).ToString();
            string result = rsdeur + ";" + rsdusd + ";" + rsdjpy;
            return Ok(result);

        }

        [HttpGet]
        [Route("/GetRSDQuotes")]
        public async Task<ActionResult<string>> GetRSDQuotes([FromQuery] string toCurr)
        {
            HttpClient httpClient = new HttpClient();
            DateTime dateTime = DateTime.Now;
            string date = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
            string url = "https://api.currencylayer.com/historical?date=" + date + "&source=" + toCurr + "&access_key="+_ACCESSKEY;
            var response = await httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseString);
            var quotes = jObject.SelectToken("quotes");
            return Ok(quotes.ToString());
        }

        [HttpGet]
        [Route("/getParamsForMonth")]
        public async Task<ActionResult<string>> getParamsForMonth([FromQuery] string fromCurr)
        {
            HttpClient httpClient = new HttpClient();
            String toCurr = "RSD";
            DateTime date = DateTime.Now;
            DateTime dateBefore = date.AddDays(-31);
            string dateBeforeString = dateBefore.ToString("yyyy-MM-dd");
            string dateString = date.ToString("yyyy-MM-dd");
            var response = await httpClient.GetAsync("https://api.currencylayer.com/timeframe?source=" + fromCurr + "&start_date=" + dateBeforeString + "&end_date=" + dateString + "&access_key=96c7a03cce11e464756d645302fbb324");
            var responseString = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseString);
            var currencies = jObject.SelectToken("quotes");
            List<DateCurrencyModel> result = new List<DateCurrencyModel>();
            for(int i = 0; i < 31; i++)
            {
                DateCurrencyModel dm = new DateCurrencyModel();
                DateTime pomocniDate = date.AddDays(-i);
                string pomocniDateString = pomocniDate.ToString("yyyy-MM-dd");
                string curr = currencies.SelectToken(pomocniDateString).SelectToken(fromCurr + toCurr).ToString();
                dm.currency = Decimal.Parse(curr);
                dm.date = pomocniDateString;
                result.Add(dm);
            }
            return Ok(result);
        }

    }
}
