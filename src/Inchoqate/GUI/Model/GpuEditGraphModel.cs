namespace Inchoqate.GUI.Model;

/*

https://stackoverflow.com/questions/31081555/opengl-gpu-memory-exceeded-possible-scenarios

Scenario:
4k Image (3840 x 2160 = 8294400 pixels = 33177600 B ~= 32 MB)
=> 32MB per node
rtx2060 (6 GB GPU ram = 6144 MB available; 30% used = 4300 MB available)
=> 4300MB / 32BM = 134 Graph Nodes
    before the before OpenGL has to lay of texture buffers to vram...
    at which point the graphics will likely begin to stutter.
=> It's realistic to give a buffer to each node.


Reusing nodes:
Computing the graphs result in an iterative, breath-first fashion, rather
recursivly, depth-first allows for reusable buffers.

=> If we reuse buffers we will practically never run out of GPU memory .

 */


//public delegate IEditSource DestinationToSourceConverter(IEditDestination destination);



//public class GpuEditGraphModel : IDisposable, IEditResult<FrameBufferModel>
//{
 
//    public class EditGraphNodeModel
//    {
//        private IEdit<TextureModel, FrameBufferModel> _edit;

//        private FrameBufferModel? _resultCache;
//        public FrameBufferModel? ResultCache
//        {
//            get
//            {
//                return _resultCache;
//            }
//            private set
//            {

//            }
//        }

//        public readonly List<EditGraphNodeModel> Inputs = [];
//        public readonly List<EditGraphNodeModel> Outputs = [];

//        public bool IsComputed => _resultCache is not null;

//        public bool IsNotComputed => !IsComputed;

//        public bool IsNeeded => Outputs.All(x => x.IsNotComputed);


//        public IEdit<TextureModel, FrameBufferModel> Edit
//        {
//            get => _edit;
//            set => _edit = value;
//        }

//        public int ExpectedInputCount
//        {
//            get => _edit.ExpectedInputCount; 
//        }


//        public EditGraphNodeModel(IEdit<TextureModel, FrameBufferModel> edit)
//        {
//            _edit = edit;
                
//        }


//        public void Compute(out bool success)
//        {
//            if (Inputs.Count != _edit.ExpectedInputCount)
//            {
//                success = false;
//            }

//            var sources = Inputs
//                .Select(node =>
//                {
//                    return node.Compute(out bool nodeSuccess);
//                })
//                .ToArray();
//            success = _edit.Apply(_resultCache, sources);
//        }


//        public void ClearCache()
//        {
//            _resultCache = null;
//        }
//    }       


//    private readonly ILogger _logger = FileLoggerFactory.CreateLogger<GpuEditGraphModel>();

//    public EditGraphNodeModel Head { get; set; }
//    public EditGraphNodeModel Tail { get; set; }

//    private Dictionary<FrameBufferModel, EditGraphNodeModel?> _bufferPool = [];

//    private TextureModel? _sourceTexture;
//    private Color _background;
//    private Size _renderSize;


//    public TextureModel? SourceTexture
//    {
//        get => _sourceTexture;
//        set
//        {
//            _sourceTexture?.Dispose();
//            _sourceTexture = value;
//        }
//    }

//    public Color Background
//    {
//        get
//        {
//            return _background;
//        }
//        set
//        {
//            if (value == _background) return;
//            _background = value;
//            // TODO: may not need to reload here, just set the border color.
//            Reload();
//        }
//    }

//    /// <summary>
//    /// The size in which the final output is rendered.
//    /// </summary>
//    public Size RenderSize
//    {
//        get => _renderSize;
//        set
//        {
//            if (value == _renderSize) return;
//            _renderSize = value;
//            Reload();
//        }
//    }
        
//    #region Clean up

//    private bool disposedValue;

//    protected virtual void Dispose(bool disposing)
//    {
//        if (!disposedValue)
//        {
//            // TODO
                
//            _sourceTexture?.Dispose();

//            disposedValue = true;
//        }
//    }

//    ~GpuEditGraphModel()
//    {
//        // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
//        // The OpenGL resources have to be released from a thread with an active OpenGL Context.
//        // The GC runs on a seperate thread, thus releasing unmanaged GL resources inside the finalizer
//        // is not possible.
//        if (disposedValue == false)
//        {
//            _logger.LogWarning("GPU Resource leak! Did you forget to call Dispose()?");
//        }
//    }

//    public void Dispose()
//    {
//        Dispose(disposing: true);
//        GC.SuppressFinalize(this);
//    }

//    #endregion
//}