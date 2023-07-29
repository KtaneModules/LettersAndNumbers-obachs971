using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conditions
{

    public class CDFewer : ConditionObj
    {
        public CDFewer(string condition, string locID, string attrID, int sum, string condID)
        : base(condition, new string[] { locID }, new string[] { attrID }, locID + attrID + locID + sum + condID)
        {

        }
        public CDFewer(string condition, string locID1, string attrID, string locID2, int sum, string condID)
        : base(condition, new string[] { locID1, locID2 }, new string[] { attrID }, locID1 + attrID + locID2 + sum + condID)
        {

        }
    }
}
