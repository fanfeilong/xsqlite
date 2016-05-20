using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace xsqlite {
    public enum QueryType{
        SingleTable,
        MultiTable
    }

    public class FieldInfo{
        public string TableName{
            get;
            set;
        }

        public string FieldName{
            get;
            set;
        }
    }

    public class Query {
        public void Update(){
            if(Ins.Union(Outs).Select(f=>f.TableName).Distinct().Count()>1){
                QueryType = QueryType.MultiTable;
            } else{
                QueryType = QueryType.SingleTable;
            }
        }

        public QueryType QueryType{
            get;
            private set;
        }
        public string EnumName{
            get{
                return string.Format("SQL_{0}", Name);
            }
        }
        public string Name{
            get;
            set;
        }

        public List<FieldInfo> Ins{
            get;
            set;
        }

        public List<FieldInfo> Outs{
            get;
            set;
        } 

        public string Statement{
            get;
            set;
        }

        public bool IsSelect{
            get{
                var k = Statement.Substring(1, Statement.IndexOf(' ')-1).ToLower();
                var s= k == "select";
                return s;
            }
        }

        public Query(){
            Ins = new List<FieldInfo>();
            Outs = new List<FieldInfo>();
            QueryType = QueryType.SingleTable;
            ;
        }
    }
}
