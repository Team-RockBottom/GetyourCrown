using System;

namespace GetYourCrown.Singletons
{
    public abstract class Singleton<T>
        where T : Singleton<T>
    {
        public static T instance
        {
            get
            {
                if(s_instance == null)
                {
                    s_instance = (T)Activator.CreateInstance(typeof(T));
                }

                return s_instance;
            }
        }

        private static T s_instance;
    }
}
