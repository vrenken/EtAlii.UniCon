// namespace EtAlii.UniCon.Editor
// {
//     using UnityEngine;
//     using UnityEngine.UIElements;
//
//     public class LogEventLine : VisualElement
//     {
//         public new class UxmlFactory : UxmlFactory<LogEventLine, UxmlTraits>
//         {
//             public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
//             {
//                 var visualTree = Resources.Load<VisualTreeAsset>(nameof(LogEventLine));
//                 var root = base.Create(bag, cc);
//                 visualTree.CloneTree(root);
//                 return root;
//             }
//         }
//  
//         public new class UxmlTraits : VisualElement.UxmlTraits {}
//
//         // ReSharper disable NotAccessedField.Global
//         public Label TimestampLabel;
//         public Label MessageLabel;
//         // ReSharper restore NotAccessedField.Global
//
//         public LogEventLine()
//         {
//             var visualTree = Resources.Load<VisualTreeAsset>(nameof(LogEventLine));
//             visualTree.CloneTree(this);
//             
//             TimestampLabel = this.Q<Label>("timestamp");
//             MessageLabel = this.Q<Label>("message");
//         }
//
//         public void Bind(LogEventLineViewModel viewModel)
//         {
//             TimestampLabel.text = viewModel.Timestamp;
//             MessageLabel.text = viewModel.Message;
//         }
//     }    
// }
