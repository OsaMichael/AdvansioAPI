using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Utilities
{
    public static class GeneralStatusCodes
    {
        public static readonly (string code, string message) Status_Success = ("00", "Success");

        public static readonly (string code, string message) Status_Exception = ("02", "an error occured");
        public static readonly (string code, string message) Status_ShitHappens_General = ("SHIT_HAPPENS", "An error has occured");
        public static readonly (string code, string message) Status_OperationFailed = ("202", "Operation failed");
    }
}
