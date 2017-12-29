using System;
namespace PZYM.Shop.Model
{
	/// <summary>
	/// Books:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Books
	{
		public Books()
		{}
		#region Model
        public int Id { get; set; }
        //public string Title { get; set; }
        //public string Contents { get; set; }
        public int Hot { get; set; }
        public DateTime CreateTime { get; set; }
        public int SortId { get; set; }
        public int ClickNum { get; set; }
        public int Type { get; set; }
        public int IsDelete { get; set; }
        public string Creator { get; set; }
        public string ShowImgUrl { get; set; }

        public string Description { get; set; }
        public string CreativeHTML { get; set; }
        public string ImageUrl { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int MenuId { get; set; }
		#endregion Model

	}
}

