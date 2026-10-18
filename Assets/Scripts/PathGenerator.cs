using System.Collections.Generic;
using UnityEngine;

namespace Cobbox
{
    // Builds the start platform and keeps a zigzag path of tiles ahead of the ball.
    // The path is planned in segments: a normal straight run, or a jump section
    // (jump-zone tiles -> gap -> landing pillar). Every segment starts with a turn,
    // so gaps always sit on a straight line and the player never has to turn and jump at once.
    // Tiles are reused: when one finishes falling it is moved to the front of the path.
    public class PathGenerator : MonoBehaviour
    {
        struct TileStep
        {
            public Vector3 offset;
            public bool jumpZone;
        }

        [SerializeField] Tile tilePrefab;
        [SerializeField] int startPlatformSize = 3;
        [SerializeField] int tilesAhead = 40;

        [Header("Straight runs")]
        [SerializeField] int minStraight = 1;
        [SerializeField] int maxStraight = 5;

        [Header("Jump sections")]
        [SerializeField, Range(0f, 1f)] float jumpChance = 0.25f;
        [SerializeField] int safeStartTiles = 15;
        [SerializeField] int jumpZoneTiles = 2;
        [SerializeField] int minGap = 2;
        [SerializeField] int maxGap = 3;
        [SerializeField] float[] heightSteps = { 0f, 0.5f, 1f };
        [SerializeField] int landingTiles = 2;

        readonly Queue<Tile> pool = new Queue<Tile>();
        readonly Queue<TileStep> planned = new Queue<TileStep>();
        Vector3 lastPosition;
        Vector3 direction = Vector3.right;
        int tilesSpawned;
        float tileY;

        void Start()
        {
            // Tile pivot is the cube's center; place it so the top surface sits at y = 0.
            tileY = -tilePrefab.transform.localScale.y * 0.5f;

            BuildStartPlatform();
            for (int i = 0; i < tilesAhead; i++)
                SpawnNext();
        }

        public void Recycle(Tile tile)
        {
            pool.Enqueue(tile);
            SpawnNext();
        }

        void BuildStartPlatform()
        {
            int half = startPlatformSize / 2;
            for (int x = -half; x <= half; x++)
                for (int z = -half; z <= half; z++)
                    PlaceTile(new Vector3(x, tileY, z), false);

            // The path continues from the platform's front edge, in line with the ball.
            lastPosition = new Vector3(0f, tileY, half);
        }

        void SpawnNext()
        {
            if (planned.Count == 0)
                PlanSegment();

            TileStep step = planned.Dequeue();
            lastPosition += step.offset;
            tilesSpawned++;
            PlaceTile(lastPosition, step.jumpZone);
        }

        void PlanSegment()
        {
            direction = direction == Vector3.forward ? Vector3.right : Vector3.forward;

            if (tilesSpawned >= safeStartTiles && Random.value < jumpChance)
                PlanJump();
            else
                PlanStraight(Random.Range(minStraight, maxStraight + 1), false);
        }

        void PlanJump()
        {
            PlanStraight(jumpZoneTiles, true);

            int gap = Random.Range(minGap, maxGap + 1);
            float rise = heightSteps[Random.Range(0, heightSteps.Length)];
            planned.Enqueue(new TileStep { offset = direction * (gap + 1) + Vector3.up * rise });

            PlanStraight(landingTiles - 1, false);
        }

        void PlanStraight(int count, bool jumpZone)
        {
            for (int i = 0; i < count; i++)
                planned.Enqueue(new TileStep { offset = direction, jumpZone = jumpZone });
        }

        void PlaceTile(Vector3 position, bool jumpZone)
        {
            Tile tile = pool.Count > 0 ? pool.Dequeue() : Instantiate(tilePrefab, transform);
            tile.Place(this, position, jumpZone);
        }
    }
}
