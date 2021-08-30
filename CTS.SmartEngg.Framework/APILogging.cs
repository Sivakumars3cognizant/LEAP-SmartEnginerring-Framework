// Copyright (c) Cognizant. All Rights Reserved.

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CTS.SmartEngg.Framework
{
    public static class ApiLogging
    {
        public static string LogApi { get; set; }
        public static string ProjectId { get; set; }
        public static string AssociateId { get; set; }
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="info"></param>
        public static async Task Log(string apiURL, ErrorLogDetails info)
        {
            ErrorLogDetails infoDetail = new ErrorLogDetails();
            if (info != null)
            {
                infoDetail.LogSeverity = info.LogSeverity;
                infoDetail.LogLevel = info.LogLevel;
                infoDetail.HostName = info.HostName;
                infoDetail.AssociateId = info.AssociateId;
                infoDetail.CreatedDate = info.CreatedDate;
                infoDetail.ProjectId = info.ProjectId;
                infoDetail.Technology = info.Technology;
                infoDetail.ModuleName = info.ModuleName;
                infoDetail.FeatureName = info.FeatureName;
                infoDetail.ClassName = info.ClassName;
                infoDetail.MethodName = info.MethodName;
                infoDetail.ProcessId = info.ProcessId;
                infoDetail.ErrorCode = info.ErrorCode;
                infoDetail.ErrorMessage = info.ErrorMessage;
                infoDetail.StackTrace = info.StackTrace;
                infoDetail.AdditionalField_1 = info.AdditionalField_1;
                infoDetail.AdditionalField_2 = info.AdditionalField_2;
            }

            var url = apiURL;
            if (!String.IsNullOrEmpty(url))
            {
                string strPayload = JsonConvert.SerializeObject(infoDetail);                
                HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
                using (var client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }))
                {
                    client.BaseAddress = new Uri(apiURL);
                    HttpResponseMessage result = await client.PostAsync("Logger", c).ConfigureAwait(false);
                    await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

            }
        }
        /// <summary>
        /// UpdateErrorLogModelValue
        /// </summary>
        /// <param name="logSeverity"></param>
        /// <param name="logLevel"></param>
        /// <param name="featureName"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="processId"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="stackTrace"></param>
        /// <param name="additionalField_1"></param>
        /// <param name="additionalField_2"></param>
        /// <returns></returns>
        public static void InsertLog(string apiURL,ErrorLogDetails info)
        {
            //string logSeverity, string logLevel, string featureName,
            //string className, string methodName, int processId, string errorCode, string errorMessage,
            //string stackTrace, string additionalField_1, string additionalField_2

            ErrorLogDetails infoDetail = new ErrorLogDetails();
            infoDetail.LogSeverity = info.LogSeverity;
            infoDetail.LogLevel = info.LogLevel;
            infoDetail.HostName = Environment.MachineName;
            infoDetail.AssociateId = AssociateId;
            infoDetail.CreatedDate = Convert.ToString(DateTime.Now.ToString());
            infoDetail.ProjectId = ProjectId;
            infoDetail.Technology = Convert.ToString(Technology.DotNetCore);
            infoDetail.ModuleName = Convert.ToString(ModuleName.BidModule);
            infoDetail.FeatureName = info.FeatureName;
            infoDetail.ClassName = info.ClassName;
            infoDetail.MethodName = info.MethodName;
            infoDetail.ProcessId = info.ProcessId;
            infoDetail.ErrorCode = info.ErrorCode;
            infoDetail.ErrorMessage = info.ErrorMessage;
            infoDetail.StackTrace = info.StackTrace;
            infoDetail.AdditionalField_1 = info.AdditionalField_1;
            infoDetail.AdditionalField_2 = info.AdditionalField_2;
            Log(apiURL,infoDetail);
        }
    }
}
