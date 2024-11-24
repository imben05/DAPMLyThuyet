using System;
using System.Collections.Generic;
using FlightManagement.Models;

namespace FlightManagement.Models
{
    public class SearchFlight
    {
        // Dữ liệu từ form tìm kiếm
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime? DepartureTime { get; set; }

        // Danh sách chuyến bay tìm được
        public List<Flight> Flights { get; set; }
    }
}
