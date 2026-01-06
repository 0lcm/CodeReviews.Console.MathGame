Random random = new Random();

//accept user input
string? userInput;
string input;

//reusable variables
string message = "";
bool validInput = false;

//actively changed within the game
string difficulty = "";
string gamemode = "";
int score = 0;
int solution = 0;

//consts
const int problemAmount = 5;
const int maxRecordedGames = 5;

//collections
string[] gamemodes = { "+", "-", "/", "*" };
Queue<string[]> recordedGames = new Queue<string[]>();

//main game menu
bool shouldExit = false;
while (!shouldExit)
{
    Console.Clear();

    message = "Please choose an option to play, or exit to quit.";
    Console.WriteLine($"{message} \nPlay game\nPrevious games\nExit");
    input = GetUserInput(skipFirstPrompt: true, prompt: message, validResponseArray: new string[] { "play game", "previous games", "exit" });

    switch (userInput)
    {
        case "play game":
            SelectionScreen(); //assign gamemode and difficulty before calling PlayGame()
            PlayGame(gamemode, difficulty);
            break;
        case "previous games":
            ShowPreviousGames();
            break;
        case "exit":
            ExitGame();
            break;
    }
}

//retrieves and validates a user response
string GetUserInput(bool doToLower = true, bool doTrim = true, bool skipFirstPrompt = false, string prompt = "", string[] validResponseArray = null)
{
    bool doPrompt = !skipFirstPrompt; //if skipFirstPrompt == true, doPrompt = false

    do
    {
        validInput = false;

        if (doPrompt && prompt != "")
        {
            Console.WriteLine($"\n{prompt}"); 
        }

        userInput = Console.ReadLine();
        doPrompt = true; //set doPrompt to true for every other iteration

        if (userInput == null)
        {
            Console.WriteLine("Input cannot be null. try again.");
            continue;
        } 

        if (doToLower)
        {
            userInput = userInput.ToLower();
        }

        if (doTrim)
        {
            userInput = userInput.Trim();
        }

        if (validResponseArray != null && validResponseArray.Length != 0)
        {
            if (validResponseArray.Contains(userInput))
            {
                return userInput;
            }
            else
            {
                Console.WriteLine($"{userInput} is not a valid response.");
                continue;
            }
        }
        else
        {
            return userInput; //if no response array, or response array is empty, return any input
        }

    } while (!validInput);

    return userInput;
}

//User selects difficulty and gamemode
void SelectionScreen()
{
    message = "Please choose an operator to play with.. (+, -, /, *, random)";
    input = GetUserInput(prompt: message, validResponseArray: new string[] {"+", "-", "/", "*", "random"});
    gamemode = input;
    
    message = "Please choose a difficulty..(easy, medium, hard)";
    input = GetUserInput(prompt: message, validResponseArray: new string[] {"easy", "medium", "hard"});
    difficulty = input;
}

void PlayGame(string gamemode, string difficulty)
{
    bool replay = false;
    do
    {
        DateTime startTime = DateTime.Now;
        string date = startTime.ToString("d");

        for (int i = 0; i < problemAmount; i++)
        {
            validInput = false;

            string problem = ConstructProblem(gamemode, difficulty);

            Console.Clear();
            Console.WriteLine(problem);
            
            do
            {
                input = GetUserInput();
                try
                {
                    string gameResult = EvaluateWinOrLoss(input);

                    if (gameResult == "win")
                    {
                        score++;
                        Console.WriteLine($"Thats correct! Score: {score} \nPress enter to continue..");
                    }
                    else
                    {
                        Console.WriteLine($"Sorry.. it was {solution} \nPress enter to continue..");
                    }
                    Console.ReadLine();
                    validInput = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine($"{userInput} is not a valid number. Try again.");
                    continue;
                }

            } while (!validInput);
        }

        DateTime endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;
        string formattedDuration = duration.ToString(@"hh\:mm\:ss");

        RecordGame(gamemode, difficulty, date, duration: formattedDuration, score);


        validInput = false;

        Console.WriteLine($"You finished the game with a score of {score}. would you like to replay? (yes/no)");

        message = "would you like to replay? (yes/no)";
        input = GetUserInput(skipFirstPrompt: true, prompt: message, validResponseArray: new string[] { "yes", "no" });

        if (input == "yes")
        {
            replay = true;
        }
        else
        {
            replay = false;
        }

    } while (replay);


}

//returns a string of "{int} {operator} {int}" to be written, and assigns the solution to variable soltuion
string ConstructProblem(string gamemode, string difficulty)
{
    bool isSafeNumber = false;

    int maxNumber = 1;

    int number1 = 0;
    int number2 = 0;


    if (gamemode == "random")
    {
        int index = random.Next(0, gamemodes.Length - 1);
        gamemode = gamemodes[index];
    }

    //assign a maximum value for each variable based on difficulty
    switch (difficulty)
    {
        case "easy":
            maxNumber = 100;
            break;

        case "medium":
            maxNumber = 150;
            break;

        case "hard":
            maxNumber = 225;
            break;
    }

    //generate random numbers and ensure any division will have a % of 0
    do
    {
        number1 = random.Next(1, maxNumber + 1);
        number2 = random.Next(1, maxNumber + 1);


        if (gamemode == "/")
        {
            if (number1 % number2 == 0)
            {
                isSafeNumber = true;
            }
            else
            {
                isSafeNumber = false;
            }
        }
        else //allow any pair of numbers to pass when its not a divison problem
        {
            isSafeNumber = true;
        }

    } while (!isSafeNumber);

    
    switch (gamemode)
    {
        case "+":
            solution = number1 + number2;
            break;
        case "-":
            solution = number1 - number2;
            break;
        case "/":
            solution = number1 / number2;
            break;
        case "*":
            solution = number1 * number2;
            break;
    }

    return $"{number1} {gamemode} {number2}";
}
string EvaluateWinOrLoss(string userResponse)
{
    int response = int.Parse(userResponse);

    if (response == solution)
    {
        return "win";
    }

    return "lose";
}

void RecordGame(string gamemode, string difficulty, string date, string duration, int score)
{
    string[] record = new string[]
    {
        date ?? "",
        difficulty ?? "",
        duration ?? "",
        score.ToString() ?? "",
        gamemode ?? "",

    };

    recordedGames.Enqueue(record);

    if (recordedGames.Count > maxRecordedGames)
    {
        recordedGames.Dequeue();
    }
}

void ShowPreviousGames()
{
    foreach (string[] row in recordedGames)
    {
        Console.WriteLine
            ($"\n{row[0]} - {row[1]} - {row[2]}\nScore: {row[3]}\nGamemode type: {row[4]}");
    }

    if (recordedGames.Count == 0)
    {
        Console.WriteLine("There are no past games to show.");
    }

    Console.WriteLine("\nPress enter to return to the main menu..");
    Console.ReadLine();
}

void ExitGame()
{
    Console.WriteLine("\nExitting process..");
    Environment.Exit(0);
}