using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MonKey.Extensions
{

    public static class StringExt
    {
        public static string GetSafeForFileName(this string name)
        {

            return name.ReplaceAll(new[] { "/", "?", "<", ">", "*", ":", "|", "\\", "\"" }, "", false);
        }

        public static bool IsNullOrEmpty(this string toCheck)
        {
            return String.IsNullOrEmpty(toCheck);
        }

        public static string Replace(this string name, string term, string toReplaceWith, bool ignoreCase)
        {
            return Regex.Replace(name, Regex.Escape(term), toReplaceWith,
                (ignoreCase) ? RegexOptions.IgnoreCase : RegexOptions.CultureInvariant);
        }

        public static string ReplaceAll(this string name, string[] terms, string toReplaceWith, bool ignoreCase)
        {
            foreach (var term in terms)
            {
                name = name.Replace(term, toReplaceWith, ignoreCase);
            }
            return name;
        }

        public static string ReplaceUntil(this string text, string term, string replacement)
        {
            int id = text.IndexOf(term, StringComparison.InvariantCultureIgnoreCase);
            string sub = text.Substring(id, text.Length - id);
            return replacement + sub;
        }

        public static string ReplaceFromLast(this string name, string separator, string toReplaceWith)
        {
            string[] splits = name.Split(new[] { separator }, StringSplitOptions.None);

            StringBuilder output = new StringBuilder();

            for (int i = 0; i < splits.Length; i++)
            {
                if (i == splits.Length - 1)
                {
                    output.Append(toReplaceWith);
                }
                else
                {
                    output.Append(splits[i]);
                    if (i != splits.Length - 2)
                    {
                        output.Append(separator);
                    }
                }
            }
            return output.ToString();
        }

        public static string ReplaceAfterLast(this string name, string separator, string toReplaceWith)
        {
            string[] splits = name.Split(new[] { separator }, StringSplitOptions.None);

            StringBuilder output = new StringBuilder();

            for (int i = 0; i < splits.Length; i++)
            {
                if (i == splits.Length - 1)
                {
                    output.Append(separator + toReplaceWith);
                }
                else
                {
                    output.Append(splits[i]);
                    if (i != splits.Length - 2)
                    {
                        output.Append(separator);
                    }
                }
            }
            return output.ToString();
        }


        public static string ReplaceFromFirst(this string name, string separator, string toReplaceWith)
        {
            string[] splits = name.Split(new[] { separator }, StringSplitOptions.None);

            if (splits.Length > 0)
            {
                return splits[0] + toReplaceWith;
            }

            return "";
        }

        public static string GetAssetNameFromPath(this string assetPath, bool includeExtension = false)
        {
            int lastIndexOfSlash = assetPath.LastIndexOf("/", StringComparison.Ordinal);
            int lastIndexOfDot = assetPath.LastIndexOf(".", StringComparison.Ordinal);

            if (lastIndexOfDot < 0 || lastIndexOfSlash < 0 || lastIndexOfDot - lastIndexOfSlash - 1 < 0)
            {
                return assetPath;
            }

            if (!includeExtension)
            {
                return assetPath.Substring(lastIndexOfSlash + 1, lastIndexOfDot - lastIndexOfSlash - 1);
            }

            return assetPath.Substring(lastIndexOfSlash + 1, assetPath.Length - lastIndexOfSlash - 1);

        }

        public static string GetDirectoryNameFromPath(this string directoryPath)
        {
            int lastIndexOfSlash = directoryPath.LastIndexOf("/", StringComparison.Ordinal);
            if (lastIndexOfSlash == -1 || lastIndexOfSlash + 1 >= directoryPath.Length)
            {
                return directoryPath;
            }

            return directoryPath.Substring(lastIndexOfSlash + 1);
        }

        public static string Directorized(this string directory)
        {
            if (!directory.EndsWith("/"))
            {
                return directory + "/";
            }
            return directory;
        }

        public static string AsFolder(this string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        public static readonly string BoldTag = "<b>";
        public static readonly string BoldTagClosing = "</b>";

        public static readonly string ItalicTag = "<i>";
        public static readonly string ItalicTagClosing = "</i>";

        public static readonly string ColorTagClosing = "</color>";

        public static string ColorTag(Color color)
        {
            return new StringBuilder().Append("<color=").Append(color.ToHtml())
                .Append(">").ToString();
        }

        public static string Italic(this string str)
        {
            return new StringBuilder().Append("<i>").Append(str).Append("</i>").ToString();
        }

        public static string Bold(this string str)
        {
            return new StringBuilder().Append("<b>").Append(str).Append("</b>").ToString();
        }

        public static string Colored(this string str, Color color)
        {
            return new StringBuilder().Append("<color=").Append(color.ToHtml()).Append(">").Append(str)
                .Append("</color>").ToString();
        }

        public static string Highlight(this string text, string searchTerms,
            bool considerSearchTermsAsWords = true,
            string nonHighlightPrefix = "", string nonHighlightSuffix = "",
            string highlightPrefix = "<b><color=#00EE00>",
            string highlightSuffix = "</color></b>")
        {

            List<int> highlighIDs = new List<int>();

            if (considerSearchTermsAsWords)
            {
                foreach (var term in searchTerms.Split(new[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    int index = text.ToLower().IndexOf(term.ToLower(), StringComparison.Ordinal);
                    if (index >= 0 && !highlighIDs.Contains(index))
                    {
                        for (int i = 0; i < term.Length; i++)
                        {
                            highlighIDs.Add(index + i);
                        }
                    }

                    /*  foreach (var i in text.AllSubIndexesOf(term))
                      {
                          if (!highlighIDs.Contains(i))
                              highlighIDs.AddRange(text.AllSubIndexesOf(term));
                      }*/
                }
            }
            else
            {
                highlighIDs.AddRange(text.AllSubIndexesOf(searchTerms));
            }

            if (!highlighIDs.Any())
            {
                return nonHighlightPrefix + text + nonHighlightSuffix;
            }

            var sb = new StringBuilder();
            int previousID = -2;

            highlighIDs.Sort();

            foreach (var id in highlighIDs)
            {
                if (id > 0 && previousID == -2)
                {
                    sb.Append(nonHighlightPrefix);
                    sb.Append(text.Substring(0, id));
                    sb.Append(nonHighlightSuffix);
                }

                if (id - 1 != previousID)
                {
                    if (previousID != -2)
                    {
                        sb.Append(highlightSuffix);
                        sb.Append(nonHighlightPrefix);
                        if (id - previousID - 1 > 0)
                        {
                            sb.Append(text.Substring(previousID + 1, id - previousID - 1));
                        }

                        sb.Append(nonHighlightSuffix);
                    }
                    sb.Append(highlightPrefix);
                    sb.Append(text[id]);
                }
                else
                {
                    sb.Append(text[id]);
                }
                previousID = id;
            }

            sb.Append(highlightSuffix);
            if (previousID != text.Length - 1)
            {
                sb.Append(nonHighlightPrefix);
                sb.Append(text.Substring(previousID + 1, text.Length - previousID - 1));
                sb.Append(nonHighlightSuffix);
            }

            return sb.ToString();
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException("the string to find may not be empty", "value");
            }

            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    return indexes;
                }

                indexes.Add(index);
            }
        }

        /// <summary>
        /// returns indexes of all the subletters part of the string to lpok for
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toSearchFor"></param>
        /// <param name="searchInitials"></param>
        /// <returns></returns>
        public static List<int> AllSubIndexesOf(this string item, string toSearchFor, bool searchInitials = true)
        {
            List<int> idMatches = new List<int>();
            List<int> indexes = item.AllIndexesOf(toSearchFor);

            foreach (var index in indexes)
            {
                if (index != -1)
                {
                    idMatches.AddRange(index, index + toSearchFor.Length - 1);
                }
            }

            if (!searchInitials)
            {
                return idMatches;
            }

            List<int> initialsIds = new List<int>();
            var initials = GetVariableInitials(item, initialsIds);
            if (initials.Equals(toSearchFor, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var id in initialsIds)
                {
                    if (!idMatches.Contains(id))
                    {
                        idMatches.Add(id);
                    }
                }
            }
            return idMatches;
        }

        /// <summary>
        /// Orders a set of string by search terms using a custom fast fuzzy search
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="includeZeroScore"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<string> OrderStringsBySearchScore(IEnumerable<string> strings, bool includeZeroScore = false,
            params string[] searchTerms)
        {
            Dictionary<string, int> scores = new Dictionary<string, int>();

            var enumerable = strings as string[] ?? strings.ToArray();
            foreach (string s in enumerable)
            {
                int score = SentenceSearchScore(s, searchTerms);

                if (score > 0 || includeZeroScore)
                {
                    scores.Add(s, score);
                }
            }

            return enumerable.Where(_ => scores.ContainsKey(_)).OrderByDescending(_ => scores[_]).ThenBy(_ => _.Length);
        }


        /// <summary>
        /// returns the initials of a string that is camel cased 
        /// (typically any c# variable that follows the usual naming conventions)
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="initialIds"></param>
        /// <returns></returns>
        public static string GetVariableInitials(this string variable, List<int> initialIds = null)
        {
            var initials = new StringBuilder();
            initials.Append(variable[0].ToString().ToUpper());

            if (initialIds != null)
            {
                initialIds.Clear();
                initialIds.Add(0);
            }

            for (int i = 1; i < variable.Length; i++)
            {
                if (Char.IsUpper(variable[i]))
                {
                    initials.Append(variable[i]);
                    if (initialIds != null)
                    {
                        initialIds.Add(i);
                    }
                }
            }
            return initials.ToString();
        }

        /// <summary>
        /// Removes any terms that may have been created by cloning an object by Unity
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RemoveUnityCloneTerms(string name)
        {
            return Regex.Replace(name.Replace("(Clone)", "", true), "\\ (\\([0-9]*\\))", "").Trim();
        }

        /// <summary>
        /// Transforms a Camel-case name into a nice group of words known as a sentence
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string NicifyVariableName(this string name)
        {
            StringBuilder outputName = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0)
                {
                    outputName.Append(Char.ToUpper(name[i]));
                }
                else
                {
                    if (Char.IsUpper(name[i]) && !Char.IsUpper(name[i - 1]))
                    {
                        outputName.Append(' ');
                    }
                    outputName.Append(name[i]);
                }
            }

            if (outputName[outputName.Length - 1] == ' ')
            {
                outputName = outputName.Remove(outputName.Length - 1, 1);
            }

            return outputName.ToString();
        }

        public static int SearchScoringFirstLetter = 20;
        public static int SearchScoringInitialLettersInRow = 20;

        public static int SearchScoringLettersInRow = 10;

        //so that 3 letters in a row is as strong as having the first letter of the word.
        public static float SearchScoringLettersInRowGrowth = 1.5f;

        public static int SearchScoringFullInitials = 400;
        public static int SearchScoringFullWord = 800;

        public static int BonusWordStart = 300;
        public static int MalusPerWordDelay = 60;

        public static int SearchScoringMissingLetter = 0;
        public static int SearchScoringMissingLetterFromText = 0;

        public static int SearchScoringFirstWord = 10;

        public static int SentenceSearchScore(this string text, params string[] searchTerms)
        {
            var texts = new [] {text};
            return StringSearchScore(text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries), searchTerms);
        }

        public static int StringSearchScore(this string[] words,
            params string[] searchTerm)
        {
            int totalScore = 0;

            foreach (var term in searchTerm)
            {
                int score = -1;
                foreach (var word in words)
                {
                    if (score == -1)
                    {
                        int subScore = WordSearchScore(word.ToLower(), false, term.ToLower());

                        if (subScore > 0)
                        {
                            score = subScore + SearchScoringFirstWord;
                        }
                        else
                        {
                            score = 0;
                        }
                    }
                    else
                    {
                        score = Math.Max(score, WordSearchScore(word.ToLower(), false, term.ToLower()));
                    }
                }
                totalScore += score;
            }

            return totalScore;
        }


        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        public static int WordSearchScore(this string s, bool max = false, params string[] searchTerms)
        {
            int score = 0;
            foreach (var item in searchTerms)
            {
                if (max)
                {
                    score = Math.Max(score, StringSearchScore(s.ToLower(), item.ToLower()));
                }
                else
                {
                    score += StringSearchScore(s.ToLower(), item.ToLower());
                }
            }

            return score;
        }

        public static int StringSearchScore(this string s, string searchTerm, bool initials = false)
        {
           //return Fuzz.PartialTokenSortRatio(s.ToLower(), searchTerm.ToLower());

            int score = 0;
            int initialsScore = 0;

            /*  if (!initials)
              {
                  string initialLetters = s.GetVariableInitials().ToLower();
                  initialsScore = StringSearchScore(initialLetters, searchTerm, true);
              }*/

            if (s == searchTerm && initials)
            {
                return initials ? SearchScoringFullInitials : SearchScoringFullWord;

            }

            if (s.Contains(searchTerm))
            {
                score += s.Length > searchTerm.Length
                    ? (s.Length - searchTerm.Length) * SearchScoringMissingLetter
                    : (searchTerm.Length - s.Length) * SearchScoringMissingLetterFromText;

                score += (int)Math.Pow(SearchScoringLettersInRowGrowth, searchTerm.Length) *
                         ((initials) ? SearchScoringInitialLettersInRow
                             : SearchScoringLettersInRow);

                int index = s.IndexOf(searchTerm, StringComparison.Ordinal);
                if (index >= 0)
                {
                    score += Math.Max(0, BonusWordStart - index * MalusPerWordDelay);
                }
            }

            score = score + initialsScore;
            return Math.Max(score, 0);
        }

        public struct Match
        {
            public string TextValue;
            public List<int> Positions;
        }

        /// <summary>
        /// Full Fuzzy Search (more costly, but exact fuzzy)
        /// </summary>
        /// <param name="resultSet"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<Match> MatchResultSet(IEnumerable<string> resultSet, string query)
        {
            if (query == string.Empty)
            {
                return null;
            }

            var tokens = query.ToCharArray();
            var matches = new List<Match>();

            foreach (var result in resultSet)
            {
                var tokenIndex = 0;
                var resultCharIndex = 0;
                var matchedPositions = new List<int>();

                while (resultCharIndex < result.Length)
                {
                    if (char.ToLower(result[resultCharIndex]) == char.ToLower(tokens[tokenIndex]))
                    {
                        matchedPositions.Add(resultCharIndex);
                        tokenIndex++;

                        if (tokenIndex >= tokens.Length)
                        {
                            var match = new Match()
                            {
                                TextValue = result,
                                Positions = matchedPositions
                            };

                            matches.Add(match);
                            break;
                        }
                    }

                    resultCharIndex++;
                }
            }

            return matches;
        }
    }

}
