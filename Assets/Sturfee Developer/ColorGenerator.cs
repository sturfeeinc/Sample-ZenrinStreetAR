using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sturfee.Internal.Developer
{
    public class ColorGenerator
    {
        private List<Color> _colors;
        private System.Random _rnd;

        public ColorGenerator()
        {
            _colors = new List<Color>() {
                new Color(0, 0.7176f, 0.9608f, 0.4f),           // R: 0 G: 182 B: 245
                new Color(0, 0.55f, 0.851f, 0.4f),              // R: 0 G: 140 B: 217
                new Color(0, 0.298f, 0.655f, 0.4f),             // R: 0 G: 76 B: 167
                new Color(0.408f, 0.1608f, 0.5804f, 0.4f),      // R: 104 G: 41 B: 148
                new Color(0, 0.7176f, 0.149f, 0.4f),            // R: 151 G: 39 B: 123
                new Color(0, 0.1373f, 0.3686f, 0.4f),           // R: 192 G: 35 B: 94
                new Color(0.933f, 0.1373f, 0.149f, 0.4f),       // R: 238 G: 35 B: 38
                new Color(0.953f, 0.4039f, 0.024f, 0.4f),       // R: 243 G: 103 B: 6
                new Color(1, 0.7686f, 0, 0.4f),                 // R: 255 G: 196 B: 0
                new Color(1, 0.9294f, 0, 0.4f),                 // R: 255 G: 237 B: 2
                new Color(0.8f, 0.8588f, 0, 0.4f),              // R: 204 G: 219 B: 2
                new Color(0.54f, 0.78f, 0.1804f, 0.4f),         // R: 138 G: 199 B: 46
                new Color(0, 0.655f, 0.216f, 0.4f),             // R: 0 G: 167 B: 55
                new Color(0, 0.54f, 0.29f, 0.4f),               // R: 0 G: 138 B: 74
            };

            _rnd = new System.Random();
        }

        public Color GetRandomColor()
        {
            return _colors[_rnd.Next(0, _colors.Count)];
        }
    }
}