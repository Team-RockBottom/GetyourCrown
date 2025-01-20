using System;

namespace GetyourCrown.UI.UI_Singleton
{
    public abstract class UI_Singleton<T> where T : UI_Singleton<T>
    {
        public static T instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = (T)Activator.CreateInstance(typeof(T));
                }

                return s_instance;
            }
        }

        private static T s_instance;
    }
}