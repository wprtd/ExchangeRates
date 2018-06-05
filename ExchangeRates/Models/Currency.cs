using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExchangeRates.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        
        [JsonIgnore]
        public IEnumerable<Rate> Rates { get; set; }
        
        public Currency()
        {
            Rates = new List<Rate>();
        }
    }
}