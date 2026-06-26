using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FunkePlayer.Player {
    public class FunkePlayerSetup : MonoBehaviour
    {
        public Transform[] transforms;
        [Space]
        public Transform headOrigin;
    
        public Transform lOrigin;
        
        public Transform rOrigin;

        void Update()
        {
            transforms[0].position = headOrigin.position;
            transforms[0].rotation = headOrigin.rotation;

            transforms[1].position = lOrigin.position;
            transforms[1].rotation = lOrigin.rotation;

            transforms[2].position = rOrigin.position;
            transforms[2].rotation = rOrigin.rotation;
        }
    }
}
