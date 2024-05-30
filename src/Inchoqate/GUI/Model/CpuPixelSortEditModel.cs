namespace Inchoqate.GUI.Model
{
    public class CpuPixelSortEditModel : ICPUEdit
    {
        public bool Apply(PixelBufferModel source, PixelBufferModel destination)
        {
            if (source != destination)
            {
                // Makes the actual computation later easier.
                // throw new ArgumentException("The source and destination parameters should be the same.");
                return false;
            }

            // TODO

            return true;
        }
    }
}
