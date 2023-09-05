using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conditions
{

    public class CDConditional : ConditionObj
    {
        public CDConditional(string condition, string locID, string attrID1, int sum1, string condID1, string attrID2, int sum2, string condID2, string condID)
        : base(condition, new string[] { locID }, new string[] { attrID1, attrID2 }, locID + attrID1 + sum1 + condID1 + attrID2 + sum2 + condID2 + condID)
        {

        }
    }
}

