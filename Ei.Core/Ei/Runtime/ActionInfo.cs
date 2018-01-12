namespace Ei.Runtime
{
    public struct ActionInfo : IActionInfo
    {
        public InstitutionCodes Code { get; private set; }
        public object[] Parameters { get; private set; }

        public bool IsOk {
            get {
                return this.Code == InstitutionCodes.Ok;
            }
        }
        public bool IsAcceptable {
            get {
                return this.Code == InstitutionCodes.Ok ||
                                    this.Code == InstitutionCodes.OkButDoNotContinue;
            }
        }
        public bool IsBreak {
            get {
                return this.Code == InstitutionCodes.OkButDoNotContinue;
            }
        }
        public bool IsNotOk {
            get {
                return this.Code != InstitutionCodes.Ok && this.Code != InstitutionCodes.OkButDoNotContinue;
            }
        }

        public static IActionInfo AccessDenied = new ActionInfo(InstitutionCodes.AccessDenied);

        public ActionInfo(InstitutionCodes code, params object[] parameters)
        {
            this.Code = code;
            this.Parameters = parameters;
        }

        public static IActionInfo Ok = new ActionInfo(InstitutionCodes.Ok);
        public static IActionInfo OkButDoNotContinue = new ActionInfo(InstitutionCodes.OkButDoNotContinue);

        public static IActionInfo FailedPreconditions = new ActionInfo(InstitutionCodes.ActivityNotReachable);
        //public static IActionInfo WaitForDecision = new ActionInfo(InstitutionCodes.WaitingForDecision);
        public static IActionInfo Failed = new ActionInfo(InstitutionCodes.Failed);
        //public static IActionInfo WaitingToJoin = new ActionInfo(InstitutionCodes.WaitingToJoin);
        public static IActionInfo AgentNotCloned = new ActionInfo(InstitutionCodes.AgentNotCloned);
        public static IActionInfo StateNotReachable = new ActionInfo(InstitutionCodes.StateNotReachable);

    }
}
