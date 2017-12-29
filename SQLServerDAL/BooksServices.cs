using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Maticsoft.DBUtility;//Please add references
using PZYM.Shop.IDAL;
namespace PZYM.Shop.SQLServerDAL {
    /// <summary>
    /// 数据访问类:BooksServices
    /// </summary>
    public partial class BooksServices : IBooksServices {
        public BooksServices() {
        }
        #region  Method

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId() {
            return DbHelperSQL.GetMaxID("Id", "Advertising");
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int Id) {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Advertising");
            strSql.Append(" where Id=@Id ");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)};
            parameters[0].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(PZYM.Shop.Model.Books model) {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Advertising(");
            strSql.Append(@"[Description]
           ,[CreativeHTML]
           ,[ImageUrl]
           ,[FileName]
           ,[FilePath]
           ,[MenuId]
            )");
            strSql.Append(" values (");
            strSql.Append(@"@Description
           ,@CreativeHTML
           ,@ImageUrl
           ,@FileName
           ,@FilePath
           ,@MenuId)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@Description", SqlDbType.NVarChar,200),
					new SqlParameter("@CreativeHTML", SqlDbType.NText,99999),
					new SqlParameter("@ImageUrl", SqlDbType.NVarChar,500),
					new SqlParameter("@FilePath", SqlDbType.NVarChar,500),
					new SqlParameter("@FileName", SqlDbType.NVarChar,500),
                    new SqlParameter("@MenuId", SqlDbType.Int,4)};
            parameters[0].Value = model.Description;
            parameters[1].Value = model.CreativeHTML;
            parameters[2].Value = model.ImageUrl;
            parameters[3].Value = model.FilePath;
            parameters[4].Value = model.FileName;
            parameters[5].Value = model.MenuId;
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if(obj == null) {
                return 0;
            } else {
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(PZYM.Shop.Model.Books model) {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Advertising set ");
            strSql.Append("Description=@Description,");
            strSql.Append("CreativeHTML=@CreativeHTML,");
            strSql.Append("ImageUrl=@ImageUrl,");
            strSql.Append("FileName=@FileName,");
            strSql.Append("FilePath=@FilePath,");
            strSql.Append("MenuId=@MenuId");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Description", SqlDbType.NVarChar,200),
					new SqlParameter("@CreativeHTML", SqlDbType.NText,99999),
					new SqlParameter("@ImageUrl", SqlDbType.NVarChar,500),
					new SqlParameter("@FileName", SqlDbType.NVarChar,500),
					new SqlParameter("@FilePath", SqlDbType.NVarChar,500),
                    new SqlParameter("@MenuId",SqlDbType.Int,4),
                    new SqlParameter("@Id",SqlDbType.Int,4) };
            parameters[0].Value = model.Description;
            parameters[1].Value = model.CreativeHTML;
            parameters[2].Value = model.ImageUrl;
            parameters[3].Value = model.FileName;
            parameters[4].Value = model.FilePath;
            parameters[5].Value = model.MenuId;
            parameters[6].Value = model.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if(rows > 0) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int Id) {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Advertising ");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)
};
            parameters[0].Value = Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if(rows > 0) {
                return true;
            } else {
                return false;
            }
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string Idlist) {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Advertising ");
            strSql.Append(" where Id in (" + Idlist + ")  ");
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString());
            if(rows > 0) {
                return true;
            } else {
                return false;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public PZYM.Shop.Model.Books GetModel(int Id) {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select  top 1 [Id]
      ,[Description]
      ,[CreativeHTML]
      ,[ImageUrl]
      ,[FileName]
      ,[FilePath]
      ,[MenuId] from Advertising ");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)
};
            parameters[0].Value = Id;

            PZYM.Shop.Model.Books model = new PZYM.Shop.Model.Books();
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if(ds.Tables[0].Rows.Count > 0) {
                if(ds.Tables[0].Rows[0]["Id"].ToString() != "") {
                    model.Id = int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
                }
                model.Description = ds.Tables[0].Rows[0]["Description"].ToString();
                if (ds.Tables[0].Rows[0]["CreativeHTML"].ToString() != "")
                {
                    model.CreativeHTML = ds.Tables[0].Rows[0]["CreativeHTML"].ToString();
                }
                if (ds.Tables[0].Rows[0]["MenuId"].ToString() != "")
                {
                    model.MenuId = Convert.ToInt32(ds.Tables[0].Rows[0]["MenuId"]);
                }
                if (ds.Tables[0].Rows[0]["ImageUrl"].ToString() != "")
                {
                    model.ImageUrl = ds.Tables[0].Rows[0]["ImageUrl"].ToString();
                }
                if (ds.Tables[0].Rows[0]["FileName"].ToString() != "")
                {
                    model.FileName = ds.Tables[0].Rows[0]["FileName"].ToString();
                }
                if (ds.Tables[0].Rows[0]["FilePath"].ToString() != "")
                {
                    model.FilePath = ds.Tables[0].Rows[0]["FilePath"].ToString();
                }
                
                return model;
            } else {
                return null;
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT [Id]
            ,[Description]
            ,[CreativeHTML]
           ,[ImageUrl]
           ,[FileName]
           ,[FilePath]
           ,[MenuId]");
            strSql.Append(" FROM [dbo].[Advertising] ");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder) {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if(Top > 0) {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(@" [Id]
      ,[Description]
      ,[CreativeHTML]
      ,[ImageUrl]
      ,[FileName]
      ,[FilePath]
      ,[MenuId] ");
            strSql.Append(" FROM [dbo].[Advertising] ");
            if(!string.IsNullOrEmpty(strWhere)) {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(strSql.ToString());
        }

        /*
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        {
            SqlParameter[] parameters = {
                    new SqlParameter("@tblName", SqlDbType.VarChar, 255),
                    new SqlParameter("@fldName", SqlDbType.VarChar, 255),
                    new SqlParameter("@PageSize", SqlDbType.Int),
                    new SqlParameter("@PageIndex", SqlDbType.Int),
                    new SqlParameter("@IsReCount", SqlDbType.Bit),
                    new SqlParameter("@OrderType", SqlDbType.Bit),
                    new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
                    };
            parameters[0].Value = "Books";
            parameters[1].Value = "Id";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  Method
    }
}

