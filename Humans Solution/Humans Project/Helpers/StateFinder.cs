using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Humans.Web.Models;

namespace Humans.Web.Helpers
{
    public class StateFinder
    {
        public static string GetStateName(int stateId)
        {
            HumansEntities db = new HumansEntities();

            State st = db.States.SingleOrDefault(s => s.Id == stateId);

            return st == null ? string.Empty : st.Name;
        }
    }
}