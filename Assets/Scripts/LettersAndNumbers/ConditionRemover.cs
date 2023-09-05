using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conditions;
using Grid;
using System;

public class ConditionRemover {

	public List<ConditionObj> testConditions(List<ConditionObj> conditions, GridObj grid, int moduleId)
	{
		List<ConditionObj> validConditions = new List<ConditionObj>();
		foreach (ConditionObj condition in conditions)
		{
			bool flag = testCondition(condition, grid);
			if (!(flag))
				Debug.Log($"[Letters and Numbers #{moduleId}]: Condition returned false: {condition.toString()}");
			else
				validConditions.Add(condition);
		}
		return validConditions;
	}
	public List<ConditionObj> removeConditions(GridObj solution, List<ConditionObj> conditions, List<int[]> combos, List<int> movableSpaces)
	{
		List<GridObj> invalidGrids = getInvalidGrids(solution, combos, movableSpaces);
		if (hasMultipleSolutions(conditions, invalidGrids))
			return null;
		removeRedundantConditions(conditions, invalidGrids);
		return getFinalConditions(conditions, invalidGrids);
	}
	private List<GridObj> getInvalidGrids(GridObj solution, List<int[]> combos, List<int> movableSpaces)
	{
		List<GridObj> invalidGrids = new List<GridObj>();
		
		foreach(int[] combo in combos)
			invalidGrids.Add(new GridObj(solution, movableSpaces, combo));
		
		return invalidGrids;
	}
	private void removeRedundantConditions(List<ConditionObj> conditions, List<GridObj> invalidGrids)
	{
		List<string> validIDs = new List<string>();
		for(int i = 0; i < conditions.Count; i++)
		{
			string validID = getValidID(conditions[i], invalidGrids);
			if (validID.IndexOf('0') < 0 || validIDs.Contains(validID))
				conditions.RemoveAt(i--);
			else
				validIDs.Add(validID + "");
		}
	}
	private List<ConditionObj> getFinalConditions(List<ConditionObj> conditions, List<GridObj> invalidGrids)
	{
		for(int i = 0; i < conditions.Count; i++)
		{
			ConditionObj condition = conditions[i];
			conditions.RemoveAt(i);
			if (hasMultipleSolutions(conditions, invalidGrids))
				conditions.Insert(i, condition);
			else
				i--;
		}
		return conditions;
	}
	private string getValidID(ConditionObj condition, List<GridObj> invalidGrids)
	{
		string validID = "";
		foreach(GridObj grid in invalidGrids)
			validID += testCondition(condition, grid) ? "1" : "0";
		return validID;
	}
	private bool hasMultipleSolutions(List<ConditionObj> conditions, List<GridObj> invalidGrids)
	{
		foreach(GridObj grid in invalidGrids)
		{
			if (isValidGrid(conditions, grid))
				return true;
		}
		return false;
	}
	private bool isValidGrid(List<ConditionObj> conditions, GridObj grid)
	{
		foreach(ConditionObj condition in conditions)
		{
			if (!testCondition(condition, grid))
				return false;
		}
		return true;
	}
	public bool testCondition(ConditionObj condition, GridObj grid)
	{
		string code = condition.getCode();
		string id = code.Substring(code.Length - 2);
		switch(id)
		{
			case "EX":
				string[] exLoc1 = getSpaces(code.Substring(0, 2), grid);
				var exAttrID = code.Substring(2, 2);
				string[] exLoc2 = getSpaces(code.Substring(4, 2), grid);
				int exSum = Int16.Parse(code.Substring(6, 1));
				return (codeToValue(exLoc1, exAttrID, exLoc2[0]) == exSum);
			case "LT":
				string[] ltLoc1 = getSpaces(code.Substring(0, 2), grid);
				var ltAttrID = code.Substring(2, 2);
				string[] ltLoc2 = getSpaces(code.Substring(4, 2), grid);
				int ltSum = Int16.Parse(code.Substring(6, 1));
				return (codeToValue(ltLoc1, ltAttrID, ltLoc2[0]) >= ltSum);
			case "FR":
				string[] frLoc1 = getSpaces(code.Substring(0, 2), grid);
				var frAttrID = code.Substring(2, 2);
				string[] frLoc2 = getSpaces(code.Substring(4, 2), grid);
				int frSum = Int16.Parse(code.Substring(6, 1));
				return (codeToValue(frLoc1, frAttrID, frLoc2[0]) <= frSum);
			case "EN":
				string[] enLoc1 = getSpaces(code.Substring(0, 2), grid);
				var enAttrID = code.Substring(2, 2);
				string[] enLoc2 = getSpaces(code.Substring(4, 2), grid);
				return (codeToValue(enLoc1, enAttrID, enLoc2[0]) % 2 == 0);
			case "OD":
				string[] odLoc1 = getSpaces(code.Substring(0, 2), grid);
				var odAttrID = code.Substring(2, 2);
				string[] odLoc2 = getSpaces(code.Substring(4, 2), grid);
				return (codeToValue(odLoc1, odAttrID, odLoc2[0]) % 2 == 1);
			case "EL":
				string[] elLoc1 = getSpaces(code.Substring(0, 2), grid);
				var elAttrID1 = code.Substring(2, 2);
				string[] elLoc2 = getSpaces(code.Substring(4, 2), grid);
				var elAttrID2 = code.Substring(6, 2);
				return (codeToValue(elLoc1, elAttrID1, null) == codeToValue(elLoc2, elAttrID2, null));
			case "LS":
				string[] lsLoc1 = getSpaces(code.Substring(0, 2), grid);
				var lsAttrID1 = code.Substring(2, 2);
				string[] lsLoc2 = getSpaces(code.Substring(4, 2), grid);
				var lsAttrID2 = code.Substring(6, 2);
				return (codeToValue(lsLoc1, lsAttrID1, null) < codeToValue(lsLoc2, lsAttrID2, null));
			case "GR":
				string[] grLoc1 = getSpaces(code.Substring(0, 2), grid);
				var grAttrID1 = code.Substring(2, 2);
				string[] grLoc2 = getSpaces(code.Substring(4, 2), grid);
				var grAttrID2 = code.Substring(6, 2);
				return (codeToValue(grLoc1, grAttrID1, null) > codeToValue(grLoc2, grAttrID2, null));
			case "SE":
				string seSpace1 = grid.getSpace(code.Substring(0, 2));
				string seAttrID = code.Substring(2, 2);
				string seSpace2 = grid.getSpace(code.Substring(4, 2));
				return codeToValue(seSpace1, seAttrID) == codeToValue(seSpace2, seAttrID);
			case "DT":
				string dtSpace1 = grid.getSpace(code.Substring(0, 2));
				string dtAttrID = code.Substring(2, 2);
				string dtSpace2 = grid.getSpace(code.Substring(4, 2));
				return codeToValue(dtSpace1, dtAttrID) != codeToValue(dtSpace2, dtAttrID);
			case "OR":
				string[][] orLocations = getLocations(code.Substring(0, 2), grid);
				string orderID = code.Substring(2, 4);
				int orSum = Int16.Parse(code.Substring(6, 1));
				return (getOrderSum(orLocations, orderID) == orSum);
			case "CL":
				string[][] clLocations = getLocations(code.Substring(0, 2), grid);
				string clAttrID1 = code.Substring(2, 2);
				int clSum1 = Int16.Parse(code.Substring(4, 1));
				string clCondID1 = code.Substring(5, 2);
				string clAttrID2 = code.Substring(7, 2);
				int clSum2 = Int16.Parse(code.Substring(9, 1));
				string clCondID2 = code.Substring(10, 2);
				return checkConditional(clLocations, clAttrID1[0], clSum1, clCondID1, clAttrID2[0], clSum2, clCondID2);
		}
		return false;
	}
	private int codeToValue(string[] loc1, string attrID, string loc2)
	{
		switch(attrID)
		{
			case "AA": return getNumOcc("A", loc1);
			case "BB": return getNumOcc("B", loc1);
			case "CC": return getNumOcc("C", loc1);
			case "DD": return getNumOcc("D", loc1);
			case "11": return getNumOcc("1", loc1);
			case "22": return getNumOcc("2", loc1);
			case "33": return getNumOcc("3", loc1);
			case "44": return getNumOcc("4", loc1);
			case "DL": return getNumDis(loc1, 0);
			case "DN": return getNumDis(loc1, 1);
			case "LL": return getNumOcc(loc2[0] + "", loc1);
			case "NN": return getNumOcc(loc2[1] + "", loc1);
			default: return getNumOcc(attrID, loc1);
		}
	}
	private string[] getSpaces(string locID, GridObj grid)
	{
		switch(locID)
		{
			case "R1": return grid.getSpaces(new string[] { "A1", "B1", "C1", "D1" });
			case "R2": return grid.getSpaces(new string[] { "A2", "B2", "C2", "D2" });
			case "R3": return grid.getSpaces(new string[] { "A3", "B3", "C3", "D3" });
			case "R4": return grid.getSpaces(new string[] { "A4", "B4", "C4", "D4" });
			case "CA": return grid.getSpaces(new string[] { "A1", "A2", "A3", "A4" });
			case "CB": return grid.getSpaces(new string[] { "B1", "B2", "B3", "B4" });
			case "CC": return grid.getSpaces(new string[] { "C1", "C2", "C3", "C4" });
			case "CD": return grid.getSpaces(new string[] { "D1", "D2", "D3", "D4" });
			case "1Q": return grid.getSpaces(new string[] { "A1", "B1", "A2", "B2" });
			case "2Q": return grid.getSpaces(new string[] { "C1", "D1", "C2", "D2" });
			case "3Q": return grid.getSpaces(new string[] { "A3", "B3", "A4", "B4" });
			case "4Q": return grid.getSpaces(new string[] { "C3", "D3", "C4", "D4" });
			case "TH": return grid.getSpaces(new string[] { "A1", "A2", "B1", "B2", "C1", "C2", "D1", "D2" });
			case "BH": return grid.getSpaces(new string[] { "A3", "A4", "B3", "B4", "C3", "C4", "D3", "D4" });
			case "LS": return grid.getSpaces(new string[] { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4" });
			case "RS": return grid.getSpaces(new string[] { "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4" });
			default: return new string[] { grid.getSpace(locID) };
		}
	}
	private string[][] getLocations(string locID, GridObj grid)
	{
		switch (locID)
		{
			case "RW": return new string[][] { getSpaces("R1", grid), getSpaces("R2", grid), getSpaces("R3", grid), getSpaces("R4", grid) };
			case "CM": return new string[][] { getSpaces("CA", grid), getSpaces("CB", grid), getSpaces("CC", grid), getSpaces("CD", grid) };
			default: return new string[][] { getSpaces("1Q", grid), getSpaces("2Q", grid), getSpaces("3Q", grid), getSpaces("4Q", grid) };

		}
	}
	private int getNumOcc(string str, string[] spaces)
	{
		int sum = 0;
		foreach(string space in spaces)
		{
			if (space.Contains(str))
				sum++;
		}
		return sum;
	}
	private int getNumDis(string[] spaces, int pos)
	{
		HashSet<char> chars = new HashSet<char>();
		foreach (string space in spaces)
			chars.Add(space[pos]);
		return chars.Count;
	}
	private char codeToValue(string space, string attrID)
	{
		return attrID.Equals("LV") ? space[0] : space[1];
	}

	private int getOrderSum(string[][] locations, string orderID)
	{
		int sum = 0;
		foreach(string[] location in locations)
		{
			bool flag = true;
			for(int i = 0; i < orderID.Length; i++)
			{
				if(!(location[i].Contains(orderID[i] + "")))
				{
					flag = false;
					break;
				}
			}
			if (flag)
				sum++;
		}
		return sum;
	}
	private bool checkConditional(string[][] locations, char c1, int s1, string condID1, char c2, int s2, string condID2)
	{
		foreach (string[] location in locations)
		{
			if (testCondition(location, c1, s1, condID1) && !testCondition(location, c2, s2, condID2))
				return false;
		}
		return true;
	}
	private bool testCondition(string[] location, char c, int sum, string condID)
	{
		int total = 0;
		foreach(string space in location)
		{
			if (space.Contains(c + ""))
				total++;
		}
		switch (condID)
		{
			case "EX": return (total == sum);
			case "LT": return (total >= sum);
			default: return (total <= sum);
		}
	}
}
