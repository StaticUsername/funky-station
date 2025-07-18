// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.NPC;
using Robust.Shared.Map;

namespace Content.Server.NPC.Pathfinding;

public sealed class PathPoly : IEquatable<PathPoly>
{
    [ViewVariables]
    public readonly EntityUid GraphUid;

    [ViewVariables]
    public readonly Vector2i ChunkOrigin;

    [ViewVariables]
    public readonly byte TileIndex;

    [ViewVariables]
    public readonly Box2 Box;

    [ViewVariables]
    public PathfindingData Data;

    [ViewVariables]
    public readonly HashSet<PathPoly> Neighbors;

    public PathPoly(EntityUid graphUid, Vector2i chunkOrigin, byte tileIndex, Box2 vertices, PathfindingData data, HashSet<PathPoly> neighbors)
    {
        GraphUid = graphUid;
        ChunkOrigin = chunkOrigin;
        TileIndex = tileIndex;
        Box = vertices;
        Data = data;
        Neighbors = neighbors;
    }

    public bool IsValid()
    {
        return (Data.Flags & PathfindingBreadcrumbFlag.Invalid) == 0x0;
    }

    [ViewVariables]
    public EntityCoordinates Coordinates => new(GraphUid, Box.Center);

    // Explicitly don't check neighbors.

    public bool IsEquivalent(PathPoly other)
    {
        return GraphUid.Equals(other.GraphUid) &&
               ChunkOrigin.Equals(other.ChunkOrigin) &&
               TileIndex == other.TileIndex &&
               Data.IsEquivalent(other.Data) &&
               Box.Equals(other.Box);
    }

    public bool Equals(PathPoly? other)
    {
        return other != null &&
               GraphUid.Equals(other.GraphUid) &&
               ChunkOrigin.Equals(other.ChunkOrigin) &&
               TileIndex == other.TileIndex &&
               Data.Equals(other.Data) &&
               Box.Equals(other.Box);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is PathPoly other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GraphUid, ChunkOrigin, TileIndex, Box);
    }
}
