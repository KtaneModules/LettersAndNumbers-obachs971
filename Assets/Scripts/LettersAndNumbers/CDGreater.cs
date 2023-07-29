using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conditions
{

    public class CDGreater : ConditionObj
    {
        public CDGreater(string condition, string locID, string attrID1, string attrID2, string condID)
        : base(condition, new string[] { locID }, new string[] { attrID1, attrID2 }, locID + attrID1 + locID + attrID2 + condID)
        {

        }
        public CDGreater(string condition, string locID1, string attrID1, string locID2, string attrID2, string condID)
        : base(condition, new string[] { locID1, locID2 }, new string[] { attrID1, attrID2 }, locID1 + attrID1 + locID2 + attrID2 + condID)
        {

        }
    }
}
