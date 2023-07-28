namespace EtAlii.UniCon.Editor
{
    using UnityEditor;
    // using UnityEngine;

    [FilePath("ProjectSettings/UniConProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class ProjectSettings : ScriptableSingleton<ProjectSettings>
    {
        // [SerializeField] private CustomFilter;
        // [SerializeField] private string _someValue;
        //
        // public string SomeValue
        // {
        //     get => _someValue;
        //     set
        //     {
        //         _someValue = value;
        //         _changeCount++;
        //         Save(true);
        //     }
        // }
        //
        // public int ChangeCount => _changeCount;
 
    }
}