using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace LogicalExpressionBuilder.Models
{
    public class QueryBuilderRequest
    {
        public string LogicType { get; set; }

        public string QuestionId { get; set; }


    }
}