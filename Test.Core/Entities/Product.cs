using System.Collections.Generic;

namespace Test.Core.Entities
{
    public class Product
    {
        private static readonly Dictionary<int, string> StatusNameCache = new Dictionary<int, string>();
        private static readonly object StatusNameCacheLock = new object();

        private int _status;
        private string _statusName;

        public int ProductId { get; set; }
        public string Name { get; set; }

        public int Status
        {
            get => _status;
            set
            {
                _status = value;
                lock (StatusNameCacheLock)
                {
                    StatusNameCache.TryGetValue(value, out var cachedStatusName);
                    if (string.IsNullOrEmpty(cachedStatusName))
                    {
                        cachedStatusName = GetStatusName(value);
                        StatusNameCache[value] = cachedStatusName;
                    }

                    _statusName = cachedStatusName;
                }
            }
        }

        public string StatusName
        {
            get
            {
                lock (StatusNameCacheLock)
                {
                    return _statusName;
                }
            }
        }

        public int Stock { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice { get; set; }
        private static string GetStatusName(int status)
        {
            return status == 1 ? "Active" : "Inactive";
        }
        public void SetStatusName(string statusName)
        {
            _statusName = statusName;
        }
    }

    public class ProductUpdateModel
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

}
