
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
            
            try {
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
                return View("displayIpPort");
            }
            catch
            {
                return RedirectToAction("LoadAndDisplay", new { fileName = ip, refreshRate = port });
            }
        }
        [HttpGet]
        public ActionResult LoadAndDisplay(string fileName, int refreshRate)
        {
            Information info = new Information();
            InfoModel.Instance.FileName = fileName;
            InfoModel.Instance.ReadDataXML();
            InfoModel.Instance.Index = -1;
            info = InfoModel.Instance.GetInformation();
            if (info == null)
            {
                Session["stop"] = 1;
                return View();
            }
            UpdateSessionDisplay(info.Lon, info.Lat, refreshRate);
            Session["stop"] = 0;
            return View("display1");
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
            return View("display1");
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
            InfoModel.Instance.FileName = fileName;
            return View();
        }

        // reads the data and passes it to the view.
        [HttpPost]
        public string GetData()
        {
            var info = new Information();
            Connection c = Connection.Instance;
            if (c.IsConnected)
            {
                c.ReadData();
                info.Lat = c.GetLocation.Lat;
                info.Lon = c.GetLocation.Lon;
                info.Rudder = c.Rudder;
                info.Throttle = c.Throttle;
                if (InfoModel.Instance.FileName != "")
                {
                    InfoModel.Instance.RecordInfo(info);
                }
                return InfoModel.Instance.ToXml(info);
            }
            else if (InfoModel.Instance.FileName != "")
            {
                info = InfoModel.Instance.GetInformation();
                if (info == null)
                {
                    Session["stop"] = 1;
                    return null;
                }
                return InfoModel.Instance.ToXml(info);
            }
            return null;
        }

        private void UpdateSessionDisplay(double lon, double lat, int refreshRate=0, int duration=0)
        {
            Session["Lon"] = lon;
            Session["Lat"] = lat;
            Session["RefreshRate"] = refreshRate;
            Session["Duration"] = duration;
        }

        [HttpPost]
        public void RecordToFile()
        {
            InfoModel.Instance.RecordToFile();
        }
    }
}