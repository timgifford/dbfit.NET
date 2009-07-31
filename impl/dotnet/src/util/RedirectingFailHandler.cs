using System;
using System.Collections.Generic;
using System.Text;
using fit;
namespace dbfit.util
{
    //bugfix for standard fail keyword handler, handles fail[null] correctly
    public class RedirectingFailHandler:fitnesse.handlers.FailKeywordHandler
    {
        public override void HandleCheck(Fixture fixture, Parse cell, Accessor accessor)
        {
            string expected = cell.Text.Substring("fail[".Length, cell.Text.Length - ("fail[".Length + 1));
            Parse newCell = new Parse("td", expected, null, null);
            ICellHandler handler = CellOperation.GetHandler(fixture, newCell, accessor.ParameterType);
            if (handler.HandleEvaluate(fixture, newCell, accessor))
            {
                fixture.Wrong(cell);
            }
            else
            {
                fixture.Right(cell);
            }
        }
    }
}
