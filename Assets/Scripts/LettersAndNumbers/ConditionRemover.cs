using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conditions;
using Grid;
using System;

public class ConditionRemover {

	public void testConditions(List<ConditionObj> conditions, GridObj grid)
	{
		foreach(ConditionObj condition in conditions)
		{
			bool flag = testCondition(condition, grid);
			if (!(flag))
				Debug.LogFormat("Condition returned false: {0}", condition.toString());
		}
	}
	public List<ConditionObj> removeConditions(List<ConditionObj> conditions)
	{
		List<GridObj> invalidGrids = getInvalidGrids(conditions);
		List<string> validIDs = getValidIDs(conditions, invalidGrids);
		return getFinalConditions(conditions, validIDs);
	}
	public GridObj getWorstGrid(List<ConditionObj> conditions)
	{
		GridObj worstGrid = new GridObj();
		int score = getGridScore(conditions, worstGrid);
		int counter = 1000;
		while(score > 0 && counter > 0)
		{
			GridObj grid = new GridObj();
			int newScore = getGridScore(conditions, grid);
			if(newScore < score)
			{
				worstGrid = grid;
				score = newScore;
			}
			counter--;
		}
		return worstGrid;
	}
	private int getGridScore(List<ConditionObj> conditions, GridObj grid)
	{
		int sum = 0;
		foreach (ConditionObj condition in conditions)
		{
			if (testCondition(condition, grid))
				sum++;
		}
		return sum;
	}
	private List<GridObj> getInvalidGrids(List<ConditionObj> conditions)
	{
		List<GridObj> invalidGrids = new List<GridObj>();
		List<bool> bools = new List<bool>();
		foreach (ConditionObj condition in conditions)
			bools.Add(true);
		for (int i = 0; i < conditions.Count; i++)
		{
			if (bools[i])
			{
				GridObj invalidGrid = getInvalidGrid(conditions[i]);
				if (invalidGrid == null)
				{
					conditions.RemoveAt(i);
					bools.RemoveAt(i);
					i--;
				}
				else
				{
					bools = getNewBools(conditions, invalidGrid);
					invalidGrids.Add(invalidGrid);
				}
			}
		}
		return invalidGrids;
	}
	private List<string> getValidIDs(List<ConditionObj> conditions, List<GridObj> invalidGrids)
	{
		List<string> validIDs = new List<string>();
		for(int i = 0; i < conditions.Count; i++)
		{
			string validID = getValidID(conditions[i], invalidGrids);
			if (validIDs.Contains(validID))
				conditions.RemoveAt(i--);
			else
				validIDs.Add(validID + "");
		}
		return validIDs;
	}
	private List<ConditionObj> getFinalConditions(List<ConditionObj> conditions, List<string> validIDs)
	{
		List<ConditionObj> finalConditions = new List<ConditionObj>();
		while(conditions.Count > 0)
		{
			string validID = validIDs[0];
			finalConditions.Add(conditions[0]);
			validIDs.RemoveAt(0);
			conditions.RemoveAt(0);
			for (int i = 0; i < conditions.Count; i++)
			{
				if (isSame(validIDs[i], validID))
				{
					finalConditions.RemoveAt(finalConditions.Count - 1);
					validID = "";
					break;
				}
			}
			if (validID.Length > 0)
			{
				for (int i = 0; i < conditions.Count; i++)
				{
					if (isSame(validID, validIDs[i]))
					{
						validIDs.RemoveAt(i);
						conditions.RemoveAt(i);
						i--;
					}
				}
			}
		}
		return finalConditions;
	}
	private GridObj getInvalidGrid(ConditionObj condition)
	{
		for(int i = 0; i < 1000; i++)
		{
			GridObj grid = new GridObj();
			if (!(testCondition(condition, grid)))
				return grid;
		}
		return null;
	}
	private List<bool>  getNewBools(List<ConditionObj> conditions, GridObj invalidGrid)
	{
		List<bool> newBools = new List<bool>();

		foreach (ConditionObj condition in conditions)
			newBools.Add(testCondition(condition, invalidGrid));

		return newBools;
	}
	private string getValidID(ConditionObj condition, List<GridObj> invalidGrids)
	{
		string validID = "";
		foreach(GridObj grid in invalidGrids)
			validID += testCondition(condition, grid) ? "1" : "0";
		return validID;
	}
	private bool isSame(string v1, string v2)
	{
		for (int i = 0; i < v1.Length; i++)
		{
			if (v1[i] == '1' && v2[i] == '0')
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
			case "1Q": return grid.getSpaces(new string[] { "A1", "A2", "B1", "B2" });
			case "2Q": return grid.getSpaces(new string[] { "C1", "C2", "D1", "D2" });
			case "3Q": return grid.getSpaces(new string[] { "A3", "A4", "B3", "B4" });
			case "4Q": return grid.getSpaces(new string[] { "C3", "C4", "D3", "D4" });
			case "TH": return grid.getSpaces(new string[] { "A1", "A2", "B1", "B2", "C1", "C2", "D1", "D2" });
			case "BH": return grid.getSpaces(new string[] { "A3", "A4", "B3", "B4", "C3", "C4", "D3", "D4" });
			case "LS": return grid.getSpaces(new string[] { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4" });
			case "RS": return grid.getSpaces(new string[] { "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4" });
			default: return new string[] { grid.getSpace(locID) };
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
}
