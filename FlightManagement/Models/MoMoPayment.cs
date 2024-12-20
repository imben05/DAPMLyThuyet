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
    
    public partial class MoMoPayment
    {
        public int PaymentID { get; set; }
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
        public string RedirectUrl { get; set; }
        public string IpnUrl { get; set; }
        public string RequestType { get; set; }
        public string Signature { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<int> bookingID { get; set; }
    
        public virtual Booking Booking { get; set; }
    }
}
