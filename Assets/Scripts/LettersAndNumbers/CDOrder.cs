using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conditions
{

    public class CDOrder : ConditionObj
    {
        public CDOrder(string condition, string locID, string orderID, int sum, string condID)
        : base(condition, new string[] { locID }, new string[] { orderID }, locID + orderID + sum + condID)
        {

        }
    }
}

