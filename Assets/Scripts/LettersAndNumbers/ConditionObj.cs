using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionObj 
{
	private string condition;
	private string[] locIDs;
	private string[] attrIDs;
	private string code;
	public ConditionObj(string condition, string[] locIDs, string[] attrIDs, string code)
	{
		this.condition = condition;
		this.locIDs = locIDs;
		this.attrIDs = attrIDs;
		this.code = code;
	}
	public string getCondition()
	{
		return condition;
	}
	public string[] getLocIDs()
	{
		return locIDs;
	}
	public string[] getAttrIDs()
	{
		return attrIDs;
	}
	public string getCode()
	{
		return code;
	}
	public string toString()
	{
		return code + ": " + condition;
	}
	
}
