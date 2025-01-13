using UnityEngine;

namespace GetyourCrown.UI.UI_Utilities
{
    public static class ComponentExtensions
    {
        public static Transform FindChildReculsively(this Component component, string childName)
        {
            foreach (Transform child in component.transform)
            {
                if(child.name.Equals(childName))
                {
                    return child;
                }
                else
                {
                    Transform grandChild = FindChildReculsively(child, childName);

                    if(grandChild)
                        return grandChild;
                }    
            }

            return null;
        }
    }
}