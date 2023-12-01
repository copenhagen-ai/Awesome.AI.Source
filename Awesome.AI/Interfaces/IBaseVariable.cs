using Awesome.AI.Common;

namespace Awesome.AI.Interfaces
{
    public interface IBaseVariable
    {

        double ToPercent(UNIT _u);
        double Variable(UNIT c);
    }
}
