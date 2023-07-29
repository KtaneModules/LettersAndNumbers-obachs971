using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Location
{
    public class LocationObj
    {
        private Dictionary<string, int> sums;
        private int numDisLet;
        private int numDisNum;
        private string name;
        private string id;
        private string[] spaces;

        public LocationObj(string[] spaces, string name, string id)
        {
            this.name = name;
            this.id = id;
            this.spaces = spaces;

            sums = new Dictionary<string, int>();
            sums.Add("A", 0);
            sums.Add("B", 0);
            sums.Add("C", 0);
            sums.Add("D", 0);
            sums.Add("1", 0);
            sums.Add("2", 0);
            sums.Add("3", 0);
            sums.Add("4", 0);
            numDisLet = 0;
            numDisNum = 0;

            HashSet<char> lets = new HashSet<char>();
            HashSet<char> nums = new HashSet<char>();

            foreach (string space in spaces)
            {
                sums[space.Substring(0, 1)]++;
                sums[space.Substring(1, 1)]++;
                lets.Add(space[0]);
                nums.Add(space[1]);
            }
            numDisLet = lets.Count;
            numDisNum = nums.Count;
        }
        public string getName()
        {
            return name;
        }
        public string getID()
        {
            return id;
        }
        public int getSum(string let)
        {
            return sums[let];
        }
        public int getNumDistinctLetters()
        {
            return numDisLet;
        }
        public int getNumDistinctNumbers()
        {
            return numDisNum;
        }
        public bool hasCombination(string combination)
        {
            foreach(string space in spaces)
            {
                if (space.Equals(combination))
                    return true;
            }
            return false;
        }
    }
}

