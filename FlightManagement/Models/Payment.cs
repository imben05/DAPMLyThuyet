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
    
    public partial class Payment
    {
        public int paymentID { get; set; }
        public decimal amount { get; set; }
        public System.DateTime paymentDate { get; set; }
        public string paymentMethod { get; set; }
        public string status { get; set; }
        public Nullable<int> bookingID { get; set; }
    
        public virtual Booking Booking { get; set; }
    }
}
