﻿namespace Inchoqate.GUI.Model
{
    public interface IGpuRenderQueue
    {
        /// <summary>
        /// Returns a reference to the final result of the render queue.
        /// Returns null if there is no source given.
        /// </summary>
        /// <returns></returns>
        public FrameBufferModel? Apply();
    }
}
