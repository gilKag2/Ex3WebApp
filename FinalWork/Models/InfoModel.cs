using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace FinalWork.Models
{
    public class InfoModel
    {
        private static InfoModel s_instace = null;
        private XmlDocument doc = new XmlDocument();
        private string fileName = "";
        private List<Information> recorded = new List<Information>();
        private int index = -1;

        public static InfoModel Instance
        {
            get
            {
                if (s_instace == null)
                {
                    s_instace = new InfoModel();
                }
                return s_instace;
            }
        }

        public string SCENARIO_FILE = "";           // The Path of the Secnario
        public Information Information { get; private set; }
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                SCENARIO_FILE = string.Format("~/App_Data/{0}.xml", value);
            }
        }
        public string ip { get; set; }
        public int port { get; set; }
        public int time { get; set; }
        public int timesPerSec { get; set; }
        public string Path { get; set; }
        public int Index
        {
            set
            {
                index = value;
            }
        }


        public InfoModel()
        {
            Information = new Information();
        }
        //add informtion to the list
        public void RecordInfo(Information info)
        {
            recorded.Add(info);
        }
        //read from XML file
        public void ReadDataXML()
        {
            string path = HttpContext.Current.Server.MapPath((SCENARIO_FILE));
            if (File.Exists(path))
            {
                //load file from path
                var document = XDocument.Load(path);
                var elements = document.Descendants("Information");
                // iterate through the child elements
                foreach (XElement node in elements)
                {
                    Information info = new Information();
                    info.Lat = double.Parse(node.Descendants("Lat").Single().Value);
                    info.Lon = double.Parse(node.Descendants("Lon").Single().Value);
                    info.Rudder = double.Parse(node.Descendants("Rudder").Single().Value);
                    info.Throttle = double.Parse(node.Descendants("Throttle").Single().Value);
                    //add to list
                    RecordInfo(info);
                }
            }
        }
        //go throuth the list of informations
        public Information GetInformation()
        {
            if (recorded.Count != 0 && index + 1 < recorded.Count)
            {
                ++index;
                return recorded[index];
            }
            return null;
        }
        //write information data to XML
        public string ToXml(Information information)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("AllInformation");

            information.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
        //go throught elemnts to XML
        public void RecordToFile()
        {
            string path = HttpContext.Current.Server.MapPath((SCENARIO_FILE));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(path, settings);

            writer.WriteStartElement("AllInformation");
            foreach (Information info in recorded)
            {
                info.ToXml(writer);
            }
            writer.WriteEndElement();

            writer.Flush();
            writer.Close();
        }
    }
}