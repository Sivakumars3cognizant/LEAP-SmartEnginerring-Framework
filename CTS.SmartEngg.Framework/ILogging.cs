// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CTS.SmartEngg.Framework
{
    public interface ISmartEnggLogging
    {
        void Information(string message);
        void Warning(string message);
        void Debug(string message);
        void Error(Exception exception);
    }
}
