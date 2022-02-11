﻿using System;
using UnityEngine;

namespace Ei.Simulation.Core
{
    class Tweener
    {
        private Transform transform;
        private float startX;
        private float startY;
        private float endX;
        private float endY;
        private float elapsed;
        private float time;

        public Tweener(Transform transform, float endX, float endY, float time) {
            this.transform = transform;
            this.elapsed = 0;
            this.startX = transform.position.x;
            this.startY = transform.position.y;
            this.endX = endX;
            this.endY = endY;
            this.time = time;
        }

        public void Update(float deltaTime) {
            this.elapsed += deltaTime;
            if (this.elapsed > this.time) {
                this.elapsed = this.time;
            }
            var fromX = this.transform.X;
            var fromY = this.transform.Y;
            this.transform.position = new Vector3(
                this.startX + (this.elapsed / this.time) * (this.endX - this.startX),
                this.startY + (this.elapsed / this.time) * (this.endY - this.startY),
                0
            );
            // Console.WriteLine($"Delta: {deltaTime} Elapsed: {this.elapsed} Position [{fromX}, {fromY}] -> [{this.transform.X}, {this.transform.Y}]");
        }
    }
}
