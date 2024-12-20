//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FlightManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hotel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Hotel()
        {
            this.BookingHotels = new HashSet<BookingHotel>();
        }
    
        public int IdHotel { get; set; }
        public string NameHotel { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Nullable<int> StarRange { get; set; }
        public string Decripstion { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Location { get; set; }
        public Nullable<decimal> RoomPrice { get; set; }
        public Nullable<int> RoomCount { get; set; }
        public Nullable<System.DateTime> depatureTime { get; set; }
        public Nullable<System.DateTime> returnTime { get; set; }
        public string HinhAnh { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingHotel> BookingHotels { get; set; }
    }
}
