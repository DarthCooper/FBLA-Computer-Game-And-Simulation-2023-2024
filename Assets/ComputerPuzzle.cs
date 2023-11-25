using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ComputerPuzzle : MonoBehaviour
{
    public static ComputerPuzzle Instance;

    [Header("Generate Puzzles")]

    public GameObject textPrefab;

    public Dictionary<string, PuzzleInfoSO> puzzles = new Dictionary<string, PuzzleInfoSO>();

    public string currentPuzzleID;

    public TMP_Text puzzleQuestion;

    public Transform context;

    public List<GameObject> spawnedText = new List<GameObject>();

    public int extraAnswers = 19;

    public int questionAmount = 3;
    public int questionsAnswered = 0;

    [Header("Add Extra Symbols")]
    public bool useExtraSymbols;

    public int lengthOfEachText = 20;

    public string[] extraSymbols;

    [Header("CheckAnswer")]
    public TMP_InputField input;

    public Indicator[] indicators;

    public int maxAttempts = 5;
    public int totalAttempts = 0;

    public Indicator[] attemptIndicators;

    public Firewall firewall;

    private void Awake()
    {
        Instance = this;
        DisableComputer();
    }

    public void DisableComputer()
    {
        foreach (var image in GetComponentsInChildren<Image>())
        {
            image.enabled = false;
        }
        foreach (var image in GetComponentsInChildren<RawImage>())
        {
            image.enabled = false;
        }
        foreach (var text in GetComponentsInChildren<TMP_Text>())
        {
            text.enabled = false;
        }
        if (Manager.Instance.GetLocalPlayer())
        {
            if (Manager.Instance.GetLocalPlayer().GetComponent<PlayerInteract>())
            {
                Manager.Instance.GetLocalPlayer().GetComponent<PlayerInteract>().computerOpen = false;
            }
        }
        input.DeactivateInputField();
    }

    public void EnableComputer(Firewall firewall)
    {
        totalAttempts = 0;
        questionsAnswered = 0;
        this.firewall = firewall;
        foreach (var image in GetComponentsInChildren<Image>())
        {
            image.enabled = true;
        }
        foreach (var image in GetComponentsInChildren<RawImage>())
        {
            image.enabled = true;
        }
        foreach (var text in GetComponentsInChildren<TMP_Text>())
        {
            text.enabled = true;
        }
        input.ActivateInputField();
        input.pointSize = 38;
        if(Manager.Instance.GetLocalPlayer())
        {
            if(Manager.Instance.GetLocalPlayer().GetComponent<PlayerInteract>())
            {
                Manager.Instance.GetLocalPlayer().GetComponent<PlayerInteract>().computerOpen = true;
            }
        }
        StartPuzzle();
    }

    public void StartPuzzle()
    {
        for(int i = questionsAnswered; i < indicators.Length; i++)
        {
            indicators[i].ChangeIndicator(false);
        }
        for(int i = totalAttempts; i < attemptIndicators.Length; i++)
        {
            attemptIndicators[i].ChangeIndicator(false);
        }
        puzzles = CreatePuzzleMap();
        foreach (var text in spawnedText)
        {
            Destroy(text);
        }
        spawnedText.Clear();
        int questionIndex = Random.Range(0, puzzles.Count);
        for (int i = 0; i < puzzles.Count; i++)
        {
            if(i == questionIndex)
            {
                currentPuzzleID = puzzles.ElementAt(i).Key;
            }
        }
        puzzleQuestion.text = puzzles[currentPuzzleID].question;
        List<string> falseAnswers = new List<string>();
        foreach (var item in puzzles.Values)
        {
            if(item.answer != puzzles[currentPuzzleID].answer)
            {
                falseAnswers.Add(item.answer);
            }
        }
        List<string> answers = new List<string>();
        answers.Add(puzzles[currentPuzzleID].answer);
        for (int i = 0; i < extraAnswers; i++)
        {
            int random = Random.Range(0, falseAnswers.Count);
            answers.Add(falseAnswers[random]);
            falseAnswers.RemoveAt(random);
        }
        for (int i = 0; i < extraAnswers + 1; i++)
        {
            int random = Random.Range(0, answers.Count);
            var text = Instantiate(textPrefab, context);
            if(useExtraSymbols)
            {
                string[] strings = new string[lengthOfEachText - answers[random].Length];
                strings[0] = "<u>" + answers[random] + "</u>";
                for(int j = 1; j < strings.Length; j++)
                {
                    int stringRandom = Random.Range(0, extraSymbols.Length);
                    strings[j] = extraSymbols[stringRandom];
                }
                string stringText = "";
                List<string> possibleStrings = strings.ToList();
                for (int j = 0; j < strings.Length; j++)
                {
                    int pos = Random.Range(0, possibleStrings.Count);
                    stringText += possibleStrings[pos];
                    possibleStrings.RemoveAt(pos);
                }
                text.GetComponent<TMP_Text>().text = stringText;
            }else
            {
                text.GetComponent<TMP_Text>().text = answers[random];
            }
            answers.RemoveAt(random);
            spawnedText.Add(text);
        }
    }

    public bool CheckAnswer()
    {
        string Input = input.text.ToLower();
        string answer = puzzles[currentPuzzleID].answer.ToLower();
        answer = answer.Replace("_", "");
        answer = answer.Replace(" ", "");
        Input = Input.Replace("_", "");
        Input = Input.Replace(" ", "");
        print(answer +  "\n" + Input);
        if(answer.Equals(Input))
        {
            return true;
        }else
        {
            return false;
        }
    }

    public void SubmitResponse()
    {
        if(CheckAnswer())
        {
            print("Correct");
            StartCoroutine(nameof(CorrectAnimation));
        }else
        {
            print("Incorrect. The answer is" + puzzles[currentPuzzleID].answer);
            StartCoroutine(nameof(IncorrectAnimation));
        }
    }

    IEnumerator CorrectAnimation()
    {
        input.interactable = false;
        input.text = "";
        yield return new WaitForSeconds(0.25f);
        input.text = "Correct";
        yield return new WaitForSeconds(0.25f);
        input.text = "";
        yield return new WaitForSeconds(0.25f);
        input.text = "Correct";
        yield return new WaitForSeconds(0.25f);
        input.text = "";
        yield return new WaitForSeconds(0.25f);
        input.text = "Correct";
        yield return new WaitForSeconds(0.25f);
        input.text = "";
        Correct();
    }

    public void Correct()
    {
        input.interactable = true;
        questionsAnswered++;
        totalAttempts = 0;
        indicators[questionsAnswered - 1].ChangeIndicator(true);
        if (questionsAnswered < questionAmount)
        {
            input.text = "";
            input.ActivateInputField();
            indicators[questionsAnswered - 1].ChangeIndicator(true);
            StartPuzzle();
        }
        else
        {
            DisableComputer();
            firewall.DisableFirewall();
        }
    }

    IEnumerator IncorrectAnimation()
    {
        input.interactable = false;
        input.text = "";
        yield return new WaitForSeconds(0.25f);
        input.text = "Incorrect";
        yield return new WaitForSeconds(0.25f);
        input.text = "";
        yield return new WaitForSeconds(0.25f);
        input.text = "Incorrect";
        yield return new WaitForSeconds(0.25f);
        input.text = "";
        yield return new WaitForSeconds(0.25f);
        input.text = "Incorrect";
        yield return new WaitForSeconds(0.25f);
        input.text = "";
        Incorrect();
    }

    public void Incorrect()
    {
        input.interactable = true;
        input.ActivateInputField();
        totalAttempts++;
        attemptIndicators[totalAttempts - 1].ChangeIndicator(true);
        if (totalAttempts >= maxAttempts)
        {
            if (questionsAnswered >= 1)
            {
                questionsAnswered--;
                indicators[questionsAnswered].ChangeIndicator(true);
            }
            totalAttempts = 0;
            StartPuzzle();
        }
    }

    private Dictionary<string, PuzzleInfoSO> CreatePuzzleMap()
    {
        PuzzleInfoSO[] allPuzzles = Resources.LoadAll<PuzzleInfoSO>("Puzzles");
        Dictionary<string, PuzzleInfoSO> idToQuestMap = new Dictionary<string, PuzzleInfoSO>();
        foreach (PuzzleInfoSO puzzleInfo in allPuzzles)
        {
            if (idToQuestMap.ContainsKey(puzzleInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + puzzleInfo.id);
            }
            idToQuestMap.Add(puzzleInfo.id, puzzleInfo);
        }
        return idToQuestMap;
    }
}
