//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BitsmackGTAPI
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pedometer
    {
        public int id { get; set; }
        public int steps { get; set; }
        public int sleep { get; set; }
        public System.DateTime trandate { get; set; }
        public double weight { get; set; }
        public double bodyfat { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<System.DateTime> lastupdateddate { get; set; }
        public Nullable<int> calconsumed { get; set; }
    }
}
