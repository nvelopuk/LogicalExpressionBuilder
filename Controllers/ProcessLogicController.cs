
using LogicalExpressionBuilder.BlockClientClasses;
using LogicalExpressionBuilder.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace LogicBuilderTest.Controllers
{
    public class ProcessLogicController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public object FromBlockClient(string questionId, string logicType, BlockClientNode data)
        {
            //we've got to transform the json into something we can use in our data expressions. Should probably be inline if for complex

            var expression = data.GetExpression();

            System.IO.File.WriteAllText(Server.MapPath(string.Format("~/{0}_Logic.json", questionId + logicType)), JsonConvert.SerializeObject(data));

            Dictionary<string, string> expressionMap = new Dictionary<string, string>();
            expressionMap.Add(questionId + logicType, data.GetExpression());

            DataSet ds = TableBuilder.UpdateExpressions(expressionMap);

            List<string> answers = new List<string>();
            foreach (DataColumn col in ds.Tables[0].Columns)
            {
                if (!col.ColumnName.Contains("Answer"))
                    continue;

                answers.Add(col.ColumnName + "=" + ds.Tables[0].Rows[0][col.ColumnName].ToString());
            }


            return Json(new { id = 1, value = string.Format("Logic for question <strong>{0}{1}</strong> is [<strong>{2}</strong>]. <br/>Result of column <strong>{0}{1}</strong> based on current answers is <strong>{3}</strong>. <br/>Answers are <br/>{4}", questionId, logicType, expression, ds.Tables[0].Rows[0][questionId + logicType], string.Join("<br/>", answers)) }); ;
        }
    }
}