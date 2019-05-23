namespace System.ComponentModel
{
    public class ModelCallbackEventArgs : EventArgs
    {
        public static readonly ModelCallbackEventArgs
            Default = new ModelCallbackEventArgs(),
            Success = new ModelCallbackEventArgs(true),
            Failure = new ModelCallbackEventArgs(false);

        public ModelCallbackEventArgs(bool? result)
        {
            Result = result;
        }

        public ModelCallbackEventArgs()
        {
        }

        public bool? Result { get; }
    }
}
