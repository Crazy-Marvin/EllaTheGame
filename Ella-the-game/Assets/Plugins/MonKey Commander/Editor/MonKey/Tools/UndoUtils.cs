using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonKey.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey
{
    public abstract class AbstractUndo<T> where T : Object
    {
        /// <summary>
        /// Registers objects 
        /// before a long series of operation,
        ///  so that the undo can be registered 
        /// until the state it was in before the operations
        /// </summary>
        /// <param name="valuesToRegister"></param>
        public abstract void Register(params T[] valuesToRegister);

        /// <summary>
        /// Creates an undo when all the operations 
        /// are done to make sure all the undo is properly collapse
        /// You must create an Undo id 
        /// and collapse it after calling this method though.
        /// </summary>
        public abstract void RecordUndo();

    }

    /// <summary>
    /// Registers transforms before a series of movement
    /// </summary>
    public class TransformUndo : AbstractUndo<Transform>
    {
        private Vector3[] positions;
        private Quaternion[] rotations;
        private Vector3[] localScales;

        private Transform[] currentValues;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valuesToRegister"></param>
        public override void Register(params Transform[] valuesToRegister)
        {
            positions = valuesToRegister.Convert(_ => _.position).ToArray();
            rotations = valuesToRegister.Convert(_ => _.rotation).ToArray();
            localScales = valuesToRegister.Convert(_ => _.localScale).ToArray();
            currentValues = valuesToRegister;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void RecordUndo()
        {
            Vector3[] newPositions = currentValues.Convert(_ => _.position).ToArray();
            Quaternion[] newRotations = currentValues.Convert(_ => _.rotation).ToArray();
            Vector3[] newLocalScales = currentValues.Convert(_ => _.localScale).ToArray();

            for (int i = 0; i < currentValues.Length; i++)
            {
                Transform currentValue = currentValues[i];

                currentValue.position = positions[i];
                currentValue.rotation = rotations[i];
                currentValue.localScale = localScales[i];
            }

            for (int i = 0; i < currentValues.Length; i++)
            {
                Transform currentValue = currentValues[i];
                Undo.RecordObject(currentValue,"transform");

                currentValue.position = newPositions[i];
                currentValue.rotation = newRotations[i];
                currentValue.localScale = newLocalScales[i];
            }
        }
    }


}
