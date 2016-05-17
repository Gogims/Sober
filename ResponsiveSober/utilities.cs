using SoberModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication4
{
    public class utilities
    {
        public static string GetStateNameFromAbbrevation(string stateAbbr)
        {
            SoberMVCEntities context = new SoberModel.SoberMVCEntities();
            var stateName = (from state in context.sober_States
                             where state.StateAbbr == stateAbbr ||state.StateName==stateAbbr
                             select state.StateName).FirstOrDefault();//.ToString();
            // if (stateName == null)
            //     throw new Exception("State Abbreviation is not valid. No state name found for that abbreviation");
            return stateName;
        }
    }
}