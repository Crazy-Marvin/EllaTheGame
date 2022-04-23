using UnityEngine;
using System.Collections.Generic;

namespace MonKey.Commands
{
    public class ParentingUtilities
    {

        /// <summary>
        /// Finds a common parent for all objects
        /// </summary>
        /// <param name="objects"></param>
        /// <returns> the common parent, or null if none </returns>
        public static Transform FindEarliestCommonParent(GameObject[] objects)
        {
            List<Transform> leftToCheck = new List<Transform>();

            foreach (GameObject obj in objects)
            {
                leftToCheck.Add(obj.transform);
            }

            while (leftToCheck.Count > 1)
            {
                Transform one = leftToCheck[0];
                Transform two = leftToCheck[1];

                Transform parent = FindEarliestCommonParent(one, two);
                if (parent == null)
                    return null;

                leftToCheck.Remove(one);
                leftToCheck.Remove(two);
                leftToCheck.Add(parent);

            }

            return leftToCheck[0];

        }

        /// <summary>
        /// Finds a common parent
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns> the earliest common transform, or null if none</returns>
        public static Transform FindEarliestCommonParent(Transform one, Transform two)
        {
            if (one == two)
            {
                return one;
            }

            List<Transform> ones = FindParentChain(one);
            List<Transform> twos = FindParentChain(two);

            if (ones.Count == 0 || twos.Count == 0)
            {
                return null;
            }

            foreach (Transform trans in ones)
            {
                foreach (Transform trans2 in twos)
                {
                    if (trans == trans2)
                    {
                        return trans;
                    }
                }
            }
            return null;
        }

        public static List<Transform> FindParentChain(Transform trans)
        {
            Transform current = trans;
            List<Transform> transforms = new List<Transform>();
            while (current != null)
            {
                transforms.Add(current);
                current = current.parent;
            }
            return transforms;
        }

    }
}
