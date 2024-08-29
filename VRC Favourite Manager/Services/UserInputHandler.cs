using System;
using System.Text.RegularExpressions;

public class UserInputHandler
{
    public string ProcessUserInput(string userInput)
    {
        userInput = SanitizeUserInput(userInput);
        return userInput;
    }

    private string SanitizeUserInput(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length > 200)
        {
            return null;
        }
        input = input.Replace("--", "");

        return Regex.Replace(input, @"[<>;'""\p{C}]", "");
    }
}