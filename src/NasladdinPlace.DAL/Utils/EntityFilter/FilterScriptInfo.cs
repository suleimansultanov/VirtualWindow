namespace NasladdinPlace.DAL.Utils.EntityFilter
{
    public class FilterScriptInfo
    {
        public string Script { get; }
        public object FilterParameter { get; }

        public FilterScriptInfo(string script, object filterParameter)
        {
            Script = script;
            FilterParameter = filterParameter;
        }
    }
}