// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.ComponentModel.DataAnnotations;

namespace CTS.SmartEngg.Framework
{

    /// <summary>
    /// To filter the special charecters in the Input Parameter!
    /// </summary>
    public sealed class SanitizeString
    {
        /// <summary>
        /// Constuctor to Sanitize the Input
        /// </summary>
        /// <param name="s"></param>
        public SanitizeString(dynamic input)
        {
            string inputValue = Convert.ToString(input);
            Value = inputValue.Replace("<", "").Replace(">", "");
        }

        private string values;

        /// <summary>
        /// gets or sets the value
        /// </summary>
        [MaxLength(int.MaxValue)]
        public string Value
        {
            get { return values; }
            set { values = value; }
        }
        /// <summary>
        /// Implicit Operator to Sanitize the Input
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator SanitizeString(string s)
        {
            return new SanitizeString(s);
        }
    }
}
