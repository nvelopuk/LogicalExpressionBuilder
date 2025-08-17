using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LogicalExpressionBuilder.BlockClientClasses
{
    public class BlockClientOption
    {
        public BlockClientOption()
        {
            Options = new List<BlockClientOption>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public List<BlockClientOption> Options { get; set; }
    }
}