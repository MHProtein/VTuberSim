using System;

namespace VTuber.Editor.Attributes
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