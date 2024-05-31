using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(menuName = "InputManagerCreator")]
public class InputManagerCreator : ScriptableObject
{
    public Options options;
    public InputActionAsset inputActionMap;
    
    private const int kSpacesPerIndentLevel = 4;

    [Serializable]
    public struct Options
    {
        public string className;
        public string sourceAssetPath;
    }

    private string CreateInputListCode()
    {
        var writer = new Writer
        {
            buffer = new StringBuilder()
        };

        
        // Using
        writer.WriteLine("using System;");
        writer.WriteLine("using System.Collections;");
        writer.WriteLine("using System.Collections.Generic;");
        writer.WriteLine("using UnityEngine;");
        writer.WriteLine("using UnityEngine.InputSystem;");
        writer.WriteLine("using UnityEngine.InputSystem.Utilities;");
        writer.WriteLine();
        
        // Begin class
        writer.WriteLine($"public class {options.className} : MonoBehaviour");
        writer.BeginBlock();
        
        // Create instance variable
        writer.WriteLine($"public static {options.className} instance;");
        
        // Create variables
        writer.WriteLine("public static Action<string> newControllerUsed;");
        writer.WriteLine("[HideInInspector] public string lastDeviceUsed;");
        writer.WriteLine($"private {inputActionMap.name} {inputActionMap.name};");
        
        // Create InputAction variables
        foreach (var actionMap in inputActionMap.actionMaps)
        {
            writer.WriteLine($"[Header(\"{actionMap.name}\")]");
            foreach (var action in actionMap.actions)
            {
                string actionName = (char.ToLower(action.name[0])+action.name.Substring(1)).Replace(" ", "");
                writer.WriteLine($"[HideInInspector] public InputAction {actionName};");
            }
            writer.WriteLine();
        }
        
        // Awake
        writer.WriteLine($"void Awake()");
        writer.BeginBlock();
        
        writer.WriteLine($"if (instance!=null) Destroy(this.gameObject);");
        writer.WriteLine($"instance = this;");
        writer.WriteLine($"{inputActionMap.name} = new {inputActionMap.name}();");
        writer.WriteLine();

        foreach (var actionMap in inputActionMap.actionMaps)
        {
            writer.WriteLine($"Init{actionMap.name}Inputs();");
            writer.WriteLine($"Enable{actionMap.name}Inputs();");
            writer.WriteLine();
        }
        
        writer.WriteLine("AttachToDeviceDetection();");
        
        writer.EndBlock();
        writer.WriteLine();
        
        //Init Inputs per map
        foreach (var actionMap in inputActionMap.actionMaps)
        {
            writer.WriteLine($"public void Init{actionMap.name}Inputs()");
            writer.BeginBlock();

            foreach (var action in actionMap.actions)
            {
                string actionName = (char.ToLower(action.name[0])+action.name.Substring(1)).Replace(" ", "");
                writer.WriteLine($"{actionName} = {inputActionMap.name}.{actionMap.name}.{action.name.Replace(" ","")};");
            }
            
            writer.EndBlock();
            writer.WriteLine();
        }

        //Enable Inputs per map
        foreach (var actionMap in inputActionMap.actionMaps)
        {
            writer.WriteLine($"public void Enable{actionMap.name}Inputs()");
            writer.BeginBlock();

            foreach (var action in actionMap.actions)
            {
                string actionName = (char.ToLower(action.name[0])+action.name.Substring(1)).Replace(" ", "");
                writer.WriteLine($"{actionName}.Enable();");
            }
            
            writer.EndBlock();
        }
        writer.WriteLine();
        
        //Disable Inputs per map
        foreach (var actionMap in inputActionMap.actionMaps)
        {
            writer.WriteLine($"public void Disable{actionMap.name}Inputs()");
            writer.BeginBlock();

            foreach (var action in actionMap.actions)
            {
                string actionName = (char.ToLower(action.name[0])+action.name.Substring(1)).Replace(" ", "");
                writer.WriteLine($"{actionName}.Disable();");
            }
            
            writer.EndBlock();
        }
        writer.WriteLine();
        
        //Attach input to device detection
        writer.WriteLine($"private void AttachToDeviceDetection()");
        writer.BeginBlock();

        foreach (var actionMap in inputActionMap.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                string actionName = (char.ToLower(action.name[0])+action.name.Substring(1)).Replace(" ", "");
                if (actionName == "point" || actionName == "navigate")
                {
                    writer.WriteLine($"{actionName}.performed += DetectDevice;");
                    continue;
                }
                writer.WriteLine($"{actionName}.started += DetectDevice;");
            }
        }
        
        writer.EndBlock();
        
        writer.WriteLine();
        //Device detection
        writer.WriteLine($"private void DetectDevice(InputAction.CallbackContext context)");
        writer.BeginBlock();
        
        writer.WriteLine($"lastDeviceUsed = context.action.activeControl.device.name;");
        writer.WriteLine($"newControllerUsed?.Invoke(lastDeviceUsed);");
        
        writer.EndBlock();
        
        // End class
        writer.EndBlock();


        return writer.buffer.ToString();
    }

    [ContextMenu("Create input manager")]
    public void CreateInputListAsset()
    {
        if (!Path.HasExtension(options.sourceAssetPath))
            options.sourceAssetPath += ".cs";

        var code = CreateInputListCode();

        if (File.Exists(options.sourceAssetPath))
        {
            var existingCode = File.ReadAllText(options.sourceAssetPath);
            if (existingCode == code)
                return;
        }

        File.WriteAllText(options.sourceAssetPath, code);
    }
    
    
    

    // <summary>
    // Detect which device is the input used from
    // </summary>
    // private void DetectDevice(InputAction.CallbackContext context)
    // {
    //     lastDeviceUsed = context.action.activeControl.device.name;
    //     newControllerUsed?.Invoke(lastDeviceUsed);
    // }

    internal struct Writer
            {
                public StringBuilder buffer;
                public int indentLevel;
    
                public void BeginBlock()
                {
                    WriteIndent();
                    buffer.Append("{\n");
                    ++indentLevel;
                }
    
                public void EndBlock()
                {
                    --indentLevel;
                    WriteIndent();
                    buffer.Append("}\n");
                }
    
                public void WriteLine()
                {
                    buffer.Append('\n');
                }
    
                public void WriteLine(string text)
                {
                    if (!text.All(char.IsWhiteSpace))
                    {
                        WriteIndent();
                        buffer.Append(text);
                    }
                    buffer.Append('\n');
                }
    
                public void Write(string text)
                {
                    buffer.Append(text);
                }
    
                public void WriteIndent()
                {
                    for (var i = 0; i < indentLevel; ++i)
                    {
                        for (var n = 0; n < kSpacesPerIndentLevel; ++n)
                            buffer.Append(' ');
                    }
                }
            }
}
