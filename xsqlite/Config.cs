using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xsqlite {
    public class Config {
        public static string TableFile{
            get;
            set;
        }
        public static string Namespace {
            get;
            set;
        }
        public static string ShortNamespace {
            get{
                return Namespace.ToUpper();
            }
        }
    }
}
