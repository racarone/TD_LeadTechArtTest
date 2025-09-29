using UnityEngine;
using TMPro;

namespace TD.LevelCompletedScreen
{
    public class SineWaveTextAnimation : MonoBehaviour
    {
        private const float amplitude = 5f; // How far the letters float
        private const float frequency = 2f; // Speed of the float
        private const float waveOffset = 0.2f; // Offset between each letter

        private TMP_Text _textMesh;
        private Vector3[] _originalVertices;

        private void Start()
        {
            _textMesh = GetComponent<TMP_Text>();
            _textMesh.ForceMeshUpdate();
            _originalVertices = _textMesh.mesh.vertices;
        }

        private void Update()
        {
            AnimateText();
        }

        private void AnimateText()
        {
            // Get the updated mesh and vertices
            _textMesh.ForceMeshUpdate();
            var mesh = _textMesh.mesh;
            var vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                int charIndex = i / 4; // Each character is represented by 4 vertices
                float wave = Mathf.Sin(Time.time * frequency + charIndex * waveOffset);
                vertices[i].y = _originalVertices[i].y + wave * amplitude;
            }

            // Apply the modified vertices back to the mesh
            mesh.vertices = vertices;
            _textMesh.canvasRenderer.SetMesh(mesh);
        }
    }
}