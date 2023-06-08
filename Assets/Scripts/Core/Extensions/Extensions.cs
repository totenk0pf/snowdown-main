using UnityEngine;

namespace Core.Extensions {
    public static class Extensions {
        public static bool CheckLayer(this int layer, LayerMask layerMask) => layerMask == (layerMask | (1 << layer));
    }
}