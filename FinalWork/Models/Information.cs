using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace FinalWork.Models
{
    public class Information
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Throttle { get; set; }
        public double Rudder { get; set; }

        // write by XML writer the information vars
        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Information");
            writer.WriteElementString("Lat", this.Lat.ToString());
            writer.WriteElementString("Lon", this.Lon.ToString());
            writer.WriteElementString("Throttle", this.Throttle.ToString());
            writer.WriteElementString("Rudder", this.Rudder.ToString());
            writer.WriteEndElement();
        }
    }
}