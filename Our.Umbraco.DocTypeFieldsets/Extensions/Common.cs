namespace Our.Umbraco.DocTypeFieldsets.Extensions
{
    public class Common
    {
        public static string ConfigName
        {
            get
            {
                return "DocTypeFieldsets";
            }
        }

        public static string FieldSetDisplay
        {
            get { return "h2"; } // "", h2, h2-pp
        }
    }
}