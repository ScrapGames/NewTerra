namespace ScrapGames
{
    using UnityEngine;
    public static class Utils
    {
        public static Vector2 ClampPosition(Vector2 value, Vector2 min, Vector2 max)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            return value;
        }

        public static bool CheckLayerInMask(int layer, LayerMask mask)
        {
            return (mask == (mask | 1 << layer));
        }

        public static int GetClosestObject<T>(T[] list, Vector3 sourcePos) where T : MonoBehaviour
        {
            float record = Mathf.Infinity;
            int winnerID = -1;

            for (int i = 0; i < list.Length; i++)
            {
                float dist = Vector3.Distance(sourcePos, list[i].transform.position);
                if (dist < record)
                {
                    // New winner!
                    winnerID = i;
                    record = dist;
                }
            }
            return winnerID;
        }

        public static int GetClosestHit(RaycastHit[] list, Vector3 sourcePos)
        {
            float record = Mathf.Infinity;
            int winnerID = -1;

            for (int i = 0; i < list.Length; i++)
            {
                float dist = Vector3.Distance(sourcePos, list[i].transform.position);
                if (dist < record)
                {
                    // New winner!
                    winnerID = i;
                    record = dist;
                }
            }
            return winnerID;
        }
    }
}