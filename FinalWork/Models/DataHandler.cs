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
    public class DataHandler
    {
        private static DataHandler s_instace = null;
        private static readonly object padLock = new object();
        
        public static DataHandler Instance
        {
            get
            {
                lock (padLock)
                {
                    if (s_instace == null)
                    {
                        s_instace = new DataHandler();
                    }
                    return s_instace;
                }
                
            }
        }
        private int index = -1;
        private string fileName = "";
        private XmlDocument doc = new XmlDocument();
        private List<Data> dataList = new List<Data>();

        public string filePath = "";           
        public Data Data { get; private set; }
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                filePath = string.Format("~/App_Data/{0}.xml", value);
            }
        }
        public string Ip { get; set; }
        public int Port { get; set; }
        public int Time { get; set; }
        public int RefreshRate { get; set; }
        public string Path { get; set; }
        public int Index
        {
            set
            {
                index = value;
            }
        }

        public DataHandler()
        {
            Data = new Data();
        }
        // adds the data to the list.
        public void SaveData(Data data)
        {
            dataList.Add(data);
        }
      
        // return the next data from the list
        public Data GetNextData()
        {
            if (dataList.Count != 0 && index + 1 < dataList.Count)
            {
                ++index;
                return dataList[index];
            }
            return null;
        }
        // return the data in the form of xml.
        public string ToXml(Data data)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("AllData");

            WriteDataToXml(writer, data);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

        //go throught elemnts to XML
        public void SaveToFile()
        {
            string path = HttpContext.Current.Server.MapPath((filePath));
            XmlWriterSettings settings = new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            XmlWriter writer = XmlWriter.Create(path, settings);

            writer.WriteStartElement("AllData");
            foreach (Data data in dataList)
            {
                WriteDataToXml(writer, data);
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }
        // gets the values from the xml file.
        public void ReadDataFromFile()
        {
            string path = HttpContext.Current.Server.MapPath((filePath));
            if (File.Exists(path))
            {
                //load file from path
                var document = XDocument.Load(path);
                var elements = document.Descendants("Data");
                // iterate through the child elements
                foreach (XElement node in elements)
                {
                    Data data = new Data();
                    data.Location.Lat = double.Parse(node.Descendants("Lat").Single().Value);
                    data.Location.Lon = double.Parse(node.Descendants("Lon").Single().Value);
                    data.Rudder = double.Parse(node.Descendants("Rudder").Single().Value);
                    data.Throttle = double.Parse(node.Descendants("Throttle").Single().Value);
                    // adds the data from the xml to the list.
                    SaveData(data);
                }
            }
        }

        // write the data to the xml by using the given xml writer.
        private void WriteDataToXml(XmlWriter writer, Data data)
        {
            writer.WriteStartElement("Data");
            writer.WriteElementString("Lat", data.Location.Lat.ToString());
            writer.WriteElementString("Lon", data.Location.Lon.ToString());
            writer.WriteElementString("Throttle", data.Throttle.ToString());
            writer.WriteElementString("Rudder", data.Rudder.ToString());
            writer.WriteEndElement();
        }
    }
}