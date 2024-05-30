namespace Inchoqate.GUI.Model
{
    public class CpuPixelSortEditModel : LinearEdit<PixelBufferModel, PixelBufferModel>
    {
        public override bool Apply(PixelBufferModel destination, params PixelBufferModel[] source)
        {
            if (source.Length == 0 || source[0] != destination)
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
