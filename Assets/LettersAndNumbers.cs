using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conditions;
using Grid;
public class LettersAndNumbers : MonoBehaviour {

	private int moduleId;
	private static int moduleIdCounter = 1;
	private int maxLength = 18;
	public KMBombModule module;
	public new KMAudio audio;

	public KMSelectable cycleUp;
	public KMSelectable cycleDown;
	public KMSelectable[] gridButtons;
	public KMSelectable screen;

	public MeshRenderer[] gridMesh;
	public MeshRenderer screenMesh;

	public TextMesh screenText;
	public TextMesh pageText;
	public TextMesh[] gridText;
	private int prevPressed = -1;

	public Material[] gridMats;
	public Material[] screenMats;

	public AudioClip[] cycleSFX;
	public AudioClip[] buttonSFX;

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
	ConditionGenerator gen = new ConditionGenerator();
	ConditionRemover rem = new ConditionRemover();
	ConditionPicker picker = new ConditionPicker(6);
	List<ConditionObj> conditions;

	List<string> conditionText = new List<string>();
	int pageNum = 0;

	void Awake()
	{
		moduleId = moduleIdCounter++;

		GridObj grid = new GridObj();
		conditions = gen.generateAllConditions(grid);
		conditions = rem.removeConditions(conditions);
		string[][] limitedTypes = {
			new string[]{ EXACTLY, LEAST, FEWER },
			new string[]{ EVEN, ODD },
			new string[]{ EQUAL, LESS, GREATER },
			new string[]{ SAME, DIFFERENT }
		};
		int[] limits = { 2, 1, 2, 1 };
		conditions = picker.pickConditions(conditions, limitedTypes, limits);
		foreach(ConditionObj condition in conditions)
		{
			Debug.Log($"[Letters and Numbers #{moduleId}]: {condition.toString()}");
			conditionText.Add(stringToText(condition.getCondition()));
		}
		grid = rem.getWorstGrid(conditions);
		
		screenText.text = conditionText[pageNum];
		pageText.text = pageText.text = (pageNum + 1) + "/" + conditionText.Count;
		cycleUp.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX[1].name, transform); pageNum = mod(pageNum - 1, conditionText.Count); screenText.text = conditionText[pageNum]; pageText.text = (pageNum + 1) + "/" + conditionText.Count; return false; };
		cycleDown.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX[0].name, transform); pageNum = mod(pageNum + 1, conditionText.Count); screenText.text = conditionText[pageNum]; pageText.text = (pageNum + 1) + "/" + conditionText.Count; return false; };
		screen.OnInteract = delegate { StartCoroutine(pressedScreen()); return false; };
		Debug.Log($"[Letters and Numbers #{moduleId}]: {grid.toString()}");
		int[] indexes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
		foreach(int i in indexes)
		{
			gridText[i].text = grid.getSpace("ABCD"[i % 4] + "" + "1234"[i / 4]);
			gridButtons[i].OnInteract = delegate { pressedButton(i); return false; };
		}
	}
	private void pressedButton(int i)
	{
		if(prevPressed == -1)
		{
			audio.PlaySoundAtTransform(buttonSFX[0].name, transform);
			prevPressed = i;
			gridMesh[i].material = gridMats[1];
			gridText[i].color = Color.black;
		}
		else
		{
			if (prevPressed == i)
				audio.PlaySoundAtTransform(buttonSFX[0].name, transform);
			else
				audio.PlaySoundAtTransform(buttonSFX[1].name, transform);
			string temp = gridText[prevPressed].text + "";
			gridText[prevPressed].text = gridText[i].text + "";
			gridText[i].text = temp;
			gridMesh[prevPressed].material = gridMats[0];
			gridText[prevPressed].color = Color.white;
			prevPressed = -1;
		}
	}
	private IEnumerator pressedScreen()
	{
		yield return new WaitForSeconds(0f);
		foreach (KMSelectable gridButton in gridButtons)
			gridButton.OnInteract = null;
		screen.OnInteract = null;
		cycleUp.OnInteract = null;
		cycleDown.OnInteract = null;
		if(prevPressed >= 0)
		{
			gridMesh[prevPressed].material = gridMats[0];
			gridText[prevPressed].color = Color.white;
			prevPressed = -1;
		}
		GridObj grid = new GridObj(gridText);
		bool solved = true;
		for(int i = 0; i < conditionText.Count; i++)
		{
			audio.PlaySoundAtTransform(cycleSFX[0].name, transform);
			pageNum = i;
			screenText.text = conditionText[pageNum];
			pageText.text = (pageNum + 1) + "/" + conditionText.Count;
			yield return new WaitForSeconds(0.5f);
			if (rem.testCondition(conditions[i], grid))
			{
				audio.PlaySoundAtTransform(cycleSFX[1].name, transform);
				screenMesh.material = screenMats[1];
			}
			else
			{
				audio.PlaySoundAtTransform(cycleSFX[2].name, transform);
				screenMesh.material = screenMats[2];
				solved = false;
			}
			yield return new WaitForSeconds(0.5f);
			screenMesh.material = screenMats[0];
		}
		if(solved)
		{
			screenText.text = "";
			pageText.text = "";
			module.HandlePass();
			audio.PlaySoundAtTransform(cycleSFX[0].name, transform);
			audio.PlaySoundAtTransform(cycleSFX[1].name, transform);
			yield return new WaitForSeconds(0.25f);
			audio.PlaySoundAtTransform(cycleSFX[0].name, transform);
			audio.PlaySoundAtTransform(cycleSFX[1].name, transform);
			yield return new WaitForSeconds(0.125f);
			audio.PlaySoundAtTransform(cycleSFX[0].name, transform);
			audio.PlaySoundAtTransform(cycleSFX[1].name, transform);
			yield return new WaitForSeconds(0.125f);
			audio.PlaySoundAtTransform(cycleSFX[0].name, transform);
			audio.PlaySoundAtTransform(cycleSFX[1].name, transform);
		}
		else
		{
			int[] indexes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			cycleUp.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX[1].name, transform); pageNum = mod(pageNum - 1, conditionText.Count); screenText.text = conditionText[pageNum]; pageText.text = (pageNum + 1) + "/" + conditionText.Count; return false; };
			cycleDown.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX[0].name, transform); pageNum = mod(pageNum + 1, conditionText.Count); screenText.text = conditionText[pageNum]; pageText.text = (pageNum + 1) + "/" + conditionText.Count; return false; };
			screen.OnInteract = delegate { StartCoroutine(pressedScreen()); return false; };
			foreach (int index in indexes)
				gridButtons[index].OnInteract = delegate { pressedButton(index); return false; };
			pageNum = 0;
			screenText.text = conditionText[pageNum];
			pageText.text = (pageNum + 1) + "/" + conditionText.Count;
			module.HandleStrike();
		}
	}
	private string stringToText(string condition)
	{
		string text = "";
		while(condition.Length > maxLength)
		{
			string temp = condition.Substring(0, maxLength);
			if(temp[maxLength - 1] == ' ')
			{
				temp = temp.Substring(0, maxLength - 1);
				condition = condition.Substring(maxLength);
			}
			else if (condition[maxLength] == ' ')
			{
				condition = condition.Substring(maxLength + 1);
			}
			else
			{
				int index = temp.LastIndexOf(' ');
				temp = temp.Substring(0, index);
				condition = condition.Substring(index + 1);
			}
			text = text + temp + "\n";
		}
		return (text + condition);
	}
	private int mod(int n, int m)
	{
		while (n < 0)
			n += m;
		return (n % m);
	}
}
