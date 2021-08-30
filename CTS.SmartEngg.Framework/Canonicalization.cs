// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace CTS.SmartEngg.Framework
{
    public static class Canonicalization
    {
        public static string Canonicalize(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            path = HttpUtility.UrlDecode(path);

            // Check for invalid characters
            if (path.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
            {
                throw new FileNotFoundException(string.Concat("FileName not valid: ", path));
            }

            return path;
        }
    }
}
