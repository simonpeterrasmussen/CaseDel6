using System.Text.Json;

namespace ElprisProducer
{
    internal class PriceParser
    {
        private readonly PriceModel model;

        public PriceParser(string json)
        {
            PriceModel? priceModel = JsonSerializer.Deserialize<PriceModel>(json);
            if (priceModel == null)
            {
                throw new NullReferenceException("Json parses to null model");
            }
            this.model = priceModel;            
        }

        private double? GetPrice(string area, DateTime hour)
        {
            foreach(var price in model.data.elspotprices)
            {
                if (price.HourDK == hour && price.PriceArea == area)
                {
                    return Math.Round(price.SpotPriceDKK / 1000, 2);
                }
            }
            return null;
        }

        /// <summary>
        /// Henter prisen vest for Storebælt
        /// </summary>
        /// <param name="hour">Dato plus timetal for starttidspunktet. Minutter og sekunder skal være sat til 0</param>
        /// <returns>Prisen per kWh i dkk. null hvis der ikke er en pris for det pågældende tidspunkt</returns>
        public double? GetWestPrice(DateTime hour)
        {
            return GetPrice("DK1", hour);
        }

        /// <summary>
        /// Henter prisen øst for Storebælt
        /// </summary>
        /// <param name="hour">Dato plus timetal for starttidspunktet. Minutter og sekunder skal være sat til 0</param>
        /// <returns>Prisen per kWh i dkk. null hvis der ikke er en pris for det pågældende tidspunkt</returns>
        public double? GetEastPrice(DateTime hour)
        {
            return GetPrice("DK2", hour);
        }

    }


    public class PriceModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles
        public Data data { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class Data
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles
        public Elspotprice[] elspotprices { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class Elspotprice
    {
        public DateTime HourUTC { get; set; }
        public DateTime HourDK { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string PriceArea { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public float SpotPriceDKK { get; set; }
        public float SpotPriceEUR { get; set; }
    }

}
