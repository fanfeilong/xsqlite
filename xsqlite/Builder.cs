using System;
using System.Collections.Generic;
using System.Text;

namespace xsqlite{
    public class EachBuilder<T> {
        public IEnumerable<T> List;
        public StringBuilder Builder;
        public List<Action<T>> Actions;
        public EachBuilder() {
            Actions=new List<Action<T>>();
        }
    }
}