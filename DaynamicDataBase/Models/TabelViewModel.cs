namespace DaynamicDataBase.Models
{
    public class TabelViewModel
    {
        public List<string> Columns { get; set; }
        public List<int> Rows { get; set; }

        public List<RowItems> RowItems { get; set; }


    }

    public class RowItems
    {
        public int RowId { get; set; }

        public string Value { get; set; }
    }
}
