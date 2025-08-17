using LogicalExpressionBuilder.BlockClientClasses;
using LogicalExpressionBuilder.Helpers;
using LogicalExpressionBuilder.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicalExpressionBuilder.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(QueryBuilderRequest request)
        {
            BlockClientPage model = new BlockClientPage();

            model.Options = BuildBlockOptions();

            model.Expression = BuildBlockRules(request.QuestionId, request.LogicType);

            return View(model);


        }

        public ActionResult Exp()
        {
            return View();
        }

        public ActionResult Blocks(QueryBuilderRequest request)
        {
            BlockClientPage model = new BlockClientPage();

            model.Options = BuildBlockOptions();

            model.Expression = BuildBlockRules(request.QuestionId, request.LogicType);

            return View(model);
        }

        private List<BlockClientOption> BuildBlockOptions()
        {
            List<BlockClientOption> options = new List<BlockClientOption>();

            //cols, conditions, operators, statements, inputs, options

            var ds = TableBuilder.BuildTable();

            BlockClientOption colsOpt = new BlockClientOption() { Name = "Columns", Type = "column" };
            BlockClientOption conditionsOpt = new BlockClientOption() { Name = "Conditions", Type = "condition" };
            BlockClientOption operatorsOpt = new BlockClientOption() { Name = "Operators", Type = "operator" };
            BlockClientOption statementsOpt = new BlockClientOption() { Name = "Statements", Type = "statement" };
            BlockClientOption inputsOpt = new BlockClientOption() { Name = "Inputs", Type = "input" };
            BlockClientOption optionsOpt = new BlockClientOption() { Name = "Options", Type = "option" };

            options.Add(colsOpt);
            options.Add(conditionsOpt);
            options.Add(operatorsOpt);
            options.Add(statementsOpt);
            options.Add(inputsOpt);
            options.Add(optionsOpt);


            foreach (DataColumn col in ds.Tables[0].Columns)
            {
                if (col.ColumnName.Contains("Answer"))
                    colsOpt.Options.Add(new BlockClientOption() { Id = col.ColumnName, Name = col.ColumnName, DataType = ds.Tables[0].Columns[col.ColumnName].DataType.Name });
                else if (col.ColumnName.Contains("Score"))
                    colsOpt.Options.Add(new BlockClientOption() { Id = col.ColumnName, Name = col.ColumnName, DataType = ds.Tables[0].Columns[col.ColumnName].DataType.Name });
                else if (col.ColumnName.Contains("Enabled"))
                    colsOpt.Options.Add(new BlockClientOption() { Id = col.ColumnName, Name = col.ColumnName, DataType = ds.Tables[0].Columns[col.ColumnName].DataType.Name });

            }

            //conditions
            conditionsOpt.Options.Add(new BlockClientOption() { Name = "AND" });
            conditionsOpt.Options.Add(new BlockClientOption() { Name = "OR" });

            //operators
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "EQUALS", Value = "EQUALS" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "NOT EQUALS", Value = "NOTEQUALS" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "GREATER THAN", Value = "GREATERTHAN" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "LESS THAN", Value = "LESSTHAN" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "CONTAINS ANY", Value = "CONTAINSANY" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "CONTAINS ALL", Value = "CONTAINSALL" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "NOT CONTAINS ANY", Value = "NOTCONTAINSANY" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "NOT CONTAINS ALL", Value = "NOTCONTAINSALL" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "PLUS", Value = "PLUS" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "MINUS", Value = "MINUS" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "MULTIPLY", Value = "MULTIPLY" });
            operatorsOpt.Options.Add(new BlockClientOption() { Name = "DIVIDE", Value = "DIVIDE" });

            //statements
            statementsOpt.Options.Add(new BlockClientOption() { Name = "IF", Type = "if" });
            statementsOpt.Options.Add(new BlockClientOption() { Name = "GROUP", Type = "group" });
            statementsOpt.Options.Add(new BlockClientOption() { Name = "EXPRESSION", Type = "expression" });
            statementsOpt.Options.Add(new BlockClientOption() { Name = "MATHS OPERATION", Type = "mathsOperation" });

            //inputs
            inputsOpt.Options.Add(new BlockClientOption() { Name = "NUMBER" });
            inputsOpt.Options.Add(new BlockClientOption() { Name = "TEXT" });

            //options
            optionsOpt.Options.Add(new BlockClientOption() { Value = "1", Name = "YES" });
            optionsOpt.Options.Add(new BlockClientOption() { Value = "0", Name = "NO" });
            optionsOpt.Options.Add(new BlockClientOption() { Value = "OPTION 1", Name = "OPTION 1" });
            optionsOpt.Options.Add(new BlockClientOption() { Value = "OPTION 2", Name = "OPTION 2" });
            optionsOpt.Options.Add(new BlockClientOption() { Value = "OPTION 3", Name = "OPTION 3" });
            optionsOpt.Options.Add(new BlockClientOption() { Value = "OPTION 4", Name = "OPTION 4" });

            return options;
        }


        private BlockClientNode BuildBlockRules(string questionId, string logicType)
        {
            BlockClientNode rules = null;

            if (string.IsNullOrEmpty(questionId) || string.IsNullOrEmpty(logicType))
                return rules;

            string filePath = Server.MapPath(string.Format("~/{0}_Logic.json", questionId + logicType));


            if (System.IO.File.Exists(filePath))
            {
                rules = JsonConvert.DeserializeObject<BlockClientNode>(System.IO.File.ReadAllText(filePath), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }

            return rules;
        }


        
        private List<BlockClientOption> BuildOptions()
        {
            var options = new List<BlockClientOption>();

            return options;
        }

        public ActionResult Handsontable()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}