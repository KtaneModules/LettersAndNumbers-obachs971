using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conditions
{

    public class CDEven : ConditionObj
    {
        public CDEven(string condition, string locID, string attrID, string condID)
        : base(condition, new string[] { locID }, new string[] { attrID }, locID + attrID + locID + condID)
        {

        }
        public CDEven(string condition, string locID1, string attrID, string locID2, string condID)
        : base(condition, new string[] { locID1, locID2 }, new string[] { attrID }, locID1 + attrID + locID2 + condID)
        {

        }
    }
}
