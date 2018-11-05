using System;
using System.Globalization;
using System.Linq;
using SpeedTest;
using SpeedTest.Models;

namespace SpeedTestLogger
{
    public class SpeedTestRunner
    {
        private readonly SpeedTestClient _client;
        private readonly Settings _settings;
        private readonly RegionInfo _location;

        public SpeedTestRunner(RegionInfo location)
        {
            _client = new SpeedTestClient();
            _settings = _client.GetSettings();
            _location = location;
        }

        public void RunSpeedTest()
        {
            Console.WriteLine("Finding best test servers");
            var server = FindBestTestServer();

            Console.WriteLine("Testing download speed from {0} in {1}", server.Host, server.Name);
            var downloadSpeed = TestDownloadSpeed(server);
            Console.WriteLine("Download speed was: {0} Mbps", downloadSpeed);

            Console.WriteLine("Testing upload speed");
            var uploadSpeed = TestUploadSpeed(server);
            Console.WriteLine("Upload speed was: {0} Mbps", uploadSpeed);
        }
        private Server FindBestTestServer()
        {
            var tenLocalServers = _settings.Servers
                .Where(s => s.Country.Equals(_location.EnglishName))
                .Take(10);

            var serversOrdersByLatency = tenLocalServers
                .Select(s =>
                {
                    s.Latency = _client.TestServerLatency(s);
                    return s;
                })
                .OrderBy(s => s.Latency);
            
            return serversOrdersByLatency.First();
        }

        private double TestDownloadSpeed(Server server)
        {
            var downloadSpeed = _client.TestDownloadSpeed(server, _settings.Download.ThreadsPerUrl);
            return ConvertSpeedToMbps(downloadSpeed);
        }

        private double TestUploadSpeed(Server server)
        {
            var uploadSpeed = _client.TestUploadSpeed(server, _settings.Upload.ThreadsPerUrl);
            return ConvertSpeedToMbps(uploadSpeed);
        }

        private double ConvertSpeedToMbps(double speed)
        {
            return Math.Round(speed / 1024, 2);
        }
    }
}