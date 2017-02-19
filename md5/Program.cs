using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace md5
{
    class Program
    {
        const string anagramPhrase = "poultryoutwitsants";

        static void Main(string[] args)
        {
            
            int[] basePhraseCharOccurence = new int[26];
            basePhraseCharOccurence = CalculateCharacterOccurence(anagramPhrase);
            checkAllCombinations(basePhraseCharOccurence);
            Console.WriteLine("Finished");
            Console.ReadKey();
        }

        /* Checks all possible combinations of words that fit the criteria 
        Criterium 1: The character occurence of the word combination cannot be greater than the character occurence of the anagramPhrase
        Criterium 2: The length of the word combination cannot be greater than the length of the anagramPhrase
        Criterium 3: The md5 hash of the word combination has to be the same as one of the solutions */
        public static void checkAllCombinations(int[] basePhraseCharOccurence)
        {
            Console.WriteLine("Searching this might take around 30 seconds");
            List<string> filteredWords = readAndFilterFile(basePhraseCharOccurence); //Words from the filtered file of all english words

            int[] anagramPhraseCharOccur = CalculateCharacterOccurence(anagramPhrase);
            int[] _anagramPhraseCOCopy = CalculateCharacterOccurence(anagramPhrase); //Copy needed to reset charOccur to its base state
            for (int i = 0; i < filteredWords.Count - 2; i++)
            {
                int[] word1 = CalculateCharacterOccurence(filteredWords[i]);

                for (int l = 0; l < 26; l++) //Subtract letters of word1 from anagramPhrase
                {
                    anagramPhraseCharOccur[l] -= word1[l];
                }

                replaceArrays(ref _anagramPhraseCOCopy, anagramPhraseCharOccur);
                for (int j = i + 1; j < filteredWords.Count - 1; j++)
                {
                    int[] word2 = CalculateCharacterOccurence(filteredWords[j]);

                    bool flag = false;

                    for (int l = 0; l < 26; l++) //checks if the letters from word2 can be subtracted from the letters of anagramPhrase
                    {
                        if ((anagramPhraseCharOccur[l] - word2[l]) < 0)
                        {
                            flag = true; //if not raise flag and continue
                            break;
                        }
                    }

                    if (flag == false)
                    {
                        for (int l = 0; l < 26; l++)
                        {
                            anagramPhraseCharOccur[l] -= word2[l];
                        }

                    }
                    else
                    {
                        replaceArrays(ref anagramPhraseCharOccur, _anagramPhraseCOCopy); //reset anagramPhraseCharOccur and continue
                        continue;
                    }

                    for (int k = j + 1; k < filteredWords.Count; k++)
                    {
                        int[] word3 = CalculateCharacterOccurence(filteredWords[k]);
                        for (int n = 0; n < 26; n++)
                        {
                            //All the remainig letters in anagramPhaseCharOccur have to be subtracted from word3 so all the letters of the anagram phrase are used up
                            if (anagramPhraseCharOccur[n] - word3[n] != 0) 
                            {
                                break;
                            }
                            else if (n == 25)
                            {
                                checkSolution1(filteredWords[i], filteredWords[j], filteredWords[k]);
                                checkSolution2(filteredWords[k], filteredWords[i], filteredWords[j]);
                            }
                        }
                    }
                }

                replaceArrays(ref anagramPhraseCharOccur, basePhraseCharOccurence); //reset character occurence to default
            }
        }



        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashedBytes = md5.ComputeHash(inputBytes);

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sb = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    sb.Append(hashedBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /* Calculates the occurence of each letter of the alphabet in a given word.
           Returns an integer array of size 26 where each index corresponds to a letter of the alphabet
           Used to check if words or combinations of words can be constructed from a series of letters
        */
        public static int[] CalculateCharacterOccurence (string input)
        {
            int[] charOccurence = new int[26];

            foreach (var character in input)
            {
                if((byte)character != 39 && (byte)character < 197)
                {
                    charOccurence[character - 'a']++;
                }  
            }

            return charOccurence;
        }


        //Replaces array1 by array 2
        public static void replaceArrays (ref int[] array1, int[] array2)
        {
            for (int i = 0; i < 26; i++)
            {
                array1[i] = array2[i];
            }
        }

        public static List<string> readAndFilterFile(int[] basePhraseCharOccurence)
        {
            List<string> results = new List<string>(); //filtered out words from the word list

            string word;

            // Read the file line by line.
            System.IO.StreamReader file =
            new System.IO.StreamReader("..\\..\\WordList\\wordlist");
            while ((word = file.ReadLine()) != null)
            {
                int[] wordCharOccurence = new int[26];
                wordCharOccurence = CalculateCharacterOccurence(word);

                for (int i = 0; i < 26; i++)
                {
                    //Checks if the word can be constructed from the letters of the anagram phrase
                    if (wordCharOccurence[i] > basePhraseCharOccurence[i])
                    {
                        break;
                    }
                    else if (i == 25 && !word.Contains("'")) //only add the word to results if it does not contain an apostrophe
                    {
                        results.Add(word);
                    }
                }

            }

            file.Close();

            return results;
        }

        public static void checkSolution1(string word1, string word2, string word3)
        {
                var md5Hash = CreateMD5(word1 + " " + word2 + " " + word3);
                if (md5Hash == "e4820b45d2277f3844eac66c903e84be".ToUpper())
                {
                    Console.WriteLine($"Solution 1 is: {word1} {word2} {word3}");
                    Console.WriteLine(md5Hash);
                }

            
        }

        public static void checkSolution2(string word1, string word2, string word3)
        {
                var md5Hash = CreateMD5(word1 + " " + word2 + " " + word3);
                if (md5Hash == "23170acc097c24edb98fc5488ab033fe".ToUpper())
                {
                    Console.WriteLine($"Solution 2 is: {word1} {word2} {word3}");
                    Console.WriteLine(md5Hash);
                }

            
        }

       


    }


}
