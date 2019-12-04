using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;

//Problem Manager-- not the student data manager-- should have the information about what colour coding scheme to use (since now that is associated with the problem type)
public class ProblemsRepository : MonoBehaviour
{
    Problem[] problemsForSession; //I think if we do go this way, we should change NGram class so that problems aere associated directly with them
    int currProblem = 0;

    public int ProblemsCompleted
    {
        get { return currProblem; }


    }

    public readonly int PROBLEMS_PER_SESSION = 3;
    public readonly int TOTAL_NUMBER_OF_EXPERIMENTAL_PROBLEMS_PER_PROBLEM_TYPE;
    ColourCodingScheme colourSchemeForSession = new RControlledVowel();
    public static ProblemsRepository instance = new ProblemsRepository();

    public ColourCodingScheme ActiveColourScheme
    {
        get
        {
            return colourSchemeForSession;

        }
    }


    public enum ProblemType
    {
        OPEN_CLOSED_VOWEL,
        MAGIC_E,
        SYLLABLE_DIVISION,
        CONSONANT_DIGRAPHS,
        CONSONANT_BLENDS,
        R_CONTROLLED_VOWEL,
        VOWEL_DIGRAPHS


    }

    public void Reset()
    {
        currProblem = 0;
    }


    public ColourCodingScheme GetColourCodingSchemeGivenProblemType(ProblemType type)
    {
        switch (type)
        {
            case ProblemType.OPEN_CLOSED_VOWEL:
                return new OpenClosedVowel();


            case ProblemType.CONSONANT_DIGRAPHS:
                return new ConsonantDigraphs();

            case ProblemType.CONSONANT_BLENDS:
                return new ConsonantBlends();

            case ProblemType.MAGIC_E:
                return new VowelInfluenceERule();


            case ProblemType.VOWEL_DIGRAPHS:
                return new VowelDigraphs(); //change after we make the vowel digraphs scheme


            case ProblemType.R_CONTROLLED_VOWEL:
                return new RControlledVowel();


            case ProblemType.SYLLABLE_DIVISION:
                return new SyllableDivision();

            default:
                return new NoColour();


        }


    }

    static readonly int INITIAL_WORD_IDX = 1;
    static readonly int TARGET_WORD_IDX = 0;
    static string[][][] activity_word_sets = 
    {
        new string[][]
        {
            new string[]{"pet","sad","hit"}, //target words
			new string[]{"p t","s d","h t"} //initial versions of target words
		},


        new string[][]{
            new string[]{"pup","hit","web"},
            new string[]{"p p","h t","w b"}
        },

        new string[][]{
            new string[]{"flag","crab","drop"},
            new string[]{"ag","ab","op"}
        },

        new string[][]{
            new string[]{"trip","drop","crab"},
            new string[]{"ip","op","ab"}
        },

        new string[][]{
            new string[]{"thin","shop","chip"},
            new string[]{"in","op","ip"}
        },


        new string[][]{
            new string[]{"path","wish","lunch",},
            new string[]{"pa","wi","lun"}
        },

        new string[][]{
            new string[]{"game","tape","hide"},
            new string[]{"g m","t p","h d"}
        },

        new string[][]{
            new string[]{"side","wide","late"},
            new string[]{"s d","w d","l t"}
        },

        new string[][]{
            new string[]{"leaf","boat","mail"},
            new string[]{"l  f","b  t","m  l"}
        },


        new string[][]{
            new string[]{"seat","coat","bait"},
            new string[]{"s  t","c  t","b  t"}
        },

        new string[][]{
            new string[]{"jar","horn","burn"},
            new string[]{"j","h  n","b  n"}
        },

        new string[][]{
            new string[]{"hurt","horn","part"},
            new string[]{"h  t","h  n","p  t"}
        }


    };
    static int numSessions = activity_word_sets.Length;

    public int NumSessions
    {
        get
        {
            return numSessions;
        }
    }

    void Problems(Problem[] problems)
    {
        this.problemsForSession = problems;
        idxToSwapUsedProblemIn = problems.Length - 1;
    }

    int idxToSwapUsedProblemIn;

    public void Initialize(int sessionIndex)
    {
        InitializeProblems(sessionIndex);
        SetSessionColourScheme(sessionIndex);


    }

    void InitializeProblems(int sessionIndex)
    {

        string[][] wordsForSessionProblems = activity_word_sets[sessionIndex % activity_word_sets.Length];
        problemsForSession = new Problem[PROBLEMS_PER_SESSION];


        for (int i = 0; i < PROBLEMS_PER_SESSION; i++)
        {

            problemsForSession[i] = new Problem(wordsForSessionProblems[INITIAL_WORD_IDX][i], wordsForSessionProblems[TARGET_WORD_IDX][i]);
        }

    }

    void SetSessionColourScheme(int sessionIndex)
    {
        switch (sessionIndex)
        {
            case 0:
            case 1:
                colourSchemeForSession = new OpenClosedVowel();
                return;
            case 2:
            case 3:
                colourSchemeForSession = new ConsonantBlends();
                return;
            case 4:
            case 5:
                colourSchemeForSession = new ConsonantDigraphs();
                return;

            case 6:
            case 7:

                colourSchemeForSession = new VowelInfluenceERule();
                return;
            case 8:
            case 9:

                colourSchemeForSession = new VowelDigraphs();
                return;
            case 10:
            case 11:

                colourSchemeForSession = new RControlledVowel();
                return;
                //case 11:
                //colourSchemeForSession = new SyllableDivision ();

                return;
            default:
                colourSchemeForSession = new NoColour();
                return;

        }




    }
    /* only if we want sub directories for words for different types of problems (slightly faster searching)*/
    string GetPathToWord()
    {
        return "audio/words/";
    }

    public Problem GetNextProblem()
    {
        if (!AllProblemsDone())
        {
            Problem result = problemsForSession[currProblem];
            currProblem++;
            return result;
        }
        else
            return problemsForSession[0];


    }

    public bool AllProblemsDone()
    {
        return currProblem == problemsForSession.Length;

    }

    public Problem GetRandomProblem()
    {
        if (idxToSwapUsedProblemIn == 0)//loop if necessary
            idxToSwapUsedProblemIn = problemsForSession.Length - 1;

        int upper = idxToSwapUsedProblemIn - 1;
        int randomIdx = (int)UnityEngine.Random.Range(0, upper);
        Problem next = problemsForSession[randomIdx];
        Problem temp = problemsForSession[idxToSwapUsedProblemIn];
        problemsForSession[randomIdx] = temp;
        problemsForSession[idxToSwapUsedProblemIn] = next;
        idxToSwapUsedProblemIn--;
        next.Reset();
        return next;

    }




}
