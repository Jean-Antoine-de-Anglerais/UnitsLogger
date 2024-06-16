//===============================
// Code from Mr . P (mr.p.4466)
//===============================

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

public class MyClass
{
    public void MyMethod()
    {
        // Original method implementation
    }

    public void MyInjectedMethod()
    {
        // Method to be called within the transpiler
    }
}

[HarmonyPatch(typeof(MyClass), "MyMethod")]
public static class MyClassPatch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        var method = AccessTools.Method(typeof(MyClass), nameof(MyClass.MyInjectedMethod));

        // Find the appropriate place to insert the call
        // For demonstration, we insert at the beginning
        // Adjust the index as per your needs
        int insertionIndex = 0;

        // Load 'this' onto the stack (assuming instance method)
        codes.Insert(insertionIndex++, new CodeInstruction(OpCodes.Ldarg_0)); // Ldarg_0 loads the instance for instance methods

        // Call the injected method
        codes.Insert(insertionIndex++, new CodeInstruction(OpCodes.Call, method));

        return codes.AsEnumerable();
    }
}