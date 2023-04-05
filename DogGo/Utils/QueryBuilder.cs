using System.Text;

namespace DogGo.Utils;

public class QueryBuilder
{
    private  string _selectStatement;
    private  string _fromStatement;
    private  string _whereClause;

    public void select(List<string> arguments)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var argument in arguments)
        {
            sb.AppendLine(argument.ToString());
        }
        _selectStatement = sb.ToString();
    }

}
