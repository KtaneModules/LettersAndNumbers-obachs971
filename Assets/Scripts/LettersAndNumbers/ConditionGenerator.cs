using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conditions;
using Grid;
using Location;


public class ConditionGenerator 
{
	private string EXACTLY = "EX";
	private string LEAST = "LT";
	private string FEWER = "FR";
	private string EVEN = "EN";
	private string ODD = "OD";
	private string EQUAL = "EL";
	private string LESS = "LS";
	private string GREATER = "GR";
	private string SAME = "SE";
	private string DIFFERENT = "DT";
	public List<ConditionObj> generateAllConditions(GridObj grid)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		LocationObj[] locations =
		{
			new LocationObj(grid.getSpaces(new string[] { "A1", "B1", "C1", "D1" }), "Row 1", "R1"),
			new LocationObj(grid.getSpaces(new string[] { "A2", "B2", "C2", "D2" }), "Row 2", "R2"),
			new LocationObj(grid.getSpaces(new string[] { "A3", "B3", "C3", "D3" }), "Row 3", "R3"),
			new LocationObj(grid.getSpaces(new string[] { "A4", "B4", "C4", "D4" }), "Row 4", "R4"),

			new LocationObj(grid.getSpaces(new string[] { "A1", "A2", "A3", "A4" }), "Column A", "CA"),
			new LocationObj(grid.getSpaces(new string[] { "B1", "B2", "B3", "B4" }), "Column B", "CB"),
			new LocationObj(grid.getSpaces(new string[] { "C1", "C2", "C3", "C4" }), "Column C", "CC"),
			new LocationObj(grid.getSpaces(new string[] { "D1", "D2", "D3", "D4" }), "Column D", "CD"),

			new LocationObj(grid.getSpaces(new string[] { "A1", "A2", "B1", "B2" }), "The 1st Quadrant", "1Q"),
			new LocationObj(grid.getSpaces(new string[] { "C1", "C2", "D1", "D2" }), "The 2nd Quadrant", "2Q"),
			new LocationObj(grid.getSpaces(new string[] { "A3", "A4", "B3", "B4" }), "The 3rd Quadrant", "3Q"),
			new LocationObj(grid.getSpaces(new string[] { "C3", "C4", "D3", "D4" }), "The 4th Quadrant", "4Q"),

			new LocationObj(grid.getSpaces(new string[] { "A1", "A2", "B1", "B2", "C1", "C2", "D1", "D2" }), "The Top Half", "TH"),
			new LocationObj(grid.getSpaces(new string[] { "A3", "A4", "B3", "B4", "C3", "C4", "D3", "D4" }), "The Bottom Half", "BH"),
			new LocationObj(grid.getSpaces(new string[] { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4" }), "The Left Side", "LS"),
			new LocationObj(grid.getSpaces(new string[] { "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4" }), "The Right Side", "RS")
		};
		int[] starts = { 0, 4, 8, 12 };

		List<ConditionObj> amountConditions = new List<ConditionObj>();
		List<ConditionObj> compareAmountConditions = new List<ConditionObj>();
		List<ConditionObj> compareValueConditions = new List<ConditionObj>();

		amountConditions.AddRange(removeHalf(generateAmountConditions(grid)));
		foreach(LocationObj location in locations)
			amountConditions.AddRange(removeHalf(generateAmountConditions(location)));

		foreach (LocationObj location in locations)
			compareAmountConditions.AddRange(removeHalf(generateCompareAmountConditions(location)));
		foreach(int start in starts)
		{
			for(int i = start; i < start + 4; i++)
			{
				for(int j = i + 1; j < start + 4; j++)
					compareAmountConditions.AddRange(removeHalf(generateCompareAmountConditions(locations[i], locations[j])));
			}
		}

		compareValueConditions.AddRange(removeHalf(generateCompareValueConditions(grid)));
		foreach (LocationObj location in locations)
			compareValueConditions.AddRange(removeHalf(generateCompareValueConditions(location, grid)));

		/*
		compareAmountConditions.Shuffle();
		compareValueConditions.Shuffle();
		while (compareAmountConditions.Count > amountConditions.Count)
			compareAmountConditions.RemoveAt(0);
		while (compareValueConditions.Count > amountConditions.Count)
			compareValueConditions.RemoveAt(0);
		*/

		conditions.AddRange(amountConditions);
		conditions.AddRange(compareAmountConditions);
		conditions.AddRange(compareValueConditions);
		conditions.Shuffle();
		return conditions;
	}
	private List<ConditionObj> generateAmountConditions(GridObj grid)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		
		for (int i = 0; i < 16; i++)
		{
			string space = "ABCD"[i % 4] + "" + ((i / 4) + 1);
			string value = grid.getSpace(space);
			conditions.Add(new CDExactly("The " + space + " space must contain the letter " + value[0], space, value[0] + "" + value[0], 1, EXACTLY));
			conditions.Add(new CDExactly("The " + space + " space must contain the number " + value[1], space, value[1] + "" + value[1], 1, EXACTLY));
			string lets = "ABCD".Replace(value[0] + "", "");
			string nums = "1234".Replace(value[1] + "", "");
			foreach(char let in lets)
				conditions.Add(new CDExactly("The " + space + " space must NOT contain the letter " + let, space, let + "" + let, 0, EXACTLY));
			foreach (char num in nums)
				conditions.Add(new CDExactly("The " + space + " space must NOT contain the number " + num, space, num + "" + num, 0, EXACTLY));
		}

		return conditions;
	}
	private List<ConditionObj> generateAmountConditions(LocationObj location)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		string chars = "ABCD1234";
		int sum, least, fewer;
		int max = (chars.Length / 2) - 1;

		foreach(char c in chars)
		{
			sum = location.getSum(c + "");
			switch(sum)
			{
				case 0:
					conditions.Add(new CDExactly(location.getName() + " must NOT contain any " + c + "s", location.getID(), c + "" + c, sum, EXACTLY));
					break;
				case 1:
					conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " " + c, location.getID(), c + "" + c, sum, EXACTLY));
					break;
				default:
					conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " " + c + "s", location.getID(), c + "" + c, sum, EXACTLY));
					break;
			}
			if(sum % 2 == 0)
				conditions.Add(new CDEven(location.getName() + " must contain an even number of " + c + "s", location.getID(), c + "" + c, EVEN));
			else
				conditions.Add(new CDOdd(location.getName() + " must contain an odd number of " + c + "s", location.getID(), c + "" + c, ODD));
			if (sum > 0)
			{
				least = UnityEngine.Random.Range(1, Mathf.Min(sum, max));
				if (least == 1)
					conditions.Add(new CDLeast(location.getName() + " must contain at least " + least + " " + c, location.getID(), c + "" + c, least, LEAST));
				else
					conditions.Add(new CDLeast(location.getName() + " must contain at least " + least + " " + c + "s", location.getID(), c + "" + c, least, LEAST));
			}
			if(sum < 4)
			{
				fewer = UnityEngine.Random.Range(Mathf.Max(1, sum), max);
				conditions.Add(new CDFewer(location.getName() + " must contain " + fewer + " or fewer " + c + "s", location.getID(), c + "" + c, fewer, FEWER));
			}
		}

		sum = location.getNumDistinctLetters();
		switch (sum)
		{
			case 1:
				conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " Distinct Letter", location.getID(), "DL", sum, EXACTLY));
				break;
			default:
				conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " Distinct Letters", location.getID(), "DL", sum, EXACTLY));
				break;
		}
		if (sum % 2 == 0)
			conditions.Add(new CDEven(location.getName() + " must contain an even number of Distinct Letters", location.getID(), "DL", EVEN));
		else
			conditions.Add(new CDOdd(location.getName() + " must contain an odd number of Distinct Letters", location.getID(), "DL", ODD));
		if (sum > 1)
		{
			least = UnityEngine.Random.Range(2, Mathf.Min(sum, max));
			conditions.Add(new CDLeast(location.getName() + " must contain at least " + least + " Distinct Letters", location.getID(), "DL", least, LEAST));
		}
		if (sum < 4)
		{
			fewer = UnityEngine.Random.Range(Mathf.Max(2, sum), max);
			conditions.Add(new CDFewer(location.getName() + " must contain " + fewer + " or fewer Distinct Letters", location.getID(), "DL", fewer, FEWER));
		}

		sum = location.getNumDistinctNumbers();
		switch (sum)
		{
			case 1:
				conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " Distinct Number", location.getID(), "DN", sum, EXACTLY));
				break;
			default:
				conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " Distinct Numbers", location.getID(), "DN", sum, EXACTLY));
				break;
		}
		if (sum % 2 == 0)
			conditions.Add(new CDEven(location.getName() + " must contain an even number of Distinct Numbers", location.getID(), "DN", EVEN));
		else
			conditions.Add(new CDOdd(location.getName() + " must contain an odd number of Distinct Numbers", location.getID(), "DN", ODD));
		if (sum > 1)
		{
			least = UnityEngine.Random.Range(2, Mathf.Min(sum, max));
			conditions.Add(new CDLeast(location.getName() + " must contain at least " + least + " Distinct Numbers", location.getID(), "DN", least, LEAST));
		}
		if (sum < 4)
		{
			fewer = UnityEngine.Random.Range(Mathf.Max(2, sum), max);
			conditions.Add(new CDFewer(location.getName() + " must contain " + fewer + " or fewer Distinct Numbers", location.getID(), "DN", fewer, FEWER));
		}

		string letters = chars.Substring(0, chars.Length / 2);
		string numbers = chars.Substring(chars.Length / 2);
		foreach (char let in letters)
		{
			foreach(char num in numbers)
			{
				if(location.hasCombination(let + "" + num))
					conditions.Add(new CDExactly(location.getName() + " must contain an " + let + num, location.getID(), let + "" + num, 1, EXACTLY));
				else
					conditions.Add(new CDExactly(location.getName() + " must NOT contain an " + let + num, location.getID(), let + "" + num, 0, EXACTLY));
			}
		}

		return conditions;
	}
	private List<ConditionObj> generateCompareAmountConditions(LocationObj location)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		string chars = "ABCD1234";
		for(int i = 0; i < chars.Length; i++)
		{
			for(int j = i + 1; j < chars.Length; j++)
			{
				int s1 = location.getSum(chars[i] + ""), s2 = location.getSum(chars[j] + "");
				if (s1 < s2)
					conditions.Add(new CDLess("The number of " + chars[i] + "s in " + location.getName().Replace("The", "the") + " must be less than the number of " + chars[j] + "s in " + location.getName().Replace("The", "the"), location.getID(), chars[i] + "" + chars[i], chars[j] + "" + chars[j], LESS));
				else if (s1 == s2)
					conditions.Add(new CDEqual("The number of " + chars[i] + "s in " + location.getName().Replace("The", "the") + " must be equal to the number of " + chars[j] + "s in " + location.getName().Replace("The", "the"), location.getID(), chars[i] + "" + chars[i], chars[j] + "" + chars[j], EQUAL));
				else
					conditions.Add(new CDGreater("The number of " + chars[i] + "s in " + location.getName().Replace("The", "the") + " must be greater than the number of " + chars[j] + "s in " + location.getName().Replace("The", "the"), location.getID(), chars[i] + "" + chars[i], chars[j] + "" + chars[j], GREATER));
			}
		}
		return conditions;
	}
	private List<ConditionObj> generateCompareAmountConditions(LocationObj loc1, LocationObj loc2)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		string chars = "ABCD1234";
		for (int i = 0; i < chars.Length; i++)
		{
			for (int j = 0; j < chars.Length; j++)
			{
				int s1 = loc1.getSum(chars[i] + ""), s2 = loc2.getSum(chars[j] + "");
				if (s1 < s2)
					conditions.Add(new CDLess("The number of " + chars[i] + "s in " + loc1.getName().Replace("The", "the") + " must be less than the number of " + chars[j] + "s in " + loc2.getName().Replace("The", "the"), loc1.getID(), chars[i] + "" + chars[i], loc2.getID(), chars[j] + "" + chars[j], LESS));
				else if (s1 == s2)
					conditions.Add(new CDEqual("The number of " + chars[i] + "s in " + loc1.getName().Replace("The", "the") + " must be equal to the number of " + chars[j] + "s in " + loc2.getName().Replace("The", "the"), loc1.getID(), chars[i] + "" + chars[i], loc2.getID(), chars[j] + "" + chars[j], EQUAL));
				else
					conditions.Add(new CDGreater("The number of " + chars[i] + "s in " + loc1.getName().Replace("The", "the") + " must be greater than the number of " + chars[j] + "s in " + loc2.getName().Replace("The", "the"), loc1.getID(), chars[i] + "" + chars[i], loc2.getID(), chars[j] + "" + chars[j], GREATER));
			}
		}
		return conditions;
	}
	private List<ConditionObj> generateCompareValueConditions(GridObj grid)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		for(int i = 0; i < 16; i++)
		{
			for(int j = i + 1; j < 16; j++)
			{
				string s1 = "ABCD"[i % 4] + "" + ((i / 4) + 1);
				string s2 = "ABCD"[j % 4] + "" + ((j / 4) + 1);
				string v1 = grid.getSpace(s1);
				string v2 = grid.getSpace(s2);
				if (v1[0] == v2[0])
					conditions.Add(new CDSame("The " + s1 + " space must have the same letter as the " + s2 + " space", s1, "LV", s2, SAME));
				else
					conditions.Add(new CDDifferent("The " + s1 + " space must NOT have the same letter as the " + s2 + " space", s1, "LV", s2, DIFFERENT));
				if(v1[1] == v2[1])
					conditions.Add(new CDSame("The " + s1 + " space must have the same number as the " + s2 + " space", s1, "NV", s2, SAME));
				else
					conditions.Add(new CDDifferent("The " + s1 + " space must NOT have the same number as the " + s2 + " space", s1, "NV", s2, DIFFERENT));
			}
		}
		return conditions;
	}
	private List<ConditionObj> generateCompareValueConditions(LocationObj location, GridObj grid)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		int max = 3;
		for (int i = 0; i < 16; i++)
		{
			
			string space = "ABCD"[i % 4] + "" + ((i / 4) + 1);
			string value = grid.getSpace(space);
			//Letters
			int sum = location.getSum(value[0] + ""), least, fewer;
			switch (sum)
			{
				case 0:
					conditions.Add(new CDExactly(location.getName() + " must NOT contain any spaces that shares the same letter as the " + space + " space", location.getID(), "LL", space, sum, EXACTLY));
					break;
				case 1:
					conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " space that shares the same letter as the " + space + " space", location.getID(), "LL", space, sum, EXACTLY));
					break;
				default:
					conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " spaces that shares the same letter as the " + space + " space", location.getID(), "LL", space, sum, EXACTLY));
					break;
			}
			if (sum % 2 == 0)
				conditions.Add(new CDEven(location.getName() + " must contain an even number of spaces that shares the same letter as the " + space + " space", location.getID(), "LL", space, EVEN));
			else
				conditions.Add(new CDOdd(location.getName() + " must contain an odd number of spaces that shares the same letter as the " + space + " space", location.getID(), "LL", space, ODD));
			if (sum > 1)
			{
				least = UnityEngine.Random.Range(2, Mathf.Min(sum, max));
				if (least == 1)
					conditions.Add(new CDLeast(location.getName() + " must contain at least " + least + " space that shares the same letter as the " + space + " space", location.getID(), "LL", space, least, LEAST));
				else
					conditions.Add(new CDLeast(location.getName() + " must contain at least " + least + " spaces that shares the same letter as the " + space + " space", location.getID(), "LL", space, least, LEAST));
			}
			if (sum < 4)
			{
				fewer = UnityEngine.Random.Range(Mathf.Max(1, sum), max);
				conditions.Add(new CDFewer(location.getName() + " must contain " + fewer + " or fewer spaces that shares the same letter as the " + space + " space", location.getID(), "LL", space, fewer, FEWER));
			}
			sum = location.getSum(value[1] + "");
			switch (sum)
			{
				case 0:
					conditions.Add(new CDExactly(location.getName() + " must NOT contain any spaces that shares the same number as the " + space + " space", location.getID(), "NN", space, sum, EXACTLY));
					break;
				case 1:
					conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " space that shares the same number as the " + space + " space", location.getID(), "NN", space, sum, EXACTLY));
					break;
				default:
					conditions.Add(new CDExactly(location.getName() + " must contain exactly " + sum + " spaces that shares the same number as the " + space + " space", location.getID(), "NN", space, sum, EXACTLY));
					break;
			}
			if (sum % 2 == 0)
				conditions.Add(new CDEven(location.getName() + " must contain an even number of spaces that shares the same number as the " + space + " space", location.getID(), "NN", space, EVEN));
			else
				conditions.Add(new CDOdd(location.getName() + " must contain an odd number of spaces that shares the same number as the " + space + " space", location.getID(), "NN", space, ODD));
			if (sum > 1)
			{
				least = UnityEngine.Random.Range(2, Mathf.Min(sum, max));
				if (least == 1)
					conditions.Add(new CDLeast(location.getName() + " must contain at least " + least + " space that shares the same number as the " + space + " space", location.getID(), "NN", space, least, LEAST));
				else
					conditions.Add(new CDLeast(location.getName() + " must contain at least " + least + " spaces that shares the same number as the " + space + " space", location.getID(), "NN", space, least, LEAST));
			}
			if (sum < 4)
			{
				fewer = UnityEngine.Random.Range(Mathf.Max(1, sum), max);
				conditions.Add(new CDFewer(location.getName() + " must contain " + fewer + " or fewer spaces that shares the same number as the " + space + " space", location.getID(), "NN", space, fewer, FEWER));
			}
		}
		return conditions;
	}
	private List<ConditionObj> removeHalf(List<ConditionObj> conditions)
	{
		int length = conditions.Count / 2;
		conditions.Shuffle();
		while (conditions.Count > length)
			conditions.RemoveAt(0);
		return conditions;
	}
}
