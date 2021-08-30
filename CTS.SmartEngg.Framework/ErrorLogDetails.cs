// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CTS.SmartEngg.Framework
{
    public class ErrorLogDetails
    {
       public string LogSeverity { get; set; }
        public string LogLevel { get; set; }
        public string HostName { get; set; }
        public string AssociateId { get; set; }
        public string CreatedDate { get; set; }
        public string ProjectId { get; set; }
        public string Technology { get; set; }
        public string ModuleName { get; set; }
        public string FeatureName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public int ProcessId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string AdditionalField_1 { get; set; }
        public string AdditionalField_2 { get; set; }
    }



    public enum LogSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
    public enum LogLevels
    {
        Info,
        Audit,
        Error,
        Warn,
        Debug
    }
    public enum Technology
    {
         Angular,
         AngularJS,
         CSharp,
         DotNet,
         DotNetCore,
         Java,
         JavaScript,
         JQuery,
         JSP,
         MongoDB,
         MVCCSharp,
         MVCCSHTML,
         MySQL,
         PHP,
         Python,
         R_Script,
         SQL
    }
    public static class ModuleName
    {
        public static readonly string BidModule = "Bid Module";
        public static readonly string DealMetadataCollection = "Deal Metadata Collection";
    }
    public static class FeatureName
    {
        public static readonly string CMI = "CMI";
    }
}
