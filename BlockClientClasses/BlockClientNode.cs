using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LogicalExpressionBuilder.BlockClientClasses
{
    public class BlockClientNode
    {
        public BlockClientNode()
        {
            Nodes = new List<BlockClientNode>();
        }

        public string Type { get; set; }

        public string Name { get; set; }

        public string InputType { get; set; }

        public string Value { get; set; }

        public List<BlockClientNode> Nodes { get; set; }

        public string BuildNode()
        {
            StringBuilder sb = new StringBuilder();

            switch (Type)
            {
                case "if":
                    return BuildIfNode();
                case "group":
                    return BuildGroupNode();
                case "then":
                    return BuildThenNode();
                case "else":
                    return BuildElseNode();
                case "expression":
                    return BuildExpressionNode();
                case "mathsoperation":
                    return BuildMathsOperationNode();
                case "criteria":
                    return BuildCriteriaNode();
                case "option":
                    return BuildOptionNode();
                case "column":
                    return BuildColumnNode();
                case "groupstatement":
                    return BuildGroupStatementNode();
                case "input":
                    return BuildInputNode();
                default:
                    return "";
            }
        }

        public string GetExpression()
        {
            if (Nodes == null || !Nodes.Any())
                return string.Empty;

            List<string> expressions = new List<string>();


            return Nodes.Any() ? Nodes.First().BuildNode() : "";
            

            //return string.Join(" " + Condition + " ", expressions);

            //return "";
        }

        //public string BuildGroups()
        //{
        //    List<string> expressions = new List<string>();

        //    foreach (var rule in Rules)
        //    {
        //        if (rule.Operator == null)
        //        {
        //            expressions.Add(rule.BuildGroups());
        //        }
        //        else
        //        {
        //            expressions.Add(rule.BuildLogicalExpression());
        //        }
        //    }

        //    return "(" + string.Join(" " + Condition + " ", expressions) + ")";
        //}

        //public string BuildLogicalExpression()
        //{
        //    string val = "";

        //    if (Operator == null)
        //        return val;

        //    if (Operator.Equals("equal"))
        //        val = " = " + BuildValueString();
        //    else if (Operator.Equals("not_equal"))
        //        val = " <> " + BuildValueString();
        //    else if (Operator.Equals("in"))
        //        val = " in(" + BuildValueString() + ")";
        //    else if (Operator.Equals("not_in"))
        //        val = " not in(" + BuildValueString() + ")";
        //    else if (Operator.Equals("greater_than"))
        //        val = " > " + BuildValueString();
        //    else if (Operator.Equals("less_than"))
        //        val = " < " + BuildValueString();


        //    val = Field + val;

        //    return val;
        //}

        //public string BuildValueString()
        //{
        //    if (Type == "string")
        //        return "\"" + Value + "\"";
        //    else if (Type == "date")
        //        return "#" + Value + "#";

        //    return Value.ToString();
        //}

        public string BuildGroupNode()
        {
            StringBuilder sb = new StringBuilder();

            var condition = Nodes.First(x => x.Type == "condition").Nodes.First().Value;

            sb.Append("(");

            foreach(var node in Nodes.Single(x => x.Type == "groupstatement").Nodes)
            {
                sb.Append(node.BuildNode());

                if (Nodes.Single(x => x.Type == "groupstatement").Nodes.Last() != node)
                    sb.Append(" " + condition + " ");
            }


            sb.Append(")");

            return sb.ToString();
        }

        public string BuildThenNode()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var node in Nodes)
            {
                sb.Append(node.BuildNode());
            }

            return sb.ToString();
        }

        public string BuildElseNode()
        {
            StringBuilder sb = new StringBuilder();

            foreach(var node in Nodes)
            {
                sb.Append(node.BuildNode());
            }

            return sb.ToString();
        }

        public string BuildIfNode()
        {
            StringBuilder sb = new StringBuilder();

            if (Nodes.Count != 3)
                throw new Exception("If node should have three child nodes");

            sb.Append("IIF(");

            foreach(var node in Nodes)
            {
                sb.Append(node.BuildNode());

                if (node.Type != "else")
                    sb.Append(",");
            }

            sb.Append(")");


            return sb.ToString();
        }

        public string BuildExpressionNode()
        {
            StringBuilder sb = new StringBuilder();

            if (Nodes.Count != 3)
                throw new Exception("Expression node should have three child nodes");

            sb.Append(string.Format(GetOperator(Nodes.First(x => x.Type == "operator").Nodes.First().Value), 
                Nodes.First(x => x.Type == "left").Nodes.First().BuildNode(),
                Nodes.First(x => x.Type == "right").Nodes.First().BuildNode()
                ));

            //sb.Append(" " +  Nodes.First(x => x.Type == "left").Nodes.First().BuildNode() + " ");
            //sb.Append(GetOperator(Nodes.First(x => x.Type == "operator").Nodes.First().Value));
            //sb.Append(" " + Nodes.First(x => x.Type == "right").Nodes.First().BuildNode() + " ");

            return sb.ToString();
        }

        public string BuildMathsOperationNode()
        {
            StringBuilder sb = new StringBuilder();

            if (Nodes.Count != 3)
                throw new Exception("Expression node should have three child nodes");

            sb.Append(string.Format(GetOperator(Nodes.First(x => x.Type == "operator").Nodes.First().Value),
                Nodes.First(x => x.Type == "left").Nodes.First().BuildNode(),
                Nodes.First(x => x.Type == "right").Nodes.First().BuildNode()
                ));

            //sb.Append(" " + Nodes.First(x => x.Type == "left").Nodes.First().BuildNode() + " ");
            //sb.Append(GetOperator(Nodes.First(x => x.Type == "operator").Nodes.First().Value));
            //sb.Append(" " + Nodes.First(x => x.Type == "right").Nodes.First().BuildNode() + " ");

            return sb.ToString();
        }

        public string BuildGroupStatementNode()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var node in Nodes)
            {
                sb.Append(node.BuildNode());
            }

            return sb.ToString();
        }

    public string BuildCriteriaNode()
        {
            if (Nodes.Count != 1)
                throw new Exception("Criteria node should have one child nodes");

            return Nodes.First().BuildNode();
        }

        public string BuildOptionNode()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Value);

            var val = sb.ToString();

            int numVal;
            bool boolVal;

            if (!int.TryParse(val, out numVal) && !bool.TryParse(val, out boolVal))

                val = "'" + val +"'";


            return val;
        }

        public string BuildInputNode()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Value);

            var val = sb.ToString();

            int numVal;
            bool boolVal;

            if (!int.TryParse(val, out numVal) && !bool.TryParse(val, out boolVal))

                val = "'" + val + "'";


            return val;
        }

        public string BuildColumnNode()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Value);

            return sb.ToString();
        }

    public string GetOperator(string wordOperator)
        {
            switch (wordOperator)
            {
                case "EQUALS":
                    return "{0} = {1}";
                case "NOTEQUALS":
                    return "{0} <> {1}";
                case "GREATERTHAN":
                    return "{0} > {1}";
                case "LESSTHAN":
                    return "{0} < {1}";
                case "CONTAINS":
                    return "{0} like *{1}*";
                case "NOTCONTAINS":
                    return "{0} not like *{1}*";
                case "PLUS":
                    return "{0} + {1}";
                case "MINUS":
                    return "{0} - {1}";
                case "MULTIPLY":
                    return "{0} * {1}";
                case "DIVIDE":
                    return "{0} % {1}";
                default:
                    return "";

            }
        }
    }
}