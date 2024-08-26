// <copyright file="ChunkInternals.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.Internal
{
    using Unity.Entities;

    public static class ChunkInternals
    {
        public static int GetChunkBufferSize()
        {
            return Chunk.kChunkBufferSize;
        }
    }
}

#endif // UNITY_ENTITES
