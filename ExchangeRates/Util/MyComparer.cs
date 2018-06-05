using ExchangeRates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExchangeRates.Util
{
    class MyComparer : IEqualityComparer<Rate>
    {
        public bool Equals(Rate x, Rate y)
        {
            return x.Date == y.Date;
        }

        public int GetHashCode(Rate obj)
        {
            return obj.Date.GetHashCode();
        }
    }
}