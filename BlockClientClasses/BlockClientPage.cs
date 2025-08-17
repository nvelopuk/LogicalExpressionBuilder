using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LogicalExpressionBuilder.BlockClientClasses
{
    public class BlockClientPage
    {
        public BlockClientPage()
        {
            
        }

        public List<BlockClientOption> Options { get; set; }
        public BlockClientNode Expression { get; set; }
    }
}