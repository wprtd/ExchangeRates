using Newtonsoft.Json;
using System;

namespace ExchangeRates.Models
{
    public class Rate
    {
        public int Id { get; set; }
        public double CurrencyRate { get; set; }
        public DateTime Date { get; set; }

        public int CurrencyId { get; set; }
        [JsonIgnore]
        public Currency Currency { get; set; }
    }
}