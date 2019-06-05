using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalWork.Models
{
    public class Location
    {
        public Location() { }
        public Location(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }
        private double _lat;
        public double Lat
        {
            get { return _lat; }
            set { _lat = value; }
        }
        private double _lon;
        public double Lon
        {
            get { return _lon; }
            set { _lon = value; }
        }
    }
}