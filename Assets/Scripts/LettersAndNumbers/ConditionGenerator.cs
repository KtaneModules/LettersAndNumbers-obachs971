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
	private string ORDER = "OR";
	private string CONDITIONAL = "CL";
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

			new LocationObj(grid.getSpaces(new string[] { "A1", "B1", "A2", "B2" }), "The 1st Quadrant", "1Q"),
			new LocationObj(grid.getSpaces(new string[] { "C1", "D1", "C2", "D2" }), "The 2nd Quadrant", "2Q"),
			new LocationObj(grid.getSpaces(new string[] { "A3", "B3", "A4", "B4" }), "The 3rd Quadrant", "3Q"),
			new LocationObj(grid.getSpaces(new string[] { "C3", "D3", "C4", "D4" }), "The 4th Quadrant", "4Q"),

			new LocationObj(grid.getSpaces(new string[] { "A1", "A2", "B1", "B2", "C1", "C2", "D1", "D2" }), "The Top Half", "TH"),
			new LocationObj(grid.getSpaces(new string[] { "A3", "A4", "B3", "B4", "C3", "C4", "D3", "D4" }), "The Bottom Half", "BH"),
			new LocationObj(grid.getSpaces(new string[] { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4" }), "The Left Side", "LS"),
			new LocationObj(grid.getSpaces(new string[] { "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4" }), "The Right Side", "RS")
		};
		int[] starts = { 0, 4, 8, 12 };

		List<ConditionObj> cond1 = new List<ConditionObj>();
		List<ConditionObj> cond2 = new List<ConditionObj>();
		List<ConditionObj> cond3 = new List<ConditionObj>();
		List<ConditionObj> cond4 = new List<ConditionObj>();
		List<ConditionObj> cond5 = new List<ConditionObj>();

		foreach (LocationObj location in locations)
			cond1.AddRange(generateAmountConditions(location));

		foreach (LocationObj location in locations)
			cond2.AddRange(generateCompareAmountConditions(location));
		foreach(int start in starts)
		{
			for(int i = start; i < start + 4; i++)
			{
				for(int j = i + 1; j < start + 4; j++)
					cond2.AddRange(generateCompareAmountConditions(locations[i], locations[j]));
			}
		}
		foreach (LocationObj location in locations)
			cond3.AddRange(generateCompareValueConditions(location, grid));

		cond4.AddRange(generateOrderConditions(new LocationObj[] { locations[0], locations[1], locations[2], locations[3] }, "Row", "RW"));
		cond4.AddRange(generateOrderConditions(new LocationObj[] { locations[4], locations[5], locations[6], locations[7] }, "Column", "CM"));
		cond4.AddRange(generateOrderConditions(new LocationObj[] { locations[8], locations[9], locations[10], locations[11] }, "Quadrant", "QT"));

		cond5.AddRange(generateConditionalConditions(new LocationObj[] { locations[0], locations[1], locations[2], locations[3] }, "Row", "RW"));
		cond5.AddRange(generateConditionalConditions(new LocationObj[] { locations[4], locations[5], locations[6], locations[7] }, "Column", "CM"));
		cond5.AddRange(generateConditionalConditions(new LocationObj[] { locations[8], locations[9], locations[10], locations[11] }, "Quadrant", "QT"));

		//Debug.LogFormat("Cond1: {0}", cond1.Count);
		//Debug.LogFormat("Cond2: {0}", cond2.Count);
		//Debug.LogFormat("Cond3: {0}", cond3.Count);
		//Debug.LogFormat("Cond4: {0}", cond4.Count);
		//Debug.LogFormat("Cond5: {0}", cond5.Count);

		cond2.Shuffle();
		cond3.Shuffle();
		cond4.Shuffle();
		cond5.Shuffle();
		
		while (cond2.Count > 15)
			cond2.RemoveAt(0);
		while (cond3.Count > 15)
			cond3.RemoveAt(0);
		while (cond4.Count > 10)
			cond4.RemoveAt(0);
		while (cond5.Count > 50)
			cond5.RemoveAt(0);

		conditions.AddRange(cond1);
		conditions.AddRange(cond2);
		conditions.AddRange(cond3);
		conditions.AddRange(cond4);
		conditions.AddRange(cond5);

		conditions.Shuffle();
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
	private List<ConditionObj> generateOrderConditions(LocationObj[] locations, string locName, string locID)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		string charList = "ABCD1234";
		foreach(char c1 in charList)
		{
			foreach (char c2 in charList)
			{
				foreach (char c3 in charList)
				{
					foreach (char c4 in charList)
					{
						string order = c1 + "" + c2 + "" + c3 + "" + c4;
						int sum = 0;
						foreach(LocationObj location in locations)
						{
							bool flag = true;
							string[] spaces = location.getSpaces();
							for(int i = 0; i < order.Length; i++)
							{
								if(!(spaces[i].Contains(order[i] + "")))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								sum++;
								if (sum > 1)
									break;
							}
						}
						if(sum == 1)
							conditions.Add(new CDOrder("One of the " + locName + "s is arranged in this order: " + order, locID, order, sum, ORDER));
					}
				}
			}
		}
		return conditions;
	}
	private List<ConditionObj> generateConditionalConditions(LocationObj[] locations, string locName, string locID)
	{
		List<ConditionObj> conditions = new List<ConditionObj>();
		string charList = "ABCD1234";
		foreach (char c1 in charList)
		{
			foreach (char c2 in charList)
			{
				if(c1 != c2)
				{
					//Exactly Exactly
					for(int i = 0; i <= 4; i++)
					{
						for (int j = 0; j <= 4; j++)
						{
							if(validConditional(locations, c1, i, EXACTLY, c2, j, EXACTLY))
							{
								string message = "Any " + locName + " that ";
								switch(i)
								{
									case 0:
										message += " does NOT contain any " + c1 + "s must also ";
										break;
									case 1:
										message += " contains exactly " + i + " " + c1 + " must also ";
										break;
									default:
										message += " contains exactly " + i + " " + c1 + "s must also ";
										break;
								}
								switch(j)
								{
									case 0:
										message += " NOT contain any " + c2 + "s";
										break;
									case 1:
										message += " contain exactly " + j + " " + c2;
										break;
									default:
										message += " contain exactly " + j + " " + c2 + "s";
										break;
								}
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, EXACTLY, c2 + "" + c2, j, EXACTLY, CONDITIONAL));
							}
						}
					}
					//Exactly Least
					for (int i = 0; i <= 4; i++)
					{
						for (int j = 1; j < 4; j++)
						{
							if (validConditional(locations, c1, i, EXACTLY, c2, j, LEAST))
							{
								string message = "Any " + locName + " that ";
								switch (i)
								{
									case 0:
										message += " does NOT contain any " + c1 + "s must also ";
										break;
									case 1:
										message += " contains exactly " + i + " " + c1 + " must also ";
										break;
									default:
										message += " contains exactly " + i + " " + c1 + "s must also ";
										break;
								}
								switch (j)
								{
									case 1:
										message += " contain at least " + j + " " + c2;
										break;
									default:
										message += " contain at least " + j + " " + c2 + "s";
										break;
								}
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, EXACTLY, c2 + "" + c2, j, LEAST, CONDITIONAL));
							}
						}
					}
					//Exactly Fewer
					for (int i = 0; i <= 4; i++)
					{
						for (int j = 1; j < 4; j++)
						{
							if (validConditional(locations, c1, i, EXACTLY, c2, j, FEWER))
							{
								string message = "Any " + locName + " that ";
								switch (i)
								{
									case 0:
										message += " does NOT contain any " + c1 + "s must also ";
										break;
									case 1:
										message += " contains exactly " + i + " " + c1 + " must also ";
										break;
									default:
										message += " contains exactly " + i + " " + c1 + "s must also ";
										break;
								}
								message += " contain " + j + " or fewer " + c2 + "s";
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, EXACTLY, c2 + "" + c2, j, FEWER, CONDITIONAL));
							}
						}
					}
					//Least Exactly
					for (int i = 1; i < 4; i++)
					{
						for (int j = 0; j <= 4; j++)
						{
							if (validConditional(locations, c1, i, LEAST, c2, j, EXACTLY))
							{
								string message = "Any " + locName + " that ";
								switch (i)
								{
									case 1:
										message += " contains at least " + i + " " + c1 + " must also ";
										break;
									default:
										message += " contains at least " + i + " " + c1 + "s must also ";
										break;
								}
								switch (j)
								{
									case 0:
										message += " NOT contain any " + c2 + "s";
										break;
									case 1:
										message += " contain exactly " + j + " " + c2;
										break;
									default:
										message += " contain exactly " + j + " " + c2 + "s";
										break;
								}
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, LEAST, c2 + "" + c2, j, EXACTLY, CONDITIONAL));
							}
						}
					}
					// Least Least
					for (int i = 1; i < 4; i++)
					{
						for (int j = 1; j < 4; j++)
						{
							if (validConditional(locations, c1, i, LEAST, c2, j, LEAST))
							{
								string message = "Any " + locName + " that ";
								switch (i)
								{
									case 1:
										message += " contains at least " + i + " " + c1 + " must also ";
										break;
									default:
										message += " contains at least " + i + " " + c1 + "s must also ";
										break;
								}
								switch (j)
								{
									case 1:
										message += " contain at least " + j + " " + c2;
										break;
									default:
										message += " contain at least " + j + " " + c2 + "s";
										break;
								}
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, LEAST, c2 + "" + c2, j, LEAST, CONDITIONAL));
							}
						}
					}
					//Least Fewer
					for (int i = 1; i < 4; i++)
					{
						for (int j = 1; j < 4; j++)
						{
							if (validConditional(locations, c1, i, LEAST, c2, j, FEWER))
							{
								string message = "Any " + locName + " that ";
								switch (i)
								{
									case 1:
										message += " contains at least " + i + " " + c1 + " must also ";
										break;
									default:
										message += " contains at least " + i + " " + c1 + "s must also ";
										break;
								}
								message += " contain " + j + " or fewer " + c2 + "s";
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, LEAST, c2 + "" + c2, j, FEWER, CONDITIONAL));
							}
						}
					}
					//Fewer Exactly
					for (int i = 1; i < 4; i++)
					{
						for (int j = 0; j <= 4; j++)
						{
							if (validConditional(locations, c1, i, FEWER, c2, j, EXACTLY))
							{
								string message = "Any " + locName + " that contains " + i + " or fewer " + c1 + "s must also ";
								switch (j)
								{
									case 0:
										message += " NOT contain any " + c2 + "s";
										break;
									case 1:
										message += " contain exactly " + j + " " + c2;
										break;
									default:
										message += " contain exactly " + j + " " + c2 + "s";
										break;
								}
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, FEWER, c2 + "" + c2, j, EXACTLY, CONDITIONAL));
							}
						}
					}
					//Fewer Least
					for (int i = 1; i < 4; i++)
					{
						for (int j = 1; j < 4; j++)
						{
							if (validConditional(locations, c1, i, FEWER, c2, j, LEAST))
							{
								string message = "Any " + locName + " that contains " + i + " or fewer " + c1 + "s must also ";
								switch (j)
								{
									case 1:
										message += " contain at least " + j + " " + c2;
										break;
									default:
										message += " contain at least " + j + " " + c2 + "s";
										break;
								}
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, FEWER, c2 + "" + c2, j, LEAST, CONDITIONAL));
							}
						}
					}
					//Fewer Fewer
					for (int i = 1; i < 4; i++)
					{
						for (int j = 1; j < 4; j++)
						{
							if (validConditional(locations, c1, i, FEWER, c2, j, FEWER))
							{
								string message = "Any " + locName + " that contains " + i + " or fewer " + c1 + "s must also contain " + j + " or fewer + " + c2 + "s";
								conditions.Add(new CDConditional(message, locID, c1 + "" + c1, i, FEWER, c2 + "" + c2, j, FEWER, CONDITIONAL));
							}
						}
					}
				} // End of if where 2 characters don't match
			}
		}
		return conditions;
	}
	private bool validConditional(LocationObj[] locations, char c1, int s1, string condID1, char c2, int s2, string condID2)
	{
		foreach(LocationObj location in locations)
		{
			if (testCondition(location, c1, s1, condID1) && !testCondition(location, c2, s2, condID2))
				return false;
		}
		return true;
	}
	private bool testCondition(LocationObj location, char c, int sum, string condID)
	{
		int total = location.getSum(c + "");
		switch(condID)
		{
			case "EX": return (total == sum);
			case "LT": return (total >= sum);
			default: return (total <= sum);
		}
	}
}
