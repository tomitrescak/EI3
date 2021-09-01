namespace Ei.Core.Runtime
{
    public interface IActionInfo
    {
        InstitutionCodes Code { get; }
        object[] Parameters { get; }
        bool IsOk { get; }
        bool IsNotOk { get; }
        bool IsAcceptable { get; }
        bool IsBreak { get; }
    }
}
