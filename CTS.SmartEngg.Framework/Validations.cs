// Copyright (c) Cognizant. All Rights Reserved.

using System.Text.RegularExpressions;
namespace CTS.SmartEngg.Framework
{
    public static class Validations
    {
        public static bool ValidFilePath(string filePath, out string validpath)
        {
            validpath = string.Empty;
            Regex rgx = new Regex("(\\\\?([^\\/]*[\\/])*)([^\\/]+)");
            if (filePath != null && rgx.IsMatch(filePath))
            {
                validpath = filePath.Replace("..", "").Replace(">", "").Replace("<", "");
                return true;
            }
            return false;
        }
    }
}

