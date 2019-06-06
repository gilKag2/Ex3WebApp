using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FinalWork.Models
{
    //singleton.
    public sealed class Connection
    {

        private string lonPath = "position/longitude-deg";
        private string latPath = "position/latitude-deg";
        private string rudderPath = "/controls/flight/rudder";
        private string throttlePath = "/controls/engines/current-engine/throttle";

        private double _rudder;
        public double Rudder
        {
            get { return _rudder; }
            set { _rudder = value; }
        }
        private double _throttle;
        public double Throttle
        {
            get { return _throttle; }
            set { _throttle = value; }
        }

        private Location location;
        public Location GetLocation
        {
            get { return location; }
        }
        volatile private TcpClient _client;
        private bool _isConnected;
        private bool _isWaiting;
        IPEndPoint ep;
        private static Connection _instance = null;
        private static readonly object padLock = new object();
        public static Connection Instance
        {
            get
            {
                lock (padLock)
                {
                    if (_instance == null)
                        _instance = new Connection();
                    return _instance;
                }
            }
        }
        public Connection()
        {
            _isWaiting = false;
            _isConnected = false;
            location = new Location();
        }
        public bool IsWaiting
        {
            get { return _isWaiting; }
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
        }
        public void Connect(string ip, int port)
        {
            ep = new IPEndPoint(IPAddress.Parse(ip), port);
            _client = new TcpClient();
            // try to connect until sucess.
            while (!_client.Connected)
            {
                _isWaiting = true;
                try
                {
                    _client.Connect(ep);
                }
                catch (SocketException) { }
            }
            _isConnected = true;
        }

        public void ReadData()
        {
            location.Lon = GetValueFromServer(lonPath);
            location.Lat = GetValueFromServer(latPath);
            Throttle = GetValueFromServer(throttlePath);
            Rudder = GetValueFromServer(rudderPath);     
        }

        public double GetValueFromServer(string path)
        {
            using (NetworkStream stream = _client.GetStream())
            {
                try
                {
                    
                    string requestValue = "get" + " " + path + "\r\n";
                    byte[] data = ASCIIEncoding.ASCII.GetBytes(requestValue);

                    stream.Write(data, 0, data.Length);
                    
                    StreamReader reader = new StreamReader(stream);
                    string value = reader.ReadLine().Split('\'')[1];
                    return Convert.ToDouble(value);
                }
                catch (SocketException) { };

            }
            return 0;
        }

        public void CloseConnection()
        {
            if (_client != null && IsConnected)
            {

                _client.Close();
                _isConnected = false;
                _isWaiting = false;
            }
        }
    }
}
