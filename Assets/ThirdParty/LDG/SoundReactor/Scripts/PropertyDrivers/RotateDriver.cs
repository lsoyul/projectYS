// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    [DisallowMultipleComponent]
    public class RotateDriver : PropertyDriver
    {
        public Quaternion localRotation;

        void Start()
        {
            localRotation = transform.localRotation;
        }

        protected override void DoLevel()
        {
            Quaternion rotation;

            Vector3 travel = LevelVector();

            rotation = Quaternion.Euler(travel.x, travel.y, travel.z);

            transform.localRotation = localRotation * rotation;
        }
    }
}
