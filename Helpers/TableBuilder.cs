using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace LogicalExpressionBuilder.Helpers
{
    public static class TableBuilder
    {
        private static string filePathSchema = HttpContext.Current.Server.MapPath("~/QuestionsSchema.xml");
        private static string filePathData = HttpContext.Current.Server.MapPath("~/QuestionsData.xml");
        public static DataSet BuildTable()
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            if (File.Exists(filePathSchema))
            {
                dt.ReadXmlSchema(filePathSchema);
                ds.Tables.Add(dt);

                if(File.Exists(filePathData))
                {
                    dt.ReadXml(filePathData);
                }

                return ds;
            }

            Dictionary<string, string> questions = new Dictionary<string, string>();
            Dictionary<string, string> answers = new Dictionary<string, string>();
            Dictionary<string, string> enabled = new Dictionary<string, string>();

            for (int i = 1; i < 5; i++)
            {
                DataColumn dcAnswer = new DataColumn(string.Format("F{0:D2}Answer", i), typeof(int));
                DataColumn dcScore = new DataColumn(string.Format("F{0:D2}Score", i), typeof(double));
                DataColumn dcEnabled = new DataColumn(string.Format("F{0:D2}Enabled", i), typeof(int));

                dt.Columns.Add(dcAnswer);
                dt.Columns.Add(dcScore);
                dt.Columns.Add(dcEnabled);
            }

            ds.Tables.Add(dt);

            DataRow dr = dt.NewRow();

            var rnd = new Random(0);

            for (int i = 1; i < 5; i++)
            {
                dr[string.Format("F{0:D2}Answer", i)] = rnd.Next(3);
            }

            dt.Rows.Add(dr);

            dt.WriteXmlSchema(filePathSchema);
            dt.WriteXml(filePathData);

            return ds;
        }

        public static DataSet UpdateExpressions(Dictionary<string, string> expressionMap)
        {
            DataSet ds = BuildTable();

            foreach(var item in expressionMap)
            {
                var matchingCol = ds.Tables[0].Columns[item.Key];

                if (matchingCol == null)
                    continue;

                matchingCol.Expression = item.Value;
            }

            ds.Tables[0].WriteXmlSchema(filePathSchema);
            ds.Tables[0].WriteXml(filePathData);

            return ds;
        }
    }
}