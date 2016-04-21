using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xsqlite {
    public class Table {
        public string Name{
            get;
            set;
        }
        public List<Field> Fields{
            get;
            set;
        }
        public Table(){
            Fields = new List<Field>();
        }
    }
}
