namespace EtAlii.UniCon.Editor
{
    using UnityEngine;

    public class SettingsViewModel : ScriptableObject
    {
        public static SettingsViewModel Instance
        {
            get
            {
                if(_instance == null) _instance = CreateInstance<SettingsViewModel>();
                return _instance;
            }
        }
        private static SettingsViewModel _instance;
        
        public void Init()
        {
            
        }
    }    
}
