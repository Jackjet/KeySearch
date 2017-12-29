using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PZYM.Shop.BLL;

namespace Web.LuceneNet {
    public partial class BookList : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            string btnCreate = Request.QueryString["btnCreate"];
            string btnSearch = Request.QueryString["btnSearch"];
            if(!string.IsNullOrEmpty(btnCreate)) {
                //创建索引库
                CreateIndexByData();
            }
            if(!string.IsNullOrEmpty(btnSearch)) {
                //搜索
                SearchFromIndexData();
            }
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        private void CreateIndexByData() {
            string indexPath = Context.Server.MapPath("~/IndexData");//索引文档保存位置         
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
            //IndexReader:对索引库进行读取的类
            bool isExist = IndexReader.IndexExists(directory); //是否存在索引库文件夹以及索引库特征文件
            if(isExist) {
                //如果索引目录被锁定（比如索引过程中程序异常退出或另一进程在操作索引库），则解锁
                //Q:存在问题 如果一个用户正在对索引库写操作 此时是上锁的 而另一个用户过来操作时 将锁解开了 于是产生冲突 --解决方法后续
                if(IndexWriter.IsLocked(directory)) {
                    IndexWriter.Unlock(directory);
                }
            }

            //创建向索引库写操作对象  IndexWriter(索引目录,指定使用盘古分词进行切词,最大写入长度限制)
            //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);
            BooksManager bookManager = new BooksManager();
            List<PZYM.Shop.Model.Books> bookList = bookManager.GetModelList("");

            //--------------------------------遍历数据源 将数据转换成为文档对象 存入索引库
            foreach(var book in bookList) {
                Document document = new Document(); //new一篇文档对象 --一条记录对应索引库中的一个文档

                //向文档中添加字段  Add(字段,值,是否保存字段原始值,是否针对该列创建索引)
                document.Add(new Field("Id", book.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));//--所有字段的值都将以字符串类型保存 因为索引库只存储字符串类型数据

                //Field.Store:表示是否保存字段原值。指定Field.Store.YES的字段在检索时才能用document.Get取出原值  
                //Field.Index.NOT_ANALYZED:指定不按照分词后的结果保存--是否按分词后结果保存取决于是否对该列内容进行模糊查询


                document.Add(new Field("Description", book.Description, Field.Store.YES, Field.Index.ANALYZED));

                //Field.Index.ANALYZED:指定文章内容按照分词后结果保存 否则无法实现后续的模糊查询 
                //WITH_POSITIONS_OFFSETS:指示不仅保存分割后的词 还保存词之间的距离

                document.Add(new Field("MenuId", book.MenuId.ToString(), Field.Store.YES, Field.Index.ANALYZED));

                document.Add(new Field("CreativeHTML", string.IsNullOrWhiteSpace(book.CreativeHTML) ? "" : book.CreativeHTML, Field.Store.YES, Field.Index.ANALYZED));
                writer.AddDocument(document); //文档写入索引库
            }
            writer.Close();//会自动解锁
            directory.Close(); //不要忘了Close，否则索引结果搜不到
        }

        /// <summary>
        /// 从索引库中检索关键字
        /// </summary>
        private void SearchFromIndexData() {
            string indexPath = Context.Server.MapPath("~/IndexData");
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件
            //PhraseQuery query = new PhraseQuery();
            ////把用户输入的关键字进行分词
            //foreach (string word in Common.SplitContent.SplitWords(Request.QueryString["SearchKey"]))
            //{
            //    query.Add(new Term("Contents", word));
            //    query.Add(new Term("Title", word));
            //}
            BooleanQuery query = new BooleanQuery();
            foreach (string word in Common.SplitContent.SplitWords(Request.QueryString["SearchKey"]))
            {
                Term t1 = new Term("CreativeHTML", word);
                TermQuery q1 = new TermQuery(t1);
                Term t2 = new Term("Description", word);
                TermQuery q2 = new TermQuery(t2);
                //query.Add(q1, BooleanClause.Occur.SHOULD);
                query.Add(q2, BooleanClause.Occur.SHOULD);

                //BooleanClause clause1 = new BooleanClause(new TermQuery(new Term("Contents", word)), Lucene.Net.Search.BooleanClause.Occur.SHOULD);
                //BooleanClause clause2 = new BooleanClause(new TermQuery(new Term("Title", word)), Lucene.Net.Search.BooleanClause.Occur.SHOULD);
                //query.Add(clause1);
                //query.Add(clause2);
            }
            //query.Add(new Term("content", "C#"));//多个查询条件时 为且的关系
            //query.selp(100); //指定关键词相隔最大距离

            //TopScoreDocCollector盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);
            searcher.Search(query, null, collector);//根据query查询条件进行查询，查询结果放入collector容器
            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            ScoreDoc[] docs = collector.TopDocs(0, collector.GetTotalHits()).scoreDocs;

            //展示数据实体对象集合
            List<PZYM.Shop.Model.Books> bookResult = new List<PZYM.Shop.Model.Books>();
            for(int i = 0; i < docs.Length; i++) {
                int docId = docs[i].doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//根据文档id来获得文档对象Document


                PZYM.Shop.Model.Books book = new PZYM.Shop.Model.Books();
                book.Description = doc.Get("Description");
                //book.ContentDescription = doc.Get("content");//未使用高亮
                //搜索关键字高亮显示 使用盘古提供高亮插件
                book.CreativeHTML = Common.SplitContent.HightLight(Request.QueryString["SearchKey"], doc.Get("CreativeHTML"));
                book.MenuId = Convert.ToInt32(doc.Get("MenuId"));
                book.Id = Convert.ToInt32(doc.Get("Id"));
                bookResult.Add(book);
            }
            Repeater1.DataSource = bookResult;
            Repeater1.DataBind();
        }
    }
}