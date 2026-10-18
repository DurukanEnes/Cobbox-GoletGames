using System.Collections;
using UnityEngine;

namespace Cobbox
{
    // One path tile. Falls after the ball leaves it, then goes back to the PathGenerator's pool.
    // Jump-zone tiles lead up to a gap: touching the screen there jumps instead of turning.
    public class Tile : MonoBehaviour
    {
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] float dropDelay = 0.2f;
        [SerializeField] float dropDuration = 1f;
        [SerializeField] float dropGravity = 20f;
        [SerializeField] Color jumpZoneColor = new Color(1f, 0.55f, 0.1f);

        public bool IsJumpZone { get; private set; }

        PathGenerator owner;
        Renderer tileRenderer;
        MaterialPropertyBlock colorBlock;
        bool dropping;

        void Awake()
        {
            tileRenderer = GetComponent<Renderer>();
            colorBlock = new MaterialPropertyBlock();
        }

        public void Place(PathGenerator generator, Vector3 position, bool jumpZone)
        {
            owner = generator;
            dropping = false;
            StopAllCoroutines();
            transform.position = position;
            SetJumpZone(jumpZone);
            gameObject.SetActive(true);
        }

        public void Drop()
        {
            if (dropping || !gameObject.activeInHierarchy) return;
            dropping = true;
            StartCoroutine(DropRoutine());
        }

        void SetJumpZone(bool jumpZone)
        {
            IsJumpZone = jumpZone;
            if (tileRenderer == null) return;

            colorBlock.Clear();
            if (jumpZone)
                colorBlock.SetColor(BaseColorId, jumpZoneColor);
            tileRenderer.SetPropertyBlock(colorBlock);
        }

        IEnumerator DropRoutine()
        {
            yield return new WaitForSeconds(dropDelay);

            float velocity = 0f;
            for (float t = 0f; t < dropDuration; t += Time.deltaTime)
            {
                velocity += dropGravity * Time.deltaTime;
                transform.position += Vector3.down * (velocity * Time.deltaTime);
                yield return null;
            }

            gameObject.SetActive(false);
            owner.Recycle(this);
        }
    }
}
