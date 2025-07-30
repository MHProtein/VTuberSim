using System;

namespace Editor.VTuber.SOCreator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SOCreateLimitAttribute : Attribute
    {
        public int soCreateCount;
        
        public SOCreateLimitAttribute(int amount)
        {
            soCreateCount = amount;
        }
    }
}