using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Grid
{
	public class GridObj
	{
		private List<string> spaces;
		public GridObj()
		{
			spaces = new List<string>();
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
					spaces.Add("ABCD"[i] + "" + (j + 1));
			}
			spaces.Shuffle();
		}
		public GridObj(TextMesh[] gridText)
		{
			spaces = new List<string>();
			foreach (TextMesh text in gridText)
				spaces.Add(text.text);
		}
		public string getSpace(string name)
		{
			return spaces[nameToIndex(name)];
		}
		public string[] getSpaces(string[] names)
		{
			string[] spaces = new string[names.Length];
			for (int i = 0; i < names.Length; i++)
				spaces[i] = this.spaces[nameToIndex(names[i])];
			return spaces;
		}
		private int nameToIndex(string name)
		{
			int col = name[0] - 'A';
			int row = name[1] - '1';
			return ((row * 4) + col);
		}
		public string toString()
		{
			string str = "";
			foreach (string space in spaces)
				str = str + space + " ";
			return str;
		}
	}
}

