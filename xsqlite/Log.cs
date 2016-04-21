using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xprotocol {
    public static class Log {
        public static string VerifyRet{
            get{
                return string.Format("{0}_PACKAGE_iRETURN_IFFAILED", Config.Namespace.ToUpper());
            }
        }
    }
}
