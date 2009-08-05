using System;
using System.Collections.Generic;
using System.Text;

namespace dbfit{

    public class MySqlTest : DatabaseTest {
        public MySqlTest() : base(new MySqlEnvironment())
        {
        }

    }
}
