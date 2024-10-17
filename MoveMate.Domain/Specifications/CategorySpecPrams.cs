using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Domain.Specifications
{
    public class CategorySpecPrams
    {
        private const int MaxPageSize = 50;
        public int pageIndex { get; set; } = 1;
        private int _pageSize = 5;

        public int pageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? sort { get; set; }
        public string? _search;

        public string? Search
        {
            get => _search;
            set => _search = value.ToLower();
        }
    }
}