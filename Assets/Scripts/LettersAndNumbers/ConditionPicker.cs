using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConditionPicker 
{
	private int numConditions;
	public ConditionPicker(int numConditions)
	{
		this.numConditions = numConditions;
	}
	public List<ConditionObj> pickConditions(List<ConditionObj> conditions, string[][] limitedTypes, int[] counters)
	{
		Debug.LogFormat("Number Conditions: {0}", numConditions);
		List<ConditionObj> pickedConditions = new List<ConditionObj>();
		while(pickedConditions.Count < numConditions)
		{
			bool flag = true;
			for(int i = 0; i < conditions.Count; i++)
			{
				string code = conditions[i].getCode();
				code = code.Substring(code.Length - 2);
				if(canUse(code, limitedTypes, counters))
				{
					flag = false;
					pickedConditions.Add(conditions[i]);
					adjustCounters(code, limitedTypes, counters);
					conditions.RemoveAt(i);
					break;
				}
			}
			if(flag)
			{
				pickedConditions.Add(conditions[0]);
				conditions.RemoveAt(0);
			}
		}
		return pickedConditions;
	}
	private bool canUse(string condID, string[][] limitedTypes, int[] counters)
	{
		for(int i = 0; i < limitedTypes.Length; i++)
		{
			for(int j = 0; j < limitedTypes[i].Length; j++)
			{
				if (limitedTypes[i][j].Equals(condID))
					return counters[i] > 0;
			}
		}
		return false;
	}
	private void adjustCounters(string condID, string[][] limitedTypes, int[] counters)
	{
		for (int i = 0; i < limitedTypes.Length; i++)
		{
			for (int j = 0; j < limitedTypes[i].Length; j++)
			{
				if (limitedTypes[i][j].Equals(condID))
				{
					counters[i]--;
					return;
				}
			}
		}
	}
}
