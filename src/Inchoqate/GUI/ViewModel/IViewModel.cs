namespace Inchoqate.GUI.ViewModel
{
    interface IViewModel<out TModel>
    {
        public TModel Model { get; }
    }
}
