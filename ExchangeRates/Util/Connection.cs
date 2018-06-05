using ExchangeRates.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Http.Cors;
using System.Xml.Linq;

namespace ExchangeRates.Util
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class Connection
    {
        private const string urlCbr = "http://www.cbr.ru/scripts/";

        public double GetRateCurrency(string _Currency, string date)
        {
            string url = String.Format(urlCbr + "XML_daily.asp?date_req={0}", date);
            WebClient client = new WebClient();
            XDocument xdoc = XDocument.Load(client.OpenRead(url));

            var items = from xe in xdoc.Element("ValCurs").Elements("Valute")
                        select new
                        {
                            Code = xe.Element("CharCode").Value,
                            CurrencyRate = Convert.ToDouble(xe.Element("Value").Value)
                        };
            var currency = items.Where(lq => lq.Code == _Currency).FirstOrDefault();

            return currency.CurrencyRate;
        }

        public string GetParentCode(string _Currency, string file_path)
        {
            string url = String.Format(file_path);
            WebClient client = new WebClient();
            XDocument xdoc = XDocument.Load(client.OpenRead(url));

            var items = from xe in xdoc.Element("Valuta").Elements("Item")
                        where xe.Element("ISO_Char_Code").Value == _Currency
                        select new
                        {
                            ParentCode = xe.Element("ParentCode").Value
                        };

            var ParentCode = items.FirstOrDefault();

            return ParentCode.ParentCode;
        }

        public List<Rate> GetRangeOfValues(string _ParentCode, string dtFrom, string dtTo, int id)
        {
            string url = String.Format("http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1=" + dtFrom + "&date_req2=" + dtTo + "&VAL_NM_RQ=" + _ParentCode);
            WebClient client = new WebClient();
            XDocument xdoc = XDocument.Load(client.OpenRead(url));

            var items = from xe in xdoc.Element("ValCurs").Elements("Record")
                        select new Rate
                        {
                            CurrencyId = id,
                            Date = new DateTime(Int32.Parse(xe.Attribute("Date").Value.Split('.')[2]), Int32.Parse(xe.Attribute("Date").Value.Split('.')[1]), Int32.Parse(xe.Attribute("Date").Value.Split('.')[0])),
                            CurrencyRate = Convert.ToDouble(xe.Element("Value").Value)
                        };

            return items.ToList();
        }
    }
}