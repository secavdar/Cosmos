using System;

namespace Cosmos.Domain.Model
{
    public class LogEntry
    {
        public string AppUser { get; set; }
        public string Machine { get; set; }
        public string RequestIpAddress { get; set; }
        public string RequestContentType { get; set; }
        public string RequestContentBody { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public string RequestHeaders { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public string ResponseContentType { get; set; }
        public string ResponseContentBody { get; set; }
        public string ResponseStatusCode { get; set; }
        public string ResponseHeaders { get; set; }
        public DateTime ResponseTimestamp { get; set; }
    }
}