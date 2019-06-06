
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalWork.Models;


namespace FinalWork.Controllers
{

    public class MainController : Controller
    {
        // default case
        public ActionResult Index()
        {
            return View();
        }


        // displays the location of the plane on the map, or redirects to the the file case.
        [HttpGet]
        public ActionResult Display(string ip, int port)
        {

            try
            {
                // try to parse the ip, if it fails goto different action.
                IPAddress.Parse(ip);
                Connection c = Connection.Instance;
                // disconnect if connected
                if (c.IsConnected || c.IsWaiting)
                {
                    c.CloseConnection();
                }
                c.Connect(ip, port);
                c.ReadData();
                Location location = c.GetLocation;
                UpdateSessionDisplay(location.Lon, location.Lat);
                Session["stop"] = 1;
                return View();
            }
            catch
            {
                return RedirectToAction("LoadAndDisplay", new { fileName = ip, refreshRate = port });
            }
        }
        [HttpGet]
        public ActionResult LoadAndDisplay(string fileName, int refreshRate)
        {
            DataHandler dh = DataHandler.Instance;
            dh.FileName = fileName;
            dh.ReadDataFromFile();
            dh.Index = -1;
            Data data = DataHandler.Instance.GetNextData();
            if (data == null)
            {
                Session["stop"] = 1;
                return View();
            }
            Location location = data.Location;
            UpdateSessionDisplay(location.Lon, location.Lat, refreshRate);
            Session["stop"] = 0;
            return View();
        }


        // displays the location of the plane on the map and draws his path, by sampling every 
        // 4 seconds his location.
        [HttpGet]
        public ActionResult DisplayWithRoute(string ip, int port, int refreshRate)
        {
            Connection c = Connection.Instance;
            // disconnect if connected
            if (c.IsConnected || c.IsWaiting)
            {
                c.CloseConnection();
            }
            c.Connect(ip, port);
            c.ReadData();
            Location location = c.GetLocation;
            UpdateSessionDisplay(location.Lon, location.Lat, refreshRate);
            return View();
        }


        // samples the location of the flight at given(refreshRate) rate for given(duration) time, 
        // and saves the data in a file in a given name(fileName)
        [HttpGet]
        public ActionResult SaveAndDisplay(string ip, int port, int refreshRate, int duration, string fileName)
        {
            Connection c = Connection.Instance;
            // disconnect if connected
            if (c.IsConnected)
            {
                c.CloseConnection();
            }
            c.Connect(ip, port);
            c.ReadData();
            Location location = c.GetLocation;
            UpdateSessionDisplay(location.Lon, location.Lat, refreshRate, duration);
            DataHandler.Instance.FileName = fileName;
            return View();
        }

        // returns the next data to the view.
        [HttpPost]
        public string GetData()
        {
            DataHandler dh = DataHandler.Instance;
            Connection c = Connection.Instance;
            Data data = new Data();
            if (c.IsConnected)
            {
                c.ReadData();
                data.Location.Lat = c.GetLocation.Lat;
                data.Location.Lon = c.GetLocation.Lon;
                data.Rudder = c.Rudder;
                data.Throttle = c.Throttle;
                if (dh.FileName != "")
                    dh.SaveData(data);

                return dh.ToXml(data);
            }
            // if the file is empty(no file created), return null;
            if (dh.FileName == "") return null;

            data = dh.GetNextData();
            if (data == null)
            {
                Session["stop"] = 1;
                return null;
            }
            return DataHandler.Instance.ToXml(data);
        }
        [HttpPost]
        public void SaveToFile()
        {
            DataHandler.Instance.SaveToFile();
        }
        // updates the session values.
        private void UpdateSessionDisplay(double lon, double lat, int refreshRate = 0, int duration = 0)
        {
            Session["Lon"] = lon;
            Session["Lat"] = lat;
            Session["RefreshRate"] = refreshRate;
            Session["Duration"] = duration;
        }

    }
}