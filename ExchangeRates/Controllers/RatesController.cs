using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Xml.Linq;
using ExchangeRates.Models;
using ExchangeRates.Util;

namespace ExchangeRates.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RatesController : ApiController
    {
        private CurrencyContext db = new CurrencyContext();

        // GET: api/Rates/2018-05-25/2018-05-31

        public IHttpActionResult GetRates(int id, string dtStart, string dtEnd)
        {
            var dt1 = dtStart.Split('-');
            var dt2 = dtEnd.Split('-');

            DateTime _dtStart = new DateTime(Int32.Parse(dt1[2]), Int32.Parse(dt1[1]), Int32.Parse(dt1[0]));
            DateTime _dtEnd = new DateTime(Int32.Parse(dt2[2]), Int32.Parse(dt2[1]), Int32.Parse(dt2[0])); ;

            var result = db.Rates.Where(lq => lq.CurrencyId == id && lq.Date >= _dtStart && lq.Date <= _dtEnd).ToList();

            Connection connection = new Connection();
            Currency c = db.Currencies.Where(lq => lq.Id == id).FirstOrDefault();

            string file_path = "http://www.cbr.ru/scripts/XML_valFull.asp";
            string ParentCode = connection.GetParentCode(c.Code, file_path);
            var elements = connection.GetRangeOfValues(ParentCode, dtStart, dtEnd, id);

            if (result != null)
            {
                db.Rates.RemoveRange(result);
            }
            if (elements != null)
            {
                db.Rates.AddRange(elements);
            }

            db.SaveChanges();
            var res = db.Rates.Where(lq => lq.CurrencyId == id && lq.Date >= _dtStart && lq.Date <= _dtEnd).ToList();
            if (res == null)
            {
                bool error = true;
                return Json(error);
            }

            return Json(res);

        }


        // GET: api/Rates/5/2018-05-31
        [ResponseType(typeof(Rate))]
        public async Task<IHttpActionResult> GetRate(int id, string dtStart)
        {
            DateTime _dateTime = DateTime.Parse(dtStart.Replace('-', '.'));
            Rate rate = await db.Rates.Where(lq => lq.CurrencyId == id && lq.Date == _dateTime).FirstOrDefaultAsync();
            if (rate == null)
            {
                Currency currency = db.Currencies.Find(id);
                if (currency != null)
                {
                    string[] dateArray = dtStart.Split('-');
                    dateArray[1] = Int32.Parse(dateArray[1]) > 9 ? dateArray[1] : "0" + dateArray[1];
                    dateArray[2] = Int32.Parse(dateArray[2]) > 9 ? dateArray[2] : "0" + dateArray[2];
                    Array.Reverse(dateArray);
                    string dateFormat = String.Join("/", dateArray);

                    try
                    {
                        Connection connection = new Connection();
                        double CurrencyRate = connection.GetRateCurrency(currency.Code, dateFormat);

                        rate = new Rate
                        {
                            CurrencyId = id,
                            CurrencyRate = Convert.ToDouble(CurrencyRate),
                            Date = DateTime.Parse(dtStart)
                        };

                        db.Rates.Add(rate);
                        db.SaveChanges();

                        return Ok(rate);
                    }
                    catch
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok(rate);
        }

        // PUT: api/Rates/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRate(int id, Rate rate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rate.Id)
            {
                return BadRequest();
            }

            db.Entry(rate).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Rates
        [ResponseType(typeof(Rate))]
        public async Task<IHttpActionResult> PostRate(Rate rate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Rates.Add(rate);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = rate.Id }, rate);
        }

        // DELETE: api/Rates/5
        [ResponseType(typeof(Rate))]
        public async Task<IHttpActionResult> DeleteRate(int id)
        {
            Rate rate = await db.Rates.FindAsync(id);
            if (rate == null)
            {
                return NotFound();
            }

            db.Rates.Remove(rate);
            await db.SaveChangesAsync();

            return Ok(rate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RateExists(int id)
        {
            return db.Rates.Count(e => e.Id == id) > 0;
        }
    }
}