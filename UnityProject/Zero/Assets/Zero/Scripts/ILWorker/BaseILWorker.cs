using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    public abstract class BaseILWorker
    {
        public abstract void Invoke(string clsName, string methodName);
    }
}