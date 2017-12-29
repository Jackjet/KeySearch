using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PZYM.Shop.BLL;
using PZYM.Shop.Model;
using System.Data;
using System.Text.RegularExpressions;

namespace Web.Handler
{
    /// <summary>
    /// QueryKeyWord 的摘要说明
    /// </summary>
    public class QueryKeyWord : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            SearchFromIndexData(context);
        }

        private void SearchFromIndexData(HttpContext context)
        {
            string indexPath = context.Server.MapPath("~/IndexData");
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);

            //--------------------------------------这里配置搜索条件
            //PhraseQuery query = new PhraseQuery();
            //foreach(string word in Common.SplitContent.SplitWords(Request.QueryString["SearchKey"])) {
            //    query.Add(new Term("content", word));//这里是 and关系
            //}
            //query.SetSlop(100);

            //关键词Or关系设置
            BooleanQuery queryOr = new BooleanQuery();
            TermQuery query = null;
            foreach (string word in Common.SplitContent.SplitWords(context.Request["SearchKey"]))
            {
                query = new TermQuery(new Term("CreativeHTML", word));
                queryOr.Add(query, BooleanClause.Occur.SHOULD);//这里设置 条件为Or关系
                query = new TermQuery(new Term("Description", word));
                queryOr.Add(query, BooleanClause.Occur.SHOULD);//这里设置 条件为Or关系
            }
            //--------------------------------------
            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);
            //searcher.Search(query, null, collector);
            searcher.Search(queryOr, null, collector);

            int PageIndex = int.Parse(Convert.ToString(context.Request["PageIndex"]));
            int PageSize = int.Parse(Convert.ToString(context.Request["PageSize"]));
            ScoreDoc[] docs = collector.TopDocs((PageIndex - 1) * PageSize, PageIndex * PageSize).scoreDocs;//取前十条数据  可以通过它实现LuceneNet搜索结果分页
            List<PZYM.Shop.Model.Books> bookResult = new List<PZYM.Shop.Model.Books>();
            DataTable dt = new DataTable();
            dt.TableName = "ds";
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("CreativeHTML", typeof(string));
            dt.Columns.Add("MenuId", typeof(int));
            for (int i = 0; i < docs.Length; i++)
            {
                int docId = docs[i].doc;
                Document doc = searcher.Doc(docId);
                PZYM.Shop.Model.Books book = new PZYM.Shop.Model.Books();
                book.Description = Common.SplitContent.HightLight(context.Request["SearchKey"], doc.Get("Description"));
                book.CreativeHTML = Common.SplitContent.HightLight(context.Request["SearchKey"], doc.Get("CreativeHTML"));
                book.Id = Convert.ToInt32(doc.Get("Id"));
                //bookResult.Add(book);
                DataRow dr = dt.NewRow();
                dr["Description"] = book.Description;
                dr["CreativeHTML"] = book.CreativeHTML;
                //string CreativeHTML = book.CreativeHTML;
                //Regex objRegex = new Regex("/<[^>]*>/g", RegexOptions.IgnoreCase);
                //dr["CreativeHTML"]  = objRegex.Replace(book.CreativeHTML, "");
                dr["Id"] = book.Id;
                dr["MenuId"] = Convert.ToInt32(doc.Get("MenuId"));
                dt.Rows.Add(dr);
            }
            JsonModel jsonModel = null;
            int RowCount = 0;
            PagedDataModel<Dictionary<string, object>> pagedDataModel = null;
            
            if (dt == null)
            {
                jsonModel = new JsonModel()
                {
                    status = "no",
                    errMsg = "无数据"
                };
            }
            RowCount = dt.Rows.Count;
            if (RowCount <= 0)
            {
                jsonModel = new JsonModel()
                {
                    status = "no",
                    errMsg = "无数据"
                };
            }
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            list = DataTableToList(dt);
            int PageCount = (int)Math.Ceiling(RowCount * 1.0 / PageSize);
            pagedDataModel = new PagedDataModel<Dictionary<string, object>>()
            {
                PageCount = PageCount,
                PagedData = list,
                PageIndex = PageIndex,
                PageSize = PageSize,
                RowCount = RowCount
            };
            //将分页数据实体封装到JSON标准实体中
            jsonModel = new JsonModel()
            {
                errNum = 0,
                errMsg = "success",
                retData = pagedDataModel
            };
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            context.Response.Write("{\"result\":" + jss.Serialize(jsonModel) + "}");

        }

        public List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(result);
            }
            return list;
        }

        public class PagedDataModel<T>
        {
            //分页数据
            public List<T> PagedData { get; set; }
            //当前页
            public int PageIndex { get; set; }
            //每页多少条
            public int PageSize { get; set; }
            //总页数
            public int PageCount { get; set; }
            //总条数
            public int RowCount { get; set; }
        }
        public class JsonModel
        {
            public object retData { get; set; }
            public string errMsg { get; set; }
            public int errNum { get; set; }
            public string status { get; set; }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}