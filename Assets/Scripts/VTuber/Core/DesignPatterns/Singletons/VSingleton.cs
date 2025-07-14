namespace VTuber.Core.Foundation
{
    public class VSingleton<T> where T: new()
    {
        public static T Instance
        {
            get
            {
                if(instance is null)
                    instance = new T();
                
                return instance;
            }
        }

        protected static T instance;
    }
}