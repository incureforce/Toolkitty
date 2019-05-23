namespace System.ComponentModel
{
    public interface IModelInteractions
    {
        event EventHandler<ModelCallbackEventArgs> Callback;

        void Startup();

        void Cleanup();
    }
}
