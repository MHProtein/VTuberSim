namespace VTuber.Character.Attribute
{
    public abstract class VCharacterAttribute
    {
        
    }
    
    public class VIntegerCharacterAttribute : VCharacterAttribute
    {
        public int Value { get; private set; }

        protected VIntegerCharacterAttribute(int value)
        {
            Value = value;
        }
    }
    
    public class VFloatCharacterAttribute : VCharacterAttribute
    {
        public float Value { get; private set; }

        protected VFloatCharacterAttribute(float value)
        {
            Value = value;
        }
    }
    
}