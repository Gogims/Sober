//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoberModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Insurance_Providers
    {
        public Insurance_Providers()
        {
            this.Treatment_Center_Survey = new HashSet<Treatment_Center_Survey>();
        }
    
        public int Ins_ID { get; set; }
        public string Ins_ProviderName { get; set; }
        public string Ins_ProviderLogo { get; set; }
        public string Ins_AboutUs { get; set; }
    
        public virtual ICollection<Treatment_Center_Survey> Treatment_Center_Survey { get; set; }
    }
}
