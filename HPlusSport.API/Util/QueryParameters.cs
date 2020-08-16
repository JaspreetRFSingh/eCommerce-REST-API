using System;

namespace HPlusSport.API.Util
{
    public class QueryParameters
    {
        const int maxSize = 100;
        private int size = 50;
        public int Page { get; set; }
        public int Size { get { return size; }
            set 
            {
                size = Math.Min(maxSize, value);
            }
        }
        public string SortBy { get; set; } = "Id";
        private string sortOrder = "asc";
        public string SortOrder
        {
            get
            {
                return sortOrder;
            }
            set
            {
                if(value == "asc" || value == "desc")
                {
                    sortOrder = value;
                }
            }
        }
    }
}
