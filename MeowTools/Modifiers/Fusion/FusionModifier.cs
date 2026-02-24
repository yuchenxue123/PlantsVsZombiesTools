using System.Diagnostics;

namespace MeowTools.Modifiers.Fusion
{
    public class FusionModifier : Modifier
    {
        public FusionModifier(Process process) : base(process)
        {
        }


        public override bool ModifySunshine(int count)
        {
            throw new System.NotImplementedException();
        }

        public override bool ModifyCoin(int count)
        {
            throw new System.NotImplementedException();
        }
    }
}