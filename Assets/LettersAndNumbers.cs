using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conditions;
using Grid;
using System.Text.RegularExpressions;

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



	ConditionGenerator gen = new ConditionGenerator();
	ConditionRemover rem = new ConditionRemover();
	List<ConditionObj> conditions;

	List<string> conditionText = new List<string>();
	int pageNum = 0;

	private int NUM_MOVABLE_SPACES = 5;

	void Awake()
	{
		moduleId = moduleIdCounter++;

		GridObj grid = new GridObj();
		List<int[]> combos = new List<int[]>();
		getAllCombos(NUM_MOVABLE_SPACES, new int[0], combos);
		combos.RemoveAt(0);
		List<int> movableSpaces = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
		movableSpaces.Shuffle();
		while (movableSpaces.Count > NUM_MOVABLE_SPACES)
			movableSpaces.RemoveAt(0);
		movableSpaces.Sort();

		conditions = gen.generateAllConditions(grid);
		//conditions = rem.testConditions(conditions, grid, moduleId);
		List<ConditionObj> temp = rem.removeConditions(grid, new List<ConditionObj>(conditions), combos, movableSpaces);
		while(temp == null)
		{
			conditions = gen.generateAllConditions(grid);
			temp = rem.removeConditions(grid, new List<ConditionObj>(conditions), combos, movableSpaces);
		}
		conditions = temp;
		Debug.Log($"[Letters and Numbers #{moduleId}]: Solution: {grid.toString()}");
		
		for (int i = 0; i < combos.Count; i++)
		{
			for (int j = 0; j < NUM_MOVABLE_SPACES; j++)
			{
				if (combos[i][j] == j)
				{
					combos.RemoveAt(i);
					i--;
					break;
				}
			}
		}
		grid = new GridObj(grid, movableSpaces, combos[UnityEngine.Random.Range(0, combos.Count)]);
		Debug.Log($"[Letters and Numbers #{moduleId}]: Initial Layout: {grid.toString()}");
		foreach (ConditionObj condition in conditions)
		{
			Debug.Log($"[Letters and Numbers #{moduleId}]: {condition.toString()}");
			conditionText.Add(stringToText(condition.getCondition()));
		}

		screenText.text = conditionText[pageNum];
		pageText.text = pageText.text = (pageNum + 1) + "/" + conditionText.Count;
		cycleUp.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX[1].name, transform); pageNum = mod(pageNum - 1, conditionText.Count); screenText.text = conditionText[pageNum]; pageText.text = (pageNum + 1) + "/" + conditionText.Count; return false; };
		cycleDown.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX[0].name, transform); pageNum = mod(pageNum + 1, conditionText.Count); screenText.text = conditionText[pageNum]; pageText.text = (pageNum + 1) + "/" + conditionText.Count; return false; };
		screen.OnInteract = delegate { StartCoroutine(pressedScreen()); return false; };
		
		int[] indexes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
		foreach (int i in indexes)
		{
			gridText[i].text = grid.getSpace("ABCD"[i % 4] + "" + "1234"[i / 4]);

		}
		foreach (int i in movableSpaces)
		{
			gridButtons[i].OnInteract = delegate { pressedButton(i); return false; };
			gridMesh[i].material = gridMats[1];
			gridText[i].color = Color.black;
		}
	}
	private void getAllCombos(int n, int[] combo, List<int[]> combos)
	{
		if (combo.Length == n)
		{
			combos.Add(combo);
		}
		else
		{
			for (int i = 0; i < n; i++)
			{
				if (canAdd(i, combo))
					getAllCombos(n, getNewCombo(i, combo), combos);
			}
		}
	}
	private bool canAdd(int num, int[] combo)
	{
		foreach (int n in combo)
		{
			if (n == num)
				return false;
		}
		return true;
	}
	private int[] getNewCombo(int num, int[] combo)
	{
		int[] newCombo = new int[combo.Length + 1];
		for (int i = 0; i < combo.Length; i++)
			newCombo[i] = combo[i];
		newCombo[newCombo.Length - 1] = num;
		return newCombo;
	}
	private void pressedButton(int i)
	{
		if(prevPressed == -1)
		{
			audio.PlaySoundAtTransform(buttonSFX[0].name, transform);
			prevPressed = i;
			gridMesh[i].material = gridMats[2];
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
			gridMesh[prevPressed].material = gridMats[1];
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
		Debug.Log($"[Letters and Numbers #{moduleId}]: Submission: {grid.toString()}");
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
				Debug.Log($"[Letters and Numbers #{moduleId}]: Condition #{i+1} passed!");
			}
			else
			{
				audio.PlaySoundAtTransform(cycleSFX[2].name, transform);
				screenMesh.material = screenMats[2];
				solved = false;
				Debug.Log($"[Letters and Numbers #{moduleId}]: Condition #{i + 1} failed!");
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
#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} press|p (U)p (D)own (Col(ABCD)Row(1234)) to press those buttons. !{0} submit to submit your current grid. !{0} clear to clear the entire grid.";
#pragma warning restore 414
	IEnumerator ProcessTwitchCommand(string command)
	{
		string[] param = command.ToUpper().Split(' ');
		if ((Regex.IsMatch(param[0], @"^\s*PRESS\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(param[0], @"^\s*P\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) && param.Length > 1)
		{
			if (isButton(param))
			{
				yield return null;
				for (int i = 1; i < param.Length; i++)
				{
					switch (param[i])
					{
						case "UP":
						case "U":
							cycleUp.OnInteract();
							break;
						case "DOWN":
						case "D":
							cycleDown.OnInteract();
							break;
						default:
							int col = param[i][0] - 'A';
							int row = param[i][1] - '1';
							int space = (row * 4) + col;
							gridButtons[space].OnInteract?.Invoke();
							break;
					}
					yield return new WaitForSeconds(0.2f);
				}
			}
			else
				yield return "sendtochat An error occured because the user inputted something wrong.";
		}
		else if (Regex.IsMatch(param[0], @"^\s*SUBMIT\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) && param.Length == 1)
		{
			yield return null;
			screen.OnInteract();
		}
		else
			yield return "sendtochat An error occured because the user inputted something wrong.";
	}
	private bool isButton(string[] param)
	{
		for (int i = 1; i < param.Length; i++)
		{
			switch (param[i])
			{
				case "UP":
				case "DOWN":
				case "U":
				case "D":
				case "A1":
				case "A2":
				case "A3":
				case "A4":
				case "B1":
				case "B2":
				case "B3":
				case "B4":
				case "C1":
				case "C2":
				case "C3":
				case "C4":
				case "D1":
				case "D2":
				case "D3":
				case "D4":
					break;
				default:
					return false;
			}
		}
		return true;
	}
}
