using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhonotacticChecker : MonoBehaviour
{
		
	
		
		public delegate bool Phonotactics (List<LetterSoundComponent> context,LetterSoundComponent l,int idxOfL);


		//returns true if we are first. (we are supposed to not be first)
		public static bool CannotBeFirst (List<LetterSoundComponent> context, LetterSoundComponent l, int idxOfL)
		{
				return idxOfL == 0 || BlankToLeft (context, idxOfL);
		
		
		}

		//returns true if we are last. (we are supposed to not be last)
		public static bool CannotBeLast (List<LetterSoundComponent> context, LetterSoundComponent l, int idxOfL)
		{
				return idxOfL == context.Count - 1 || BlankToRight (context, idxOfL);
		
		
		}


		//returns true if we are not beside a "u".
		public static bool Q (List<LetterSoundComponent> context, LetterSoundComponent l, int idxOfL)
		{
				bool result = !(IsAdjacentTo ("u", -1, idxOfL, context) || IsAdjacentTo ("u", 1, idxOfL, context));
				return result;
	
		}

		static bool BlankToLeft (List<LetterSoundComponent> context, int idx)
		{
				return idx - 1 > -1 && context [idx - 1] is Blank;
		}

		static bool BlankToRight (List<LetterSoundComponent> context, int idx)
		{
				return idx + 1 < context.Count && context [idx + 1] is Blank;
		}

		//assume that l is a consonant.
		//return true if l's neigbor is also a consonant type (this means, a con digraph, single con, or con blend)
		public static bool NoRestrictions (List<LetterSoundComponent> context, LetterSoundComponent l, int idxOfL)
		{
				return false;

		}

		public static bool ConsonantCannotPrecede (List<LetterSoundComponent> context, LetterSoundComponent l, int idxOfL)
		{
				return idxOfL - 1 > -1 && context [idxOfL - 1].IsConsonantConsonantDigraphOrBlend;


		}

		public static bool ConsonantCannotPrecedeUnlessDoubled (List<LetterSoundComponent> context, LetterSoundComponent l, int idxOfL)
		{
				if (idxOfL - 1 > -1) {
						LetterSoundComponent adj = context [idxOfL - 1];
						return  adj.IsConsonantConsonantDigraphOrBlend && !adj.LettersMatch (l);
				}
				return false;
		
		
		}

		public static bool SuffixCannotBeAgainstY (List<LetterSoundComponent> context, LetterSoundComponent l, int idxOfL)
		{


				return LastLetterSoundUnitCannotBe (context, "y");

		}

		public static bool Plurals (List<LetterSoundComponent> context, LetterSoundComponent l, int idxOfL)
		{
			
				if (SuffixCannotBeAgainstY (context, l, idxOfL))
						return true;
				return false;

				//some variation between s and es, but neither can be beside a y. 
				//(i.e., "y" word plural forms use i, then "es". puppys no; pupp i es yes.

		}

		static bool IsAdjacentTo (string asString, int leftOrRight, int to, List<LetterSoundComponent> context)
		{
				int adj = to + leftOrRight;
				if (adj > -1 && adj < context.Count)
						return context [adj].AsString.Equals (asString);
				return false;



		}

		static bool LastLetterSoundUnitCannotBe (List<LetterSoundComponent> context, string asString)
		{

				return context.Count > 0 && LastNonBlankLetter (context).AsString.Equals (asString);

		}

		static LetterSoundComponent LastNonBlankLetter (List<LetterSoundComponent> context)
		{
				for (int i=context.Count-1; i>-1; i--) {
						LetterSoundComponent l = context [i];
						if (!(l is Blank))
								return l;

				}
				return null;


		}





	   

	  


		


	   





}
