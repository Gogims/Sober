using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SoberModel
{
    public class Insurance
    {
        public int Ins_ID { get; set; }
        [Display(Name="Provider")]
        [Required]
        public string Ins_ProviderName { get; set; }
        [Display(Name = "Logo")]
        [Required]
        public string Ins_ProviderLogo { get; set; }
        [AllowHtml]
        public string Ins_AboutUs { get; set; }

        public int total { get; set; }

        public Insurance() { }

        public Insurance(string name)
        {
            using(DBManager context = new DBManager())
            {
                var ins = (from insurance in context.Insurance_Providers
                           where insurance.Ins_ProviderName == name
                           select insurance).FirstOrDefault();

                if (ins != null)
                {
                    Ins_ID = ins.Ins_ID;
                    Ins_ProviderLogo = ins.Ins_ProviderLogo;
                    Ins_ProviderName = ins.Ins_ProviderName;
                    Ins_AboutUs = ins.Ins_AboutUs;
                }
            }           
        }

        public Insurance(int id)
        {
            using (DBManager context = new DBManager())
            {
                var ins = context.Insurance_Providers.Find(id);

                if (ins != null)
                {
                    Ins_ID = ins.Ins_ID;
                    Ins_ProviderLogo = ins.Ins_ProviderLogo;
                    Ins_ProviderName = ins.Ins_ProviderName;
                    Ins_AboutUs = ins.Ins_AboutUs;
                }
            }
        }

        public List<Insurance> GetTopInsurance(int amount)
        {
            List<Insurance> tops = new List<Insurance>();

            using (DBManager context = new DBManager())
            {
                var insurances = context.Insurance_Providers.OrderByDescending(x => x.Treatment_Center_Survey.Count).Take(amount).ToList();

                foreach (var item in insurances)
                {
                    Insurance i = new Insurance();
                    i.total = item.Treatment_Center_Survey.Count;
                    i.Ins_ProviderName = item.Ins_ProviderName;
                    i.Ins_ProviderLogo = item.Ins_ProviderLogo;
                    i.Ins_ID = item.Ins_ID;

                    tops.Add(i);
                }
            }            

            return tops;
        }

        public List<State> GetStatesWithInsurance()
        {
            List<State> states = new List<State>();
            Insurance_Providers insuranceObject;

            using (DBManager context = new DBManager())
            {
                insuranceObject = context.Insurance_Providers.Where(x => x.Ins_ProviderName.ToLower().Trim() == Ins_ProviderName.ToLower().Trim()).First();

                states = (from tc in context.Treatment_Center_Survey
                            join enhanced in context.EnhancedProfileLists on tc.User_Id equals enhanced.User_Id
                            join samhsa in context.tbl_SamhsaListings on enhanced.PageName equals samhsa.PageName
                            join state in context.sober_States on samhsa.location_state equals state.StateAbbr
                            where tc.Insurance_Providers.Select(x => x.Ins_ID).Contains(insuranceObject.Ins_ID) && enhanced.Survey == "TreatmentCenterSurvey"
                            group state by state.StateName into g
                            select new State { StateName = g.Key, Count = g.Count() }).Where(x => x.Count > 0).ToList();
            }

            return states;
        }

        public List<State> GetStatesAllInsurances()
        {
            List<State> states = new List<State>();

            using (DBManager context = new DBManager())
            {
                states = (from tc in context.Treatment_Center_Survey
                            join enhanced in context.EnhancedProfileLists on tc.User_Id equals enhanced.User_Id
                            join samhsa in context.tbl_SamhsaListings on enhanced.PageName equals samhsa.PageName
                            join state in context.sober_States on samhsa.location_state equals state.StateAbbr
                            where tc.Insurance_Providers.Count > 0 && enhanced.Survey == "TreatmentCenterSurvey"
                            group state by state.StateName into g
                            select new State { StateName = g.Key, Count = g.Count() }).Where(x => x.Count > 0).ToList();
            }

            return states;
        }

        public static List<Insurance_Providers> GetInsurances()
        {
            DBManager context = new DBManager();

            return context.Insurance_Providers.OrderBy(x => x.Ins_ProviderName).ToList();
        }
    }
}
