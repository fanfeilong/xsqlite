using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xsqlite {
    public class DataBase {
        public string Name{
            get;
            set;
        }
        public List<Table> Tables{
            get;
            set;
        }

        public List<Query> FreeQuerys{
            get;
            set;
        } 
        public DataBase(){
            Tables = new List<Table>();
            FreeQuerys=new List<Query>();
        }
    }
}
