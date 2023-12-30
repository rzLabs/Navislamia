using System;

namespace Navislamia.Game.Scripting;

public interface IScriptService
{
    void Start();

    void RegisterFunction(string name, Func<object[], int> function);

    int RunString(string script);
}