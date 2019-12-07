using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ObesenecGame {
    class Program {

        /*Ziskani hadaneho slova*/
        static List<List<char>> GetWordInList(string wordsFilePath, string lastWordFilePath)
        {
            List<List<char>> wordOut = new List<List<char>>();
            for (int i = 0; i < 3; i++) {
                //INDEX 0 - Hadane slovo, INDEX 1 - Uhadnute pismena, INDEX 2 - Spatne uhadnute pismena
                wordOut.Add(new List<char>());
            }

            //Kontrola zdali exituje soubor se slovama k hadani
            if (File.Exists(wordsFilePath)) {
                string[] wordsArr = File.ReadAllLines(wordsFilePath); //Ziskani array vsech slov na hadani
                wordsArr = wordsArr.Select(s => s.ToLower()).ToArray();//Osetreni, aby byli vsechna slova lowecase v souboru
                File.WriteAllLines(wordsFilePath, wordsArr);

                try {
                    string[] lastWordArr = File.ReadAllLines(lastWordFilePath); //Ziskani posledniho hadaneho slova
                    string lastWord = lastWordArr[0];
                    int lastWordIndex = Array.IndexOf(wordsArr, lastWord);

                    Console.WriteLine($"\n1. New game\n2. Continue from last word - '{lastWord}'");
                    string lastWordChoose = Console.ReadLine();
                    while (true) {
                        if (lastWordChoose == "1") {
                            throw new Exception();
                        } else if (lastWordChoose == "2") {
                            //Ziskani indexu posledniho hadaneho slova a vypsani slova nasledujiciho a prevedeni do pole.
                            foreach (char character in wordsArr[lastWordIndex + 1]) {
                                wordOut[0].Add(char.ToLower(character));
                                wordOut[1].Add('_');
                            }
                            break;
                        } else {
                            Console.WriteLine("Please choose from existing options.");
                            lastWordChoose = Console.ReadLine();
                        }
                    }

                } catch (Exception e) {
                    //Pokud neexistuje posledni hadane slovo nebo toto slovo bylo posledni v souboru
                    foreach (char character in wordsArr[0]) {
                        wordOut[0].Add(char.ToLower(character));
                        wordOut[1].Add('_');
                    }
                }
                
            }
            return wordOut;

        }

        /*Ulozeni posledniho hadaneho slova*/
        static void saveWord(List<List<char>> wordList, string path)
        {
            string wordToGuess = getGuessWordString(wordList);
            File.WriteAllText(path, wordToGuess);
        }

        /*Ziskani hadaneho slova ve stringu*/
        static string getGuessWordString(List<List<char>> wordList)
        {
            string wordToGuess = "";
            foreach (char letter in wordList[0]) {
                wordToGuess += letter;
            }
            return wordToGuess;
        }

        /*Odecitani zivotu*/
        static int minusLive(List<List<char>> minusLiveInfo, int lives)
        {
            if (minusLiveInfo[0][0] == 'N') {
                return lives - 1;
            } else {
                return lives;
            }
        }

        /*Ziskani pocet zivotu podle delky hadaneho slova*/
        static int getLives(List<List<char>> wordList)
        {
            if (wordList[0].Count > 6) {
                return 8;
            }else if (wordList[0].Count >= 4 && wordList[0].Count <= 6) {
                return 7;
            } else {
                return 6;
            }
        }

        /*Updatnuti listu na zaklade uhodnuti a output charu s infem jesli uhodl nebo ne (pouziti infa pro odecitani zivotu)*/
        static List<List<List<char>>> UpdateList(List<List<char>> wordList)
        {
            string wordToGuess = getGuessWordString(wordList);
            Console.WriteLine("Guess a letter (A-Z).");
            List<List<char>> minusLife = new List<List<char>>();
            minusLife.Add(new List<char>());

            //Kontrola uhadnuti
            while (true){
                string guess = Console.ReadLine().ToLower();

                if (new Regex("^[a-zA-Z]*$").IsMatch(guess) && guess.Length > 0) {
                    char[] guessChar = guess.ToCharArray();

                    if (guess.Length > 1 && wordToGuess == guess) {
                        // Uhodnuti celeho slova
                        wordList[1] = wordList[0];
                        minusLife[0].Add('Y');
                        break;
                    } else if (guess.Length == 1 && !wordToGuess.Contains(guess) && !wordList[2].Contains(guessChar[0])) {
                        // Neuhodnuti 1 pismena
                        wordList[2].Add(guessChar[0]);
                        minusLife[0].Add('N');
                        break;
                    } else if (guess.Length == 1 && (wordList[2].Contains(guessChar[0]) || wordList[1].Contains(guessChar[0]))) {
                        Console.WriteLine("This letter was already guessed");
                    } else if (guess.Length == 1) {
                        // Uhodnuti 1 pismena
                        for (int i = 0; i < wordList[0].Count; i++) {
                            if (wordList[0][i] == guessChar[0]) {
                                wordList[1][i] = wordList[0][i];
                            }
                        }
                        minusLife[0].Add('Y');
                        break;
                    } else {
                        // Neuhodnuti celeho slova
                        Console.WriteLine("This guess was't correct. Try to guess the word letter by letter");
                        minusLife[0].Add('N');
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                        break;
                    }
                } else {
                    Console.WriteLine("Your guess must contain only letters A-Z");
                }
            }
            return new List<List<List<char>>> { wordList, minusLife};
        }

        /*Ulozeni skore*/
        static void saveScore(string nickname, int rounds, string guessWord, string path)
        {
            List<string> scoreList = new List<string>();
            if (File.Exists(path)) {
                string[] scoreArr = File.ReadAllLines(path);
                scoreList = scoreArr.OfType<string>().ToList();
            }
            scoreList.Add(nickname + ";" + rounds + ";" + guessWord);
            File.WriteAllLines(path, scoreList);
        }

        /*Vypsani obrazku obesence podle poctu zivotu*/
        static void renderHangman(int lives)
        {
            switch (lives){
                case 0:
                    Console.WriteLine(@"  +---+");
                    Console.WriteLine(@"  |   |");
                    Console.WriteLine(@"  O   |");
                    Console.WriteLine(@" /|\  |");
                    Console.WriteLine(@" / \  |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"=========");
                    break;
                case 1:
                    Console.WriteLine(@"  +---+");
                    Console.WriteLine(@"  |   |");
                    Console.WriteLine(@"  O   |");
                    Console.WriteLine(@" /|\  |");
                    Console.WriteLine(@" /    |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"=========");
                    break;
                case 2:
                    Console.WriteLine(@"  +---+");
                    Console.WriteLine(@"  |   |");
                    Console.WriteLine(@"  O   |");
                    Console.WriteLine(@" /|\  |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"=========");
                    break;
                case 3:
                    Console.WriteLine(@"  +---+");
                    Console.WriteLine(@"  |   |");
                    Console.WriteLine(@"  O   |");
                    Console.WriteLine(@" /|   |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"=========");
                    break;
                case 4:
                    Console.WriteLine(@"  +---+");
                    Console.WriteLine(@"  |   |");
                    Console.WriteLine(@"  O   |");
                    Console.WriteLine(@"  |   |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"=========");
                    break;
                case 5:
                    Console.WriteLine(@"  +---+");
                    Console.WriteLine(@"  |   |");
                    Console.WriteLine(@"  O   |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"=========");
                    break;
                case 6:
                    Console.WriteLine(@"  +---+");
                    Console.WriteLine(@"  |   |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"      |");
                    Console.WriteLine(@"=========");
                    break;
                case 7:
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"=========");
                    break;
                case 8:
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"       ");
                    Console.WriteLine(@"         ");
                    break;
            }
        }

        /*Vypsani uvitani*/
        static void WellcomeText()
        {
            Console.Clear();
            Console.WriteLine("******************************************************");
            Console.WriteLine(@" _   _                                               ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"| | | |                                              ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"| |_| |  __ _  _ __    __ _  _ __ ___    __ _  _ __  ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"|  _  | / _` || '_ \  / _` || '_ ` _ \  / _` || '_ \ ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"| | | || (_| || | | || (_| || | | | | || (_| || | | |");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"\_| |_/ \__,_||_| |_| \__, ||_| |_| |_| \__,_||_| |_|");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"                       __/ |                         ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"                      |___/                          ");
            Console.WriteLine("******************************************************");
        }

        /*Vypsani rozlouceni*/
        static void ByeText()
        {
            Console.Clear();
            Console.WriteLine("******************************************************");
            Console.WriteLine(@"______               ______              ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"| ___ \              | ___ \             ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"| |_/ / _   _   ___  | |_/ / _   _   ___ ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"| ___ \| | | | / _ \ | ___ \| | | | / _ \");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"| |_/ /| |_| ||  __/ | |_/ /| |_| ||  __/");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"\____/  \__, | \___| \____/  \__, | \___|");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"         __/ |                __/ |      ");
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(@"        |___/                |___/       ");
            Console.WriteLine("******************************************************");
        }
        static void Main(string[] args)
        {
            //Vytvoreni potrebnych slozek a ziskani souboru
            string DirPath = Environment.CurrentDirectory + @"\hangmanGame_files\";
            Directory.CreateDirectory(DirPath);
            string wordsFilePath = DirPath + "words.txt";
            string lastWordFilePath = DirPath + "last_word.txt";
            string scoreFilePath = DirPath + "score.txt";

            //Uvitani
            WellcomeText();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            while (true) {

                //Ziskani hadaneho rozlozeneho slova v listu
                List<List<char>> wordList = GetWordInList(wordsFilePath, lastWordFilePath);

                if (!wordList[0].Any()) {
                    Console.WriteLine("Please create file named 'words.txt' with words to guess.");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    Environment.Exit(0);
                } else {
                    saveWord(wordList, lastWordFilePath);
                    int round = 0;
                    int lives = getLives(wordList);

                    //Grafika hraciho kola
                    while (true) {
                        Console.Clear();
                        round++;

                        Console.WriteLine($"\n******************** ROUND NUMBER {round} ********************");

                        Console.WriteLine("\n");
                        renderHangman(lives);
                        Console.WriteLine("\n");

                        Console.Write("Word to guess:  ");
                        for (int x = 0; x < wordList[1].Count; x++) {
                            Console.Write(wordList[1][x]);
                        }
                        Console.WriteLine("\n");

                        Console.Write("Incorrect letters:  ");
                        for (int o = 0; o < wordList[2].Count; o++) {
                            Console.Write(wordList[2][o] + " ");
                        }
                        Console.WriteLine("\n");

                        List<List<List<char>>>roundResult = UpdateList(wordList);
                        lives = minusLive(roundResult[1], lives);
                        wordList = roundResult[0];


                        // Kontrola vyhry
                        if (wordList[0].SequenceEqual(wordList[1]) && lives > 0) {
                            Console.Clear();

                            Console.WriteLine($"\n******************** CONGRATULATIONS ********************");
                            Console.WriteLine($"You have guessed the word in just {round} rounds.");
                            Console.WriteLine($"You have {lives} lives letf");
                            Console.WriteLine("*********************************************************");
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadKey();

                            Console.WriteLine("\nInsert your nickname please, so this score can be saved.");
                            string nickname = Console.ReadLine();
                            while (!new Regex("^[a-zA-Z0-9]*$").IsMatch(nickname) || nickname.Length == 0) {
                                Console.WriteLine("Please insert your nickname correctly, it can not contain special characters.");
                                nickname = Console.ReadLine();
                            }
                            string wordToGuess = getGuessWordString(wordList);
                            saveScore(nickname, round, wordToGuess, scoreFilePath);

                            break;
                        }else if (lives <= 0) {
                            Console.Clear();
                            Console.WriteLine($"\n*********************** GAME OVER ***********************");
                            Console.WriteLine($"It took you {round} rounds.");
                            Console.WriteLine($"The word to guess was {getGuessWordString(wordList)}");
                            Console.WriteLine("*********************************************************");
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadKey();

                            break;
                        }
                    }

                    Console.Clear();
                    Console.WriteLine("Do you want to play again Y/N");
                    string againChoose = Console.ReadLine().ToLower();
                    while (true) {
                        if (againChoose == "y") {
                            break;
                        } else if (againChoose == "n") {
                            ByeText();
                            Environment.Exit(0);
                        } else {
                            Console.WriteLine("Please choose from existing option Y - yes / N - no");
                            againChoose = Console.ReadLine().ToLower();
                        }
                    }
                }
            }
        }
    }
}