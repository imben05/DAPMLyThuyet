using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightManagement.Models
{
    public class SearchHotel
    {
        public string NameHotel { get; set; }
        public string Location { get; set; }
        public int? StarRange { get; set; }
        public decimal? MaxRoomPrice { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public List<Hotel> Hotels { get; set; } // Danh sách khách sạn tìm thấy
    }
}