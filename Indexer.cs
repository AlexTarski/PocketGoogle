using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PocketGoogle;

public class Indexer : IIndexer
{
	private Dictionary<string, Dictionary<int, List<int>>> indexedWords = new();

	public void Add(int id, string documentText)
	{
		int start = 0;
		int position;
		char[] delimeters = new char[] { ' ', '.', ',', '!', '?', ':', '-', '\r', '\n' };

        do
        {
            position = documentText.IndexOfAny(delimeters, start);
            if (position >= 0)
            {
				SearchAndAdd(id, documentText, start, position);
				start = position + 1;
            }
        } while (position > 0);

		if (start <= documentText.Length - 1 && char.IsLetter(documentText[start]))
		{
            SearchAndAdd(id, documentText, start, documentText.Length);
        }
    }

	public void SearchAndAdd (int id, string documentText, int start, int position)
	{
			string word = documentText.Substring(start, position - start).Trim();
			if (indexedWords.ContainsKey(word))
			{
				if (indexedWords[word].ContainsKey(id))
				{
					List<int> temp = indexedWords[word][id];
					temp.Add(start);
                    indexedWords[word][id] = temp;
                }
               else
				{
					indexedWords[word].Add(id, new List<int>() { start });
                }
            }
            else
			{
				indexedWords.Add(word, new Dictionary<int, List<int>>() { { id, new List<int>() { start } } });
			}
	}

	public List<int> GetIds(string word)
	{
		List<int> result = new ();
		if (indexedWords.ContainsKey(word))
		{
			foreach(var id in indexedWords[word])
			{
				result.Add(id.Key);
			}
		}

		return result;
	}

	public List<int> GetPositions(int id, string word)
	{
		if (indexedWords.ContainsKey(word))
		{
			if (indexedWords[word].ContainsKey(id))
			{
				return indexedWords[word][id];
			}
			else { return new List<int>();}
		}
		return new List<int>();
	}

	public void Remove(int id)
	{
		foreach(var word in indexedWords)
		{
			if(word.Value.ContainsKey(id))
			{
				word.Value.Remove(id);
			}
		}
	}
}