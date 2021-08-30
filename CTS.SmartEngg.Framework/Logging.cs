// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Web;

namespace CTS.SmartEngg.Framework
{
    public class SmartEnggLogging: ISmartEnggLogging
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public SmartEnggLogging()
        {
        }

        public void Information(string message)
        {
            logger.Info(message);
        }

        public void Warning(string message)
        {
            logger.Warn(message);
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Error(Exception exception)
        {
            logger.Error(exception);
        }
    }
}
