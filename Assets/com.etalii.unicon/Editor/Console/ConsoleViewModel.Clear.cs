namespace EtAlii.UniCon.Editor
{
    public partial class ConsoleViewModel
    {
        public void Clear()
        {
            LogSink.Instance.Clear();
            ConfigureStream();
        }
    }    
}
