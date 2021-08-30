// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CTS.SmartEngg.Framework
{
    /// <summary>
    /// Place holder for all the string operations
    /// </summary>
    public static class StringOperations
    {
        static char Backslash = '\\';
        /// <summary>
        /// This is used to remove the domain prefrix and Backslash
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string RemoveDomain(string userID)
        {
            if (!string.IsNullOrEmpty(userID) && userID.Contains(Backslash))
            {
                int indexOfSlash = userID.IndexOf(Backslash);
                return userID.Substring(indexOfSlash + 1);
            }
            else
            {
                return userID;
            }
        }
    }
}
