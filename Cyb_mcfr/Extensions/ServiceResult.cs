﻿using System.ComponentModel;

namespace Cyb_mcfr.Extensions
{
    public enum ServiceResultStatus
    {
        [Description("Error")]
        Error = 0,
        [Description("Success")]
        Success = 1,
        //[Description("Warning")] // these are not used atm
        //Warning,
        //[Description("Info")]
        //Info,
    }
    public class ServiceResult
    {
        public ServiceResultStatus Result { get; set; }
        public ICollection<string> Messages { get; set; }

        public ServiceResult()
        {
            Result = ServiceResultStatus.Success;
            Messages = new List<string>();
        }

        public static Dictionary<string, ServiceResult> CommonResults { get; set; } = new Dictionary<string, ServiceResult>()
        {
              {"NotFound" , new ServiceResult() {
                  Result =ServiceResultStatus.Error,
                  Messages = new List<string>( new string[] { "Object not found" })  } },
              {"OK" , new ServiceResult() {
                  Result =ServiceResultStatus.Success,
                  Messages = new List<string>()  } }
        };
    }
}
