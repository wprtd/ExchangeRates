using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace ExchangeRates.Models
{
    public class CurrencyContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Rate> Rates { get; set; }

        public CurrencyContext() : base("CurrencyConnection")
        {
            Database.SetInitializer(new DbInit());
        }
    }
    class DbInit : DropCreateDatabaseIfModelChanges<CurrencyContext>
    {
        protected override void Seed(CurrencyContext db)
        {
            base.Seed(db);

            Currency c1 = new Currency { Code = "USD", Description = "Доллар США" };
            Currency c2 = new Currency { Code = "EUR", Description = "Евро" };
            Currency c3 = new Currency { Code = "CNY", Description = "CNY" };

            db.Currencies.AddRange(new List<Currency> { c1, c2, c3 });

            Rate r1 = new Rate { CurrencyRate = 62, Currency = c1, Date = new DateTime(2018, 05, 26) };
            Rate r2 = new Rate { CurrencyRate = 61, Currency = c1, Date = new DateTime(2018, 05, 25) };
            Rate r3 = new Rate { CurrencyRate = 62.50, Currency = c1, Date = new DateTime(2018, 05, 27) };
            Rate r4 = new Rate { CurrencyRate = 72.95, Currency = c2, Date = new DateTime(2018, 05, 25) };
            Rate r5 = new Rate { CurrencyRate = 71.45, Currency = c2, Date = new DateTime(2018, 05, 26) };
            Rate r6 = new Rate { CurrencyRate = 74, Currency = c2, Date = new DateTime(2018, 05, 27) };
            Rate r7 = new Rate { CurrencyRate = 73.22, Currency = c2, Date = new DateTime(2018, 05, 28) };
            Rate r8 = new Rate { CurrencyRate = 73.98, Currency = c2, Date = new DateTime(2018, 05, 29) };
            Rate r9 = new Rate { CurrencyRate = 22, Currency = c3, Date = new DateTime(2018, 05, 26) };

            db.Rates.AddRange(new List<Rate> { r1, r2, r3, r4, r5,r6,r7,r8,r9 });

            db.SaveChanges();
        }
    }
}