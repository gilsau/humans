using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Humans.Web.Helpers
{
    public static class GeoCodeCalc
    {
        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;

        public static double ToRadian(double val) { return val * (Math.PI / 180); }
        public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }

        public static double CalcDistance(double? lat1=0, double? lng1=0, double? lat2=0, double? lng2=0)
        {
            double lat1a = double.Parse(lat1.ToString());
            double lat2a = double.Parse(lat2.ToString());
            double lng1a = double.Parse(lng1.ToString());
            double lng2a = double.Parse(lng2.ToString());

            return CalcDistance(lat1a, lng1a, lat2a, lng2a, GeoCodeCalcMeasurement.Miles);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
        {
            double radius = GeoCodeCalc.EarthRadiusInMiles;

            if (m == GeoCodeCalcMeasurement.Kilometers) { radius = GeoCodeCalc.EarthRadiusInKilometers; }
            return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }
    }

    public enum GeoCodeCalcMeasurement : int
    {
        Miles = 0,
        Kilometers = 1
    }
}