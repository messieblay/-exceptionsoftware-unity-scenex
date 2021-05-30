namespace ExceptionSoftware.ExScenes
{
    public class TableFilter
    {
        public string textFilter = "";
        public SelectionOnly viewSelectedOnly = 0;
        public ImportantOnly importantOnly = 0;

        public bool HasFilter
        {
            get
            {
                if ((int)viewSelectedOnly != 0) return true;
                if ((int)importantOnly != 0) return true;
                if (textFilter != string.Empty) return true;
                return false;
            }
        }
    }
    public enum SelectionOnly { SelectionOnly = 0, True = 1, False = 2 }
    public enum ImportantOnly { ImportantOnly = 0, True = 1, False = 2 }
    //public enum FilterEnumValue { None = 0, True, False }
}
